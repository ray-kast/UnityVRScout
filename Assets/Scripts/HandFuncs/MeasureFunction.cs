using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public enum MeasureUnits {
    Imperial,
    Metric,
  }

  public class MeasureFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Select",
    };

    IHandController hand;
    GameObject lineObj, readoutObj;
    LineRenderer line;
    MeasureReadout readout;
    Vector3 pos1, pos2;
    int state = 0;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    Vector3 TrackPos => hand.gameObject.transform.position;

    public MeasureFunction() { }

    public void Enable(IHandController ctl) {
      if (lineObj != null) GameObject.Destroy(lineObj);
      if (readoutObj != null) GameObject.Destroy(readoutObj);

      hand = ctl;
      ctl.Events.TriggerPressed += Advance;
      ctl.OnFixedUpdate += FixedUpdate;

      lineObj = GameObject.Instantiate(ctl.Player.MeasureLine, ctl.gameObject.transform, false);
      readoutObj = GameObject.Instantiate(ctl.Player.MeasureReadout, ctl.gameObject.transform, false);

      line = lineObj.GetComponent<LineRenderer>();
      readout = readoutObj.GetComponent<MeasureReadout>();

      readout.Scale = 0.002f;
      readout.Size = new Vector2(100.0f, 30.0f);
      readout.Position = new Vector3(0.0f, 0.0f, 0.1f);

      readout.transform.localRotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);

      UpdateLine();
    }

    public void Disable(IHandController ctl) {
      ctl.Events.TriggerPressed -= Advance;
      ctl.OnFixedUpdate -= FixedUpdate;

      GameObject.Destroy(lineObj);
      GameObject.Destroy(readoutObj);
    }

    void UpdateLine() {
      switch (state) {
        case 0:
          line.enabled = false;
          break;
        case 1:
          line.enabled = true;
          line.SetPosition(0, pos1);
          line.SetPosition(1, TrackPos);
          break;
        case 2:
          line.enabled = true;
          line.SetPosition(0, pos1);
          line.SetPosition(1, pos2);
          break;
      }

      UpdateReadout(false);
    }

    void UpdateReadout(float dist) {
      dist *= hand.Player.MeasureConvertRatio;

      string str = null;

      switch (hand.Player.MeasureUnits) {
        case MeasureUnits.Imperial: {
            dist *= 1.0f / 0.0254f;

            str = $@"{dist % 12.0f:N2}""";

            var feet = Mathf.Floor(dist / 12.0f);

            if (feet > 1e-5f) str = $"{Mathf.Floor(dist / 12.0f):N0}' {str}";

            break;
          }
        case MeasureUnits.Metric: {
            dist *= 1000.0f;

            str = $"{dist:N0} mm";

            break;
          }
      }

      readout.Text = str;
    }

    void UpdateReadout(bool dynamic) {
      if (dynamic && state != 1) return; // No point repeatedly setting the text to a static value

      switch (state) {
        case 0: readout.Text = ""; break;
        case 1: UpdateReadout((TrackPos - pos1).magnitude); break;
        case 2: UpdateReadout((pos2 - pos1).magnitude); break;
      }
    }

    void Advance(object sender, ControllerInteractionEventArgs e) {
      switch (state) {
        case 0: pos1 = TrackPos; break;
        case 1: pos2 = TrackPos; break;
      }

      state = (state + 1) % 3;
      UpdateLine();
    }

    void FixedUpdate() {
      switch (state) {
        case 1:
          line.SetPosition(1, TrackPos);
          break;
      }

      UpdateReadout(true);
    }
  }
}
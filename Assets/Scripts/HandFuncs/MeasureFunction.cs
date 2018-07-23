using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class MeasureFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Select",
    };

    IHandController hand;
    GameObject lineObj;
    LineRenderer line;
    int state = 0;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public MeasureFunction() { }

    // TODO: None of this code is currently right
    public void Enable(IHandController ctl) {
      if (lineObj != null) GameObject.Destroy(lineObj);

      hand = ctl;
      ctl.Events.TriggerPressed += Advance;
      ctl.OnFixedUpdate += FixedUpdate;

      lineObj = GameObject.Instantiate(ctl.Player.MeasureLine, ctl.gameObject.transform, false);

      line = lineObj.GetComponent<LineRenderer>();
      line.enabled = false;
    }

    // TODO: None of this code is currently right
    public void Disable(IHandController ctl) {
      ctl.Events.TriggerPressed -= Advance;
      ctl.OnFixedUpdate -= FixedUpdate;

      GameObject.Destroy(lineObj);
    }

    void Advance(object sender, ControllerInteractionEventArgs e) {
      switch (state) {
        case 0:
          line.SetPosition(0, hand.gameObject.transform.position);
          line.enabled = true;
          break;
        case 1:
          line.SetPosition(1, hand.gameObject.transform.position);
          break;
        case 2:
          line.enabled = false;
          break;
      }

      state = (state + 1) % 3;
    }

    void FixedUpdate() {
      switch (state) {
        case 1:
          line.SetPosition(1, hand.gameObject.transform.position);
          break;
      }
    }
  }
}
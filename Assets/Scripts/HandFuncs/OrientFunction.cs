using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class OrientFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.GripTooltip] = "Move",
    };

    GameObject readoutObj;
    SimpleReadout readout;
    Vector3 lastPos, lastDoubleDiff;
    VRTK_ControllerEvents events, otherEvents;
    IPlayerController player;
    bool grab;

    Vector3 DoubleDiff => otherEvents.transform.position - events.transform.position;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public OrientFunction() { }

    public void Enable(IHandController ctl) {
      events = ctl.Events;
      otherEvents = ctl.Other.Events;
      player = ctl.Player;

      ctl.Events.GripPressed += OnGrabStart;
      ctl.Events.GripReleased += OnGrabEnd;
      ctl.Other.Events.GripPressed += OnDoubleGrabStart;
      ctl.Other.Events.GripReleased += OnGrabEnd;
      ctl.OnFixedUpdate += FixedUpdate;

      readoutObj = GameObject.Instantiate(ctl.Player.SimpleReadout, ctl.gameObject.transform, false);

      readout = readoutObj.GetComponent<SimpleReadout>();

      readout.Scale = 0.002f;
      readout.Size = new Vector2(100.0f, 30.0f);
      readout.Position = new Vector3(0.0f, 0.15f, -0.15f);

      readout.transform.localRotation = Quaternion.Euler(65.0f, 0.0f, 0.0f);

      grab = false; // Reset the state to avoid weird behavior

      UpdateReadout();
    }

    public void Disable(IHandController ctl) {
      ctl.Events.GripPressed -= OnGrabStart;
      ctl.Events.GripReleased -= OnGrabEnd;
      ctl.Other.Events.GripPressed -= OnDoubleGrabStart;
      ctl.Other.Events.GripReleased -= OnGrabEnd;
      ctl.OnFixedUpdate -= FixedUpdate;

      GameObject.Destroy(readoutObj);
    }

    void UpdateReadout() {
      string str = null;

      float scl = player.Controller.transform.localScale.z;

      if (scl > 1) {
        str = $"{scl:G4}:1";
      } else {
        str = $"1:{1 / scl:G4}";
      }

      readout.Text = str;
    }

    void OnGrabStart(object sender, ControllerInteractionEventArgs e) {
      // This makes the assumption both hands are in grab mode
      if (!otherEvents.gripPressed) {
        lastPos = events.transform.position;
        grab = true;
      }
    }

    void OnDoubleGrabStart(object sender, ControllerInteractionEventArgs e) {
      if (grab) lastDoubleDiff = DoubleDiff;
    }

    void OnGrabEnd(object sender, ControllerInteractionEventArgs e) => grab = false;

    // TODO: Maybe add a drag deadband?
    void FixedUpdate() {
      // NB: All delta values below are intentionally inverted.
      if (grab) {
        if (otherEvents.gripPressed) {
          var currDoubleDiff = DoubleDiff;

          {
            var fromVec = currDoubleDiff;
            var toVec = lastDoubleDiff;

            // for my own sanity and the sanity of those around me
            if (player.OrientLockY) fromVec.y = toVec.y = 0.0f;

            var quat = Quaternion.FromToRotation(fromVec, toVec);

            player.Controller.transform.rotation = quat * player.Controller.transform.rotation;
          }

          player.Controller.transform.localScale *= lastDoubleDiff.magnitude / currDoubleDiff.magnitude;

          lastDoubleDiff = DoubleDiff; // Recompute just in case
        }
        else {
          player.Controller.Move(lastPos - events.transform.position);
          lastPos = events.transform.position; // Gotta get the new position after the player moved
        }
      }

      UpdateReadout();
    }
  }
}
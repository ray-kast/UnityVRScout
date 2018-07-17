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

      grab = false; // Reset the state to avoid weird behavior
    }

    public void Disable(IHandController ctl) {
      ctl.Events.GripPressed -= OnGrabStart;
      ctl.Events.GripReleased -= OnGrabEnd;
      ctl.Other.Events.GripPressed -= OnDoubleGrabStart;
      ctl.Other.Events.GripReleased -= OnGrabEnd;
      ctl.OnFixedUpdate -= FixedUpdate;
    }

    void OnGrabStart(object sender, ControllerInteractionEventArgs e) {
      // This makes the assumption both hands are in grab mode
      if (!otherEvents.gripPressed) {
        lastPos = events.transform.position;
        grab = true;
      }
    }

    void OnDoubleGrabStart(object sender, ControllerInteractionEventArgs e) {
      if (grab) {
        lastDoubleDiff = DoubleDiff;
      }
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
    }
  }
}
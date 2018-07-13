using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout.HandFuncs {
  public class OrientFunction : IHandFunction {
    Vector3 lastPos;
    VRTK_ControllerEvents events;
    IPlayerController player;
    bool grab = false;

    public OrientFunction() { }

    public void Enable(IHandController ctl) {
      events = ctl.Events;
      player = ctl.Player;
      ctl.Events.GripPressed += OnGrabStart;
      ctl.Events.GripReleased += OnGrabEnd;
      ctl.OnFixedUpdate += FixedUpdate;

      grab = false; // Just in case.
    }

    public void Disable(IHandController ctl) {
      ctl.Events.GripPressed -= OnGrabStart;
      ctl.Events.GripReleased -= OnGrabEnd;
      ctl.OnFixedUpdate -= FixedUpdate;
    }

    void OnGrabStart(object sender, ControllerInteractionEventArgs e) {
      lastPos = events.transform.position;
      grab = true;
    }

    void OnGrabEnd(object sender, ControllerInteractionEventArgs e) {
      grab = false;
    }

    void FixedUpdate() {
      if (!grab) return;

      // NB: This is intentionally inverted
      player.Controller.Move(lastPos - events.transform.position);
      lastPos = events.transform.position; // Gotta get the new position after the player moved
      // TODO: Alternatively, would it make sense to use .localPosition?

      // TODO: Also set up rotate and scale (or should that go somewhere else?)
    }
  }
}
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout.HandFuncs {
  public class GrabFunction : IHandFunction {
    VRTK_InteractTouch touch;
    VRTK_InteractGrab grab;

    public GrabFunction() { }

    public void Enable(IHandController ctl) {
      if (touch != null) GameObject.Destroy(touch);
      if (grab != null) GameObject.Destroy(grab);

      touch = ctl.gameObject.AddComponent<VRTK_InteractTouch>();
      grab = ctl.gameObject.AddComponent<VRTK_InteractGrab>();

      grab.controllerEvents = ctl.Events;
      grab.interactTouch = touch;
    }

    public void Disable(IHandController ctl) {
      GameObject.Destroy(touch);
      GameObject.Destroy(grab);
    }
  }
}
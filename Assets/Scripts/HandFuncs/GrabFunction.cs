using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class GrabFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.GripTooltip] = "Grab",
    };

    VRTK_InteractTouch touch;
    VRTK_InteractGrab grab;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

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
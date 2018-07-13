using System.Collections.Generic;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class PointFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Pointer",
    };

    VRTK_Pointer point;
    VRTK_StraightPointerRenderer renderer;

    public Dictionary<TooltipButtons, string> Tooltips => tooltips;

    public PointFunction() { }

    public void Enable(IHandController ctl) {
      if (point != null) GameObject.Destroy(point);
      if (renderer != null) GameObject.Destroy(renderer);

      point = ctl.gameObject.AddComponent<VRTK_Pointer>();
      renderer = ctl.gameObject.AddComponent<VRTK_StraightPointerRenderer>();

      point.pointerRenderer = renderer;

      point.activationButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
    }

    public void Disable(IHandController ctl) {
      GameObject.Destroy(point.customOrigin.gameObject);
      GameObject.Destroy(point);
      GameObject.Destroy(renderer);
    }
  }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    VRTK_CustomRaycast raycast;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public PointFunction() { }

    public void Enable(IHandController ctl) {
      if (point != null) GameObject.Destroy(point);
      if (renderer != null) GameObject.Destroy(renderer);
      if (raycast != null) GameObject.Destroy(raycast);

      point = ctl.gameObject.AddComponent<VRTK_Pointer>();
      renderer = ctl.gameObject.AddComponent<VRTK_StraightPointerRenderer>();
      raycast = ctl.gameObject.AddComponent<VRTK_CustomRaycast>();

      point.pointerRenderer = renderer;
      renderer.customRaycast = raycast;

      point.activationButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
      raycast.layersToIgnore = ctl.Player.PointerIgnoreLayers;
    }

    public void Disable(IHandController ctl) {
      GameObject.Destroy(point.customOrigin.gameObject);
      GameObject.Destroy(point);
      GameObject.Destroy(renderer);
      GameObject.Destroy(raycast);
    }
  }
}
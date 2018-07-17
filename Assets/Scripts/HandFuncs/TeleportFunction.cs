using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class TeleportFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Teleport",
    };

    VRTK_Pointer point;
    VRTK_BezierPointerRenderer renderer;
    VRTK_CustomRaycast raycast;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public TeleportFunction() { }

    public void Enable(IHandController ctl) {
      if (point != null) GameObject.Destroy(point);
      if (renderer != null) GameObject.Destroy(renderer);
      if (raycast != null) GameObject.Destroy(raycast);

      point = ctl.gameObject.AddComponent<VRTK_Pointer>();
      renderer = ctl.gameObject.AddComponent<VRTK_BezierPointerRenderer>();
      raycast = ctl.gameObject.AddComponent<VRTK_CustomRaycast>();

      point.pointerRenderer = renderer;
      renderer.customRaycast = raycast;

      point.enableTeleport = true;
      point.activationButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
      point.selectionButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
      point.selectOnPress = false;
      raycast.layersToIgnore = ctl.Player.PointerIgnoreLayers;

      ctl.Player.Teleport.InitDestinationSetListener(ctl.gameObject, true);
    }

    public void Disable(IHandController ctl) {
      ctl.Player.Teleport.InitDestinationSetListener(ctl.gameObject, false);

      GameObject.Destroy(point.customOrigin.gameObject);
      GameObject.Destroy(point);
      GameObject.Destroy(renderer);
      GameObject.Destroy(raycast);
    }
  }
}
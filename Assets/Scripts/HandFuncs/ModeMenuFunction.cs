using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class ModeMenuFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.ButtonTwoTooltip] = "Back",
      [TooltipButtons.TriggerTooltip] = "Select",
    };

    IHandModeController modeCtl;
    VRTK_Pointer point;
    VRTK_StraightPointerRenderer renderer;
    VRTK_CustomRaycast raycast;
    VRTK_PolicyList policyList;
    GameObject menuObj;
    ModeMenu menu;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public ModeMenuFunction(IHandModeController _modeCtl) {
      modeCtl = _modeCtl;
    }

    public void Enable(IHandController ctl) {
      ctl.Events.ButtonTwoPressed += OnMenu;

      if (point != null) GameObject.Destroy(point);
      if (renderer != null) GameObject.Destroy(renderer);
      if (raycast != null) GameObject.Destroy(raycast);
      if (policyList != null) GameObject.Destroy(policyList);
      if (menuObj != null) GameObject.Destroy(menuObj);
      // No need to destroy menu, as it should have been attached to menuObj
      // (and if it wasn't then we have bigger problems to worry about)

      point = ctl.gameObject.AddComponent<VRTK_Pointer>();
      renderer = ctl.gameObject.AddComponent<VRTK_StraightPointerRenderer>();
      raycast = ctl.gameObject.AddComponent<VRTK_CustomRaycast>();
      policyList = ctl.gameObject.AddComponent<VRTK_PolicyList>();
      menuObj = new GameObject("ModeMenu");
      menu = menuObj.AddComponent<ModeMenu>();

      point.pointerRenderer = renderer;
      point.targetListPolicy = policyList;
      renderer.customRaycast = raycast;

      point.enableTeleport = false;
      point.activationButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
      point.selectionButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
      point.selectOnPress = false;
      raycast.layersToIgnore = ctl.Player.PointerIgnoreLayers;
      policyList.checkType = VRTK_PolicyList.CheckTypes.Script;
      policyList.operation = VRTK_PolicyList.OperationTypes.Include;
      policyList.identifiers = new List<string> { nameof(ModeMenuItem) };

      point.DestinationMarkerEnter += (sender, e) => Debug.Log(e.target);
      point.DestinationMarkerSet += (sender, e) => {
        var item = e.target.GetComponent<ModeMenuItem>();

        if (item != null) {
          switch (item.type) {
            case ModeMenuItem.ModeType.Primary: modeCtl.SelectPrimary(item.index); break;
            case ModeMenuItem.ModeType.Grip: modeCtl.SelectGrip(item.index); break;
          }

          modeCtl.EndMenu();
        }
      };

      menuObj.transform.position = ctl.gameObject.transform.position;
      // The ternary ensures the menu's not flipped if you're pointing the controller backwards
      menuObj.transform.rotation = Quaternion.Euler(
        0.0f,
        ctl.gameObject.transform.eulerAngles.y + (ctl.gameObject.transform.up.y < 0.0f ? 180.0f : 0.0f),
        0.0f);
      menuObj.transform.localScale = ctl.gameObject.transform.lossyScale;

      menu.Init(ctl.Player, modeCtl);
    }

    public void Disable(IHandController ctl) {
      ctl.Events.ButtonTwoPressed -= OnMenu;

      GameObject.Destroy(point.customOrigin.gameObject);
      GameObject.Destroy(point);
      GameObject.Destroy(renderer);
      GameObject.Destroy(raycast);
      GameObject.Destroy(policyList);
      GameObject.Destroy(menuObj);
    }

    void OnMenu(object sender, ControllerInteractionEventArgs e) => modeCtl.EndMenu();
  }
}
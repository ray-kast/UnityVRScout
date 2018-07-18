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
    };

    IHandModeController modeCtl;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public ModeMenuFunction(IHandModeController _modeCtl) {
      modeCtl = _modeCtl;
    }

    public void Enable(IHandController ctl) {
      ctl.Events.ButtonTwoPressed += OnMenu;
    }

    public void Disable(IHandController ctl) {
      ctl.Events.ButtonTwoPressed -= OnMenu;
    }

    void OnMenu(object sender, ControllerInteractionEventArgs e) => modeCtl.EndMenu();
  }
}
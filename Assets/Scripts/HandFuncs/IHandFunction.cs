using System;
using System.Collections.Generic;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public interface IHandFunction {
    Dictionary<TooltipButtons, string> Tooltips { get; } // TODO: This shouldn't be mutable

    void Enable(IHandController ctl);
    void Disable(IHandController ctl);
  }
}
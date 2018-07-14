using System;
using System.Collections.Generic;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public interface IHandFunction {
    Dictionary<TooltipButtons, string> Tooltips { get; }

    void Enable(IHandController ctl);
    void Disable(IHandController ctl);
  }
}
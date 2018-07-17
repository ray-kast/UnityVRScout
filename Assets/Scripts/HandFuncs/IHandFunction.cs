using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public interface IHandFunction {
    ReadOnlyDictionary<TooltipButtons, string> Tooltips { get; }

    void Enable(IHandController ctl);
    void Disable(IHandController ctl);
  }
}
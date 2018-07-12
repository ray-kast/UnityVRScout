using System;
using System.Collections.Generic;
using VRTK;

namespace VRScout.HandFuncs {
  public interface IHandFunction {
    void Enable(IHandController ctl);
    void Disable(IHandController ctl);
  }
}
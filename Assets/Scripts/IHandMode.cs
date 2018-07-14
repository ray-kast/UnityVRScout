using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRScout {
  public interface IHandMode {
    void Enable(IHandModeManager man);

    void Disable(IHandModeManager man);
  }
}
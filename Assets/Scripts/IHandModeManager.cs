using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRScout {
  public interface IHandModeManager {
    void EnableFunc(Type func);
    void DisableFunc(Type func);
  }
}
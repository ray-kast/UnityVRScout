using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  public interface IHandController {
    VRTK_ControllerEvents Events { get; }
    GameObject gameObject { get; }
  }
}
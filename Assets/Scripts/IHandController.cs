using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  public interface IHandController {
    GameObject gameObject { get; }
    IHandController Other { get; }
    IPlayerController Player { get; }
    VRTK_ControllerEvents Events { get; }

    event Action OnFixedUpdate;
  }
}
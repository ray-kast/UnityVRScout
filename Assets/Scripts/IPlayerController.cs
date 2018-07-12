using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRScout {
  public interface IPlayerController {
    CharacterController Controller { get; }

    float MaxFlySpeed { get; }
    float FlyDeadband { get; }
    float FlySensitivity { get; }
  }
}
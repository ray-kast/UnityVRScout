using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  [RequireComponent(typeof(CharacterController))]
  public class PlayerController : MonoBehaviour, IPlayerController {
    CharacterController controller;

    public float maxFlySpeed = 10.0f;
    public float flyDeadband = 0.1f;
    public float flySensitivity = 0.75f;

    CharacterController IPlayerController.Controller => controller;
    float IPlayerController.MaxFlySpeed => maxFlySpeed;
    float IPlayerController.FlyDeadband => flyDeadband;
    float IPlayerController.FlySensitivity => flySensitivity;

    void Awake() {
      controller = GetComponent<CharacterController>();
    }
  }
}

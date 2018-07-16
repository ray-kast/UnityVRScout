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

    [Tooltip("Lock the Y axis for rotating the world with the grip buttons.\nWARNING: Disable at your own risk!")]
    public bool orientLockY = true;
    public LayerMask pointerIgnoreLayers;
    public GameObject camViewfinder;

    CharacterController IPlayerController.Controller => controller;
    float IPlayerController.MaxFlySpeed => maxFlySpeed;
    float IPlayerController.FlyDeadband => flyDeadband;
    float IPlayerController.FlySensitivity => flySensitivity;
    bool IPlayerController.OrientLockY => orientLockY;
    LayerMask IPlayerController.PointerIgnoreLayers => pointerIgnoreLayers;
    GameObject IPlayerController.CamViewfinder => camViewfinder;

    void Awake() {
      controller = GetComponent<CharacterController>();
    }
  }
}

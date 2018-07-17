using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  [RequireComponent(typeof(CharacterController))]
  public class PlayerController : MonoBehaviour, IPlayerController {
    CharacterController controller;

    public float camFilmSize = 35.0f;
    public int camJpegQuality = 75;
    public float maxFlySpeed = 10.0f;
    public float flyDeadband = 0.1f;
    public float flySensitivity = 0.75f;

    [Tooltip("Lock the Y axis for rotating the world with the grip buttons.\nWARNING: Disable at your own risk!")]
    public bool orientLockY = true;
    public LayerMask pointerIgnoreLayers;
    public VRTK_HeightAdjustTeleport teleport;

    public GameObject camViewfinder;

    CharacterController IPlayerController.Controller => controller;
    float IPlayerController.CamFilmSize => camFilmSize;
    int IPlayerController.CamJpegQuality => camJpegQuality;
    float IPlayerController.MaxFlySpeed => maxFlySpeed;
    float IPlayerController.FlyDeadband => flyDeadband;
    float IPlayerController.FlySensitivity => flySensitivity;
    bool IPlayerController.OrientLockY => orientLockY;
    LayerMask IPlayerController.PointerIgnoreLayers => pointerIgnoreLayers;
    VRTK_HeightAdjustTeleport IPlayerController.Teleport => teleport;

    GameObject IPlayerController.CamViewfinder => camViewfinder;

    void Awake() {
      controller = GetComponent<CharacterController>();
    }
  }
}

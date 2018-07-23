using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  [RequireComponent(typeof(CharacterController))]
  public class PlayerController : MonoBehaviour, IPlayerController {
    CharacterController controller;

    public float camFilmSize = 35.0f;
    public float camPitchOffset = 15.0f;
    public int camJpegQuality = 75;
    public float maxFlySpeed = 10.0f;
    public float flyDeadband = 0.1f;
    public float flySensitivity = 0.75f;

    [Tooltip("Lock the Y axis for rotating the world with the grip buttons.\nWARNING: Disable at your own risk!")]
    public bool orientLockY = true;
    public LayerMask pointerIgnoreLayers;
    public VRTK_HeightAdjustTeleport teleport;

    public GameObject modeMenuItem;
    public GameObject snapshotCam;
    public GameObject camViewfinder;
    public GameObject measureLine;
    public GameObject measureReadout;

    CharacterController IPlayerController.Controller => controller;
    float IPlayerController.CamFilmSize => camFilmSize;
    float IPlayerController.CamPitchOffset => camPitchOffset;
    int IPlayerController.CamJpegQuality => camJpegQuality;
    float IPlayerController.MaxFlySpeed => maxFlySpeed;
    float IPlayerController.FlyDeadband => flyDeadband;
    float IPlayerController.FlySensitivity => flySensitivity;
    bool IPlayerController.OrientLockY => orientLockY;
    LayerMask IPlayerController.PointerIgnoreLayers => pointerIgnoreLayers;
    VRTK_HeightAdjustTeleport IPlayerController.Teleport => teleport;

    GameObject IPlayerController.ModeMenuItem => modeMenuItem;
    GameObject IPlayerController.SnapshotCam => snapshotCam;
    GameObject IPlayerController.CamViewfinder => camViewfinder;
    GameObject IPlayerController.MeasureLine => measureLine;
    GameObject IPlayerController.MeasureReadout => measureReadout;

    void Awake() {
      controller = GetComponent<CharacterController>();
    }
  }
}

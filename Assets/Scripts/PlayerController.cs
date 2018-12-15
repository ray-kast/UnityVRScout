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

    [Tooltip("The ratio between one Unity unit and one meter")]
    public float measureConvertRatio = 1.0f;
    public HandFuncs.MeasureUnits measureUnits = HandFuncs.MeasureUnits.Metric;

    [Tooltip("Lock the Y axis for rotating the world with the grip buttons.\nWARNING: Disable at your own risk!")]
    public bool orientLockY = true;
    public LayerMask pointerIgnoreLayers;
    public VRTK_HeightAdjustTeleport teleport;

    public GameObject modeMenuItem;
    public GameObject snapshotCam;
    public GameObject cameraHud;
    public GameObject measureLine;
    public GameObject measureReadout;

    CharacterController IPlayerController.Controller => controller;
    float IPlayerController.CamFilmSize => camFilmSize;
    float IPlayerController.CamPitchOffset => camPitchOffset;
    int IPlayerController.CamJpegQuality => camJpegQuality;
    float IPlayerController.MaxFlySpeed => maxFlySpeed;
    float IPlayerController.FlyDeadband => flyDeadband;
    float IPlayerController.FlySensitivity => flySensitivity;
    float IPlayerController.MeasureConvertRatio => measureConvertRatio;
    HandFuncs.MeasureUnits IPlayerController.MeasureUnits => measureUnits;
    bool IPlayerController.OrientLockY => orientLockY;
    LayerMask IPlayerController.PointerIgnoreLayers => pointerIgnoreLayers;
    VRTK_HeightAdjustTeleport IPlayerController.Teleport => teleport;

    GameObject IPlayerController.ModeMenuItem => modeMenuItem;
    GameObject IPlayerController.SnapshotCam => snapshotCam;
    GameObject IPlayerController.CameraHud => cameraHud;
    GameObject IPlayerController.MeasureLine => measureLine;
    GameObject IPlayerController.MeasureReadout => measureReadout;

    void Awake() {
      controller = GetComponent<CharacterController>();
    }
  }
}

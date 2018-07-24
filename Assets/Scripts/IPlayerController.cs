using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  public interface IPlayerController {
    CharacterController Controller { get; }

    float CamFilmSize { get; }
    float CamPitchOffset { get; }
    int CamJpegQuality { get; }
    float MaxFlySpeed { get; }
    float FlyDeadband { get; }
    float FlySensitivity { get; }
    float MeasureConvertRatio { get; }
    HandFuncs.MeasureUnits MeasureUnits { get; }
    bool OrientLockY { get; }
    LayerMask PointerIgnoreLayers { get; }
    VRTK_HeightAdjustTeleport Teleport { get; }

    GameObject ModeMenuItem { get; }
    GameObject SnapshotCam { get; }
    GameObject CamViewfinder { get; }
    GameObject MeasureLine { get; }
    GameObject MeasureReadout { get; }
  }
}
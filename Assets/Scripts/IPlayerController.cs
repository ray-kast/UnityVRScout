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
    bool OrientLockY { get; }
    LayerMask PointerIgnoreLayers { get; }
    VRTK_HeightAdjustTeleport Teleport { get; }

    GameObject SnapshotCam { get; }
    GameObject CamViewfinder { get; }
  }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

namespace VRScout {
  public interface IHandModeController {
    ReadOnlyCollection<SimpleHandMode> PrimaryModes { get; }

    ReadOnlyCollection<SimpleHandMode> GripModes { get; }

    int CurrPrimaryMode { get; }

    int CurrGripMode { get; }

    void BeginMenu();

    void EndMenu();

    void SelectPrimary(int mode);

    void SelectGrip(int mode);
  }
}
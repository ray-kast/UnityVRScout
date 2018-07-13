using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout {
  public struct CompoundHandMode : IHandMode {
    List<IHandMode> modes; // TODO: maybe make this public and make it observable?

    public CompoundHandMode(IEnumerable<IHandMode> _modes) {
      modes = new List<IHandMode>(_modes);
    }

    public void Enable(IHandModeManager man) {
      foreach (var mode in modes) mode.Enable(man);
    }

    public void Disable(IHandModeManager man) {
      foreach (var mode in modes) mode.Disable(man);
    }
  }
}
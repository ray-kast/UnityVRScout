using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout {
  public struct CompoundHandMode : IHandMode {
    List<IHandMode> modes; // TODO: maybe make this public and make it observable?

    public HashSet<Type> FuncTypes => new HashSet<Type>(Enumerable.SelectMany(modes, m => m.FuncTypes));

    public CompoundHandMode(IEnumerable<IHandMode> _modes) {
      modes = new List<IHandMode>(_modes);
    }
  }
}
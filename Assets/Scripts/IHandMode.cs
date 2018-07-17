using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRScout {
  public interface IHandMode {
    HashSet<Type> FuncTypes { get; }
  }
}
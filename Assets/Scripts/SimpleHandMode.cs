using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout {
  public struct SimpleHandMode : IHandMode {
    List<Type> funcs;

    string name { get; }

    public SimpleHandMode(string _name, IEnumerable<Type> _funcs) {
      name = _name;
      funcs = new List<Type>(_funcs);
    }

    public void Enable(IHandModeManager man) {
      foreach (var func in funcs) man.EnableFunc(func);
    }

    public void Disable(IHandModeManager man) {
      foreach (var func in funcs) man.DisableFunc(func);
    }
  }
}
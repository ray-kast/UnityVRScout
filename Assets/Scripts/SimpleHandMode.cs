using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout {
  public struct SimpleHandMode : IHandMode {
    List<Type> funcs;

    string name { get; }

    public HashSet<Type> FuncTypes => new HashSet<Type>(funcs);

    public SimpleHandMode(string _name, IEnumerable<Type> _funcs) {
      name = _name;
      funcs = new List<Type>(_funcs);
    }
  }
}
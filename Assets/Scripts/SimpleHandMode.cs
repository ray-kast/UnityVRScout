using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout {
  public struct SimpleHandMode : IHandMode {
    public static SimpleHandMode Disabled { get; } = new SimpleHandMode("<disabled>", new Type[0]);

    List<Type> funcs;

    public string Name { get; }

    public HashSet<Type> FuncTypes => new HashSet<Type>(funcs);

    public SimpleHandMode(string name, IEnumerable<Type> _funcs) {
      Name = name;
      funcs = new List<Type>(_funcs);
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRScout {
  [RequireComponent(typeof(VRTK_ControllerEvents))]
  public class HandController : MonoBehaviour, IHandController {
    List<HandFuncs.IHandFunction> funcs;
    HashSet<HandFuncs.IHandFunction> activeFuncs;
    VRTK_ControllerEvents events;
    int currFunc; // TODO: This is temporary

    VRTK_ControllerEvents IHandController.Events => events;

    void Awake() {
      funcs = new List<HandFuncs.IHandFunction>();
      activeFuncs = new HashSet<HandFuncs.IHandFunction>();
      events = GetComponent<VRTK_ControllerEvents>();
      currFunc = 0;

      funcs.Add(new HandFuncs.GrabFunction());
      funcs.Add(new HandFuncs.PointFunction());

      events.TouchpadPressed += (sender, e) => {
        DisableFunc(currFunc);
        currFunc = (currFunc + 1) % funcs.Count;
        EnableFunc(currFunc);
      };

      EnableFunc(currFunc);
    }

    void EnableFunc(int index) {
      if (activeFuncs.Add(funcs[index])) funcs[index].Enable(this);
    }

    void DisableFunc(int index) {
      if (activeFuncs.Remove(funcs[index])) funcs[index].Disable(this);
    }
  }
}

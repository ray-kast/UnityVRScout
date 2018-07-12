using System;
using System.Collections.Generic;
using UnityEngine;
using VRScout.HandFuncs;
using VRTK;

namespace VRScout {

  [RequireComponent(typeof(VRTK_ControllerEvents))]
  public class HandController : MonoBehaviour, IHandController, IHandModeManager {
    Dictionary<Type, IHandFunction> funcs;
    List<HandMode> modes;
    HashSet<IHandFunction> activeFuncs;
    VRTK_ControllerEvents events;
    int currMode; // TODO: This is temporary

    event Action onFixedUpdate;

    IPlayerController IHandController.Player => player;
    VRTK_ControllerEvents IHandController.Events => events;

    event Action IHandController.OnFixedUpdate {
      add { onFixedUpdate += value; }
      remove { onFixedUpdate -= value; }
    }

    public PlayerController player;

    void Awake() {
      // TODO: Is it worth it to try instantiating these at runtime with reflection?
      funcs = new Dictionary<Type, IHandFunction> {
        [typeof(FlyFunction)] = new FlyFunction(),
        [typeof(GrabFunction)] = new GrabFunction(),
        [typeof(PointFunction)] = new PointFunction(),
      };

      // TODO: This should probably be static.
      modes = new List<HandMode> {
        new HandMode("Fly", new[] { typeof(FlyFunction) }),
        new HandMode("Hand", new[] { typeof(GrabFunction), }),
        new HandMode("Pointer", new[] { typeof(GrabFunction), typeof(PointFunction) }),
      };

      activeFuncs = new HashSet<IHandFunction>();
      events = GetComponent<VRTK_ControllerEvents>();
      currMode = 0;

      // TODO: Make this some kind of menu.
      events.TouchpadPressed += (sender, e) => SetMode((currMode + 1) % modes.Count);

      modes[currMode].Enable(this);
    }

    void FixedUpdate() => onFixedUpdate?.Invoke();

    void IHandModeManager.EnableFunc(Type func) {
      if (activeFuncs.Add(funcs[func])) funcs[func].Enable(this);
    }

    void IHandModeManager.DisableFunc(Type func) {
      if (activeFuncs.Remove(funcs[func])) funcs[func].Disable(this);
    }

    void SetMode(int index) {
      modes[currMode].Disable(this);
      modes[currMode = index].Enable(this);
    }
  }
}

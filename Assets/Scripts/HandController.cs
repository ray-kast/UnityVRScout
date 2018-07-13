using System;
using System.Collections.Generic;
using UnityEngine;
using VRScout.HandFuncs;
using VRTK;

namespace VRScout {

  [RequireComponent(typeof(VRTK_ControllerEvents))]
  public class HandController : MonoBehaviour, IHandController, IHandModeManager {
    Dictionary<Type, IHandFunction> funcs;
    List<SimpleHandMode> primaryModes, gripModes;
    HashSet<IHandFunction> activeFuncs;
    VRTK_ControllerEvents events;
    IHandMode currMode;
    int currPrimaryMode, currGripMode; // TODO: currGripMode needs to be shared between both hands

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
        [typeof(OrientFunction)] = new OrientFunction(),
        [typeof(PointFunction)] = new PointFunction(),
      };

      // TODO: These both should probably be static.
      primaryModes = new List<SimpleHandMode> {
        new SimpleHandMode("None", new Type[0]),
        new SimpleHandMode("Fly", new[] { typeof(FlyFunction) }),
        new SimpleHandMode("Pointer", new[] { typeof(PointFunction) }),
      };

      gripModes = new List<SimpleHandMode> {
        new SimpleHandMode("Grab", new[] { typeof(GrabFunction), }),
        new SimpleHandMode("Orient", new[] { typeof(OrientFunction) }),
      };

      activeFuncs = new HashSet<IHandFunction>();
      events = GetComponent<VRTK_ControllerEvents>();
      currMode = null;
      currPrimaryMode = currGripMode = 0;

      // TODO: Make this some kind of menu.
      events.TouchpadPressed += (sender, e) => SetToolMode((currPrimaryMode + 1) % primaryModes.Count, currGripMode);
      events.ButtonOnePressed += (sender, e) => SetToolMode(currPrimaryMode, (currGripMode + 1) % gripModes.Count);

      SetToolMode(currPrimaryMode, currGripMode);
    }

    void FixedUpdate() => onFixedUpdate?.Invoke();

    void IHandModeManager.EnableFunc(Type func) {
      if (activeFuncs.Add(funcs[func])) funcs[func].Enable(this);
    }

    void IHandModeManager.DisableFunc(Type func) {
      if (activeFuncs.Remove(funcs[func])) funcs[func].Disable(this);
    }

    void SetMode(IHandMode mode) {
      currMode?.Disable(this);
      currMode = mode;
      currMode.Enable(this);
    }

    void SetToolMode(int primary, int grip)
      => SetMode(new CompoundHandMode(new IHandMode[] {
        primaryModes[currPrimaryMode = primary],
        gripModes[currGripMode = grip],
      }));
  }
}

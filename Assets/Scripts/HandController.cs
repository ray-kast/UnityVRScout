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
    VRTK_ControllerTooltips tooltips;
    IHandMode currMode;
    int currPrimaryMode, currGripMode; // TODO: currGripMode needs to be shared between both hands

    public HandController other;

    event Action onFixedUpdate;

    IHandController IHandController.Other => other;
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
      tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
      currMode = null;
      currPrimaryMode = currGripMode = 0;

      tooltips.SendMessage("Awake"); // This is super dumb but it prevents an exception from being thrown

      // TODO: Make this some kind of menu.
      events.TouchpadPressed += (sender, e) => SetToolMode((currPrimaryMode + 1) % primaryModes.Count, currGripMode);
      events.ButtonOnePressed += (sender, e) => SetToolMode(currPrimaryMode, (currGripMode + 1) % gripModes.Count);
      events.ButtonTwoPressed += (sender, e) => SetToolMode(currPrimaryMode, (currGripMode + 1) % gripModes.Count);

      SetToolMode(currPrimaryMode, currGripMode);
    }

    void FixedUpdate() => onFixedUpdate?.Invoke();

    void IHandModeManager.EnableFunc(Type func) {
      var fnObj = funcs[func];

      if (activeFuncs.Add(fnObj)) fnObj.Enable(this);

      foreach (var pair in fnObj.Tooltips) tooltips.UpdateText(pair.Key, pair.Value);

      tooltips.ToggleTips(true);
    }

    void IHandModeManager.DisableFunc(Type func) {
      var fnObj = funcs[func];

      if (activeFuncs.Remove(fnObj)) fnObj.Disable(this);

      foreach (var pair in fnObj.Tooltips) tooltips.UpdateText(pair.Key, "");

      tooltips.ToggleTips(true);
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

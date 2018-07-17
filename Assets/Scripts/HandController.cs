using System;
using System.Collections.Generic;
using UnityEngine;
using VRScout.HandFuncs;
using VRTK;

namespace VRScout {

  [RequireComponent(typeof(VRTK_ControllerEvents))]
  public class HandController : MonoBehaviour, IHandController {
    Dictionary<Type, IHandFunction> funcs;
    List<SimpleHandMode> primaryModes, gripModes;
    HashSet<Type> activeFuncs;
    VRTK_ControllerEvents events;
    VRTK_ControllerTooltips tooltips;
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
        [typeof(CameraFunction)] = new CameraFunction(),
        [typeof(FlyFunction)] = new FlyFunction(),
        [typeof(GrabFunction)] = new GrabFunction(),
        [typeof(OrientFunction)] = new OrientFunction(),
        [typeof(PointFunction)] = new PointFunction(),
        [typeof(TeleportFunction)] = new TeleportFunction(),
      };

      // TODO: These both should probably be static.
      primaryModes = new List<SimpleHandMode> {
        new SimpleHandMode("None", new Type[0]),
        new SimpleHandMode("Fly", new[] { typeof(FlyFunction) }),
        new SimpleHandMode("Teleport", new[] { typeof(TeleportFunction) }),
        new SimpleHandMode("Pointer", new[] { typeof(PointFunction) }),
        new SimpleHandMode("Camera", new[] { typeof(CameraFunction) }),
      };

      gripModes = new List<SimpleHandMode> {
        new SimpleHandMode("Grab", new[] { typeof(GrabFunction), }),
        new SimpleHandMode("Orient", new[] { typeof(OrientFunction) }),
      };

      activeFuncs = new HashSet<Type>();
      events = GetComponent<VRTK_ControllerEvents>();
      tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
      currPrimaryMode = currGripMode = 0;

      tooltips.SendMessage("Awake"); // This is super dumb but it prevents an exception from being thrown

      // TODO: Make this some kind of menu.
      events.TouchpadPressed += (sender, e) => SetToolMode((currPrimaryMode + 1) % primaryModes.Count, currGripMode);
      events.ButtonOnePressed += (sender, e) => SetToolMode(currPrimaryMode, (currGripMode + 1) % gripModes.Count);
      events.ButtonTwoPressed += (sender, e) => SetToolMode(currPrimaryMode, (currGripMode + 1) % gripModes.Count);

      SetToolMode(currPrimaryMode, currGripMode);
    }

    void FixedUpdate() => onFixedUpdate?.Invoke();

    void SetMode(IHandMode mode) {

      var newActiveFuncs = mode.FuncTypes;

      VRTK_ControllerTooltips.TooltipButtons newTooltips = 0;

      foreach (var func in activeFuncs) {
        if (!newActiveFuncs.Contains(func)) {
          var fnObj = funcs[func];

          fnObj.Disable(this);

          foreach (var pair in fnObj.Tooltips) {
            tooltips.UpdateText(pair.Key, "");

            newTooltips |= pair.Key;
          }
        }
      }

      foreach (var func in newActiveFuncs) {
        if (!activeFuncs.Contains(func)) {
          var fnObj = funcs[func];

          fnObj.Enable(this);

          foreach (var pair in fnObj.Tooltips) {
            tooltips.UpdateText(pair.Key, pair.Value);

            newTooltips |= pair.Key;
          }
        }
      }

      tooltips.ToggleTips(true, newTooltips);

      activeFuncs = newActiveFuncs;
    }

    void SetToolMode(int primary, int grip)
      => SetMode(new CompoundHandMode(new IHandMode[] {
        primaryModes[currPrimaryMode = primary],
        gripModes[currGripMode = grip],
      }));
  }
}

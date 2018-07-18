using System;
using System.Collections.Generic;
using UnityEngine;
using VRScout.HandFuncs;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout {

  [RequireComponent(typeof(VRTK_ControllerEvents))]
  public class HandController : MonoBehaviour, IHandController, IHandModeController {
    static readonly List<SimpleHandMode> primaryModes = new List<SimpleHandMode> {
      new SimpleHandMode("None", new Type[0]),
      new SimpleHandMode("Fly", new[] { typeof(FlyFunction) }),
      new SimpleHandMode("Teleport", new[] { typeof(TeleportFunction) }),
      new SimpleHandMode("Pointer", new[] { typeof(PointFunction) }),
      new SimpleHandMode("Camera", new[] { typeof(CameraFunction) }),
    };

    static readonly List<SimpleHandMode> gripModes = new List<SimpleHandMode> {
      new SimpleHandMode("Grab", new[] { typeof(GrabFunction), }),
      new SimpleHandMode("Orient", new[] { typeof(OrientFunction) }),
    };

    static readonly SimpleHandMode modeControl = new SimpleHandMode("Mode Control", new[] { typeof(ModeControlFunction) });

    static readonly SimpleHandMode modeMenu = new SimpleHandMode("Mode Menu", new[] { typeof(ModeMenuFunction) });

    Dictionary<Type, IHandFunction> funcs;
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
      // NB: This CANNOT be static! (Each function can operate on exactly one controller)
      funcs = new Dictionary<Type, IHandFunction> {
        [typeof(CameraFunction)] = new CameraFunction(),
        [typeof(FlyFunction)] = new FlyFunction(),
        [typeof(GrabFunction)] = new GrabFunction(),
        [typeof(ModeControlFunction)] = new ModeControlFunction(this),
        [typeof(ModeMenuFunction)] = new ModeMenuFunction(this),
        [typeof(OrientFunction)] = new OrientFunction(),
        [typeof(PointFunction)] = new PointFunction(),
        [typeof(TeleportFunction)] = new TeleportFunction(),
      };

      activeFuncs = new HashSet<Type>();
      events = GetComponent<VRTK_ControllerEvents>();
      tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
      currPrimaryMode = currGripMode = 0;

      tooltips.SendMessage("Awake"); // This is super dumb but it prevents an exception from being thrown

      SetEnabled();
    }

    void FixedUpdate() => onFixedUpdate?.Invoke();

    void SetMode(IHandMode mode) {

      var newActiveFuncs = mode.FuncTypes;

      TooltipButtons newTooltips = 0;

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

      // ...because apparently passing a mask to ToggleTips breaks it.
      foreach (TooltipButtons value in Enum.GetValues(typeof(TooltipButtons))) {
        if ((newTooltips & value) != 0) tooltips.ToggleTips(true, value);
      }

      activeFuncs = newActiveFuncs;
    }

    void SetToolMode(int primary, int grip)
      => SetMode(new CompoundHandMode(new IHandMode[] {
        modeControl,
        primaryModes[currPrimaryMode = primary],
        gripModes[currGripMode = grip],
      }));

    void SetEnabled() => SetToolMode(currPrimaryMode, currGripMode);

    // TODO, obviously
    void SetMenuMode() => SetMode(modeMenu);

    void SetDisabled() => SetMode(SimpleHandMode.Disabled);

    void IHandModeController.BeginMenu() {
      SetMenuMode();
      other.SetDisabled();
    }

    void IHandModeController.EndMenu() {
      SetEnabled();
      other.SetEnabled();
    }
  }
}

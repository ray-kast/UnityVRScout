using UnityEngine;

namespace VRScout.HandFuncs {
  public class ModeMenuItem : MonoBehaviour {
    public enum ModeType {
      Primary,
      Grip,
    }

    public int index;
    public ModeType type;
  }
}
using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout.HandFuncs {
  public class ModeMenu : MonoBehaviour {
    public void Init(IPlayerController player, IHandModeController modeCtl) {
      var primaryObj = new GameObject("PrimaryModes");
      var gripObj = new GameObject("GripModes");

      primaryObj.transform.parent = transform;
      gripObj.transform.parent = transform;

      primaryObj.transform.localPosition = Vector3.zero;
      primaryObj.transform.localRotation = Quaternion.identity;
      primaryObj.transform.localScale = Vector3.one;

      gripObj.transform.localPosition = Vector3.zero;
      gripObj.transform.localRotation = Quaternion.identity;
      gripObj.transform.localScale = Vector3.one;

      // Because these are generated values
      var primaryModes = modeCtl.PrimaryModes;
      var gripModes = modeCtl.GripModes;

      const float GAP_FACTOR = 0.02f;
      const float Z_GAP = 0.25f;

      var primOffs = -0.5f * (primaryModes.Count + GAP_FACTOR * (primaryModes.Count - 1));
      var gripOffs = -0.5f * (gripModes.Count + GAP_FACTOR * (gripModes.Count - 1));

      for (int i = 0; i < primaryModes.Count; ++i) {
        var mode = primaryModes[i];

        var itemObj = GameObject.Instantiate(player.ModeMenuItem, transform, false);
        var size = itemObj.GetComponent<MeshFilter>().mesh.bounds.size;
        size.Scale(itemObj.transform.localScale);
        var item = itemObj.GetComponent<ModeMenuItem>();

        itemObj.transform.localPosition = new Vector3((primOffs + i + i * GAP_FACTOR) * size.x, (1.0f + GAP_FACTOR) * 0.5f * size.z, Z_GAP);
        itemObj.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        item.type = ModeMenuItem.ModeType.Primary;
        item.index = i;
      }

      for (int i = 0; i < gripModes.Count; ++i) {
        var mode = gripModes[i];

        var itemObj = GameObject.Instantiate(player.ModeMenuItem, transform, false);
        var size = itemObj.GetComponent<MeshFilter>().mesh.bounds.size;
        size.Scale(itemObj.transform.localScale);
        var item = itemObj.GetComponent<ModeMenuItem>();

        itemObj.transform.localPosition = new Vector3((gripOffs + i + i * GAP_FACTOR) * size.x, ((1.0f + GAP_FACTOR) * 0.5f - (1.0f + GAP_FACTOR)) * size.z, Z_GAP);
        itemObj.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        item.type = ModeMenuItem.ModeType.Grip;
        item.index = i;
      }
    }
  }
}
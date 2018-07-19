using System.Collections.ObjectModel;
using UnityEngine;

namespace VRScout.HandFuncs {
  public class ModeMenu : MonoBehaviour {

    const float GAP_FACTOR = 0.02f;
    const float GAPPED_WIDTH = 1.0f + GAP_FACTOR;
    const float Z_GAP = 0.25f;

    void InstantiateItem(
        GameObject prefab,
        Vector2 pos,
        ModeMenuItem.ModeType type,
        int index,
        string text) {
      var itemObj = GameObject.Instantiate(prefab, transform, false);
      var item = itemObj.GetComponent<ModeMenuItem>();
      item.SetSize(new Vector2(100.0f, 30.0f), 0.5f);
      var size = item.canvas.GetComponent<RectTransform>().rect.size;
      size.Scale(itemObj.transform.localScale);

      pos.Scale(size);

      itemObj.transform.localPosition = new Vector3(pos.x, pos.y, Z_GAP);

      item.type = type;
      item.index = index;

      item.SetText(text);
    }

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

      var primOffs = -0.5f * GAPPED_WIDTH * (primaryModes.Count - 1);
      var gripOffs = -0.5f * GAPPED_WIDTH * (gripModes.Count - 1);

      for (int i = 0; i < primaryModes.Count; ++i) {
        var mode = primaryModes[i];

        InstantiateItem(
          player.ModeMenuItem,
          new Vector2(primOffs + GAPPED_WIDTH * i, GAPPED_WIDTH * 0.5f),
          ModeMenuItem.ModeType.Primary, i, mode.Name);
      }

      for (int i = 0; i < gripModes.Count; ++i) {
        var mode = gripModes[i];

        InstantiateItem(
          player.ModeMenuItem,
          new Vector2(gripOffs + GAPPED_WIDTH * i, GAPPED_WIDTH * 0.5f - GAPPED_WIDTH),
          ModeMenuItem.ModeType.Grip, i, mode.Name);
      }
    }
  }
}
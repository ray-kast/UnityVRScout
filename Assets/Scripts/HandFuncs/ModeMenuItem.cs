using UnityEngine;

using UI = UnityEngine.UI;

namespace VRScout.HandFuncs {
  public class ModeMenuItem : MonoBehaviour {
    public enum ModeType {
      Primary,
      Grip,
    }

    public int index;
    public ModeType type;

    public Canvas canvas;
    public UI.Image background;
    public UI.Text text;

    public void SetSize(Vector2 size, float scale) {
      transform.localScale = Vector3.one * 0.01f * scale;
      GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 1.0f);
      canvas.GetComponent<RectTransform>().sizeDelta = size;
      background.GetComponent<RectTransform>().sizeDelta = size;
      text.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetText(string value) {
      text.text = value;
    }
  }
}
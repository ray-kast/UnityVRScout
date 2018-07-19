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

    public Vector3 Position {
      set {
        value.Scale(transform.localScale);
        transform.localPosition = value;
      }
    }

    public float Scale {
      set {
        transform.localScale = Vector3.one * value;
      }
    }

    public Vector2 Size {
      get { return canvas.GetComponent<RectTransform>().rect.size; }
      set {
        GetComponent<BoxCollider>().size = new Vector3(value.x, value.y, 0.01f / transform.localScale.z);
        canvas.GetComponent<RectTransform>().sizeDelta = value;
        background.GetComponent<RectTransform>().sizeDelta = value;
        text.GetComponent<RectTransform>().sizeDelta = value;
      }
    }

    public void SetText(string value) {
      text.text = value;
    }

    public void SetSelected() {
      background.color = new Color(0.1f, 0.5f, 1.0f, 1.0f);
    }
  }
}
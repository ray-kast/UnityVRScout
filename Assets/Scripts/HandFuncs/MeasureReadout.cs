using UnityEngine;

using UI = UnityEngine.UI;

namespace VRScout.HandFuncs {
  // TODO: This and ModeMenuItem are slightly redundant
  public class MeasureReadout : MonoBehaviour {
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
      set { transform.localScale = new Vector3(value, value, 1.0f); }
    }

    public Vector2 Size {
      get { return canvas.GetComponent<RectTransform>().rect.size; }
      set {
        canvas.GetComponent<RectTransform>().sizeDelta = value;
        background.GetComponent<RectTransform>().sizeDelta = value;
        text.GetComponent<RectTransform>().sizeDelta = value;
      }
    }

    public string Text {
      get { return text.text; }
      set { text.text = value; }
    }
  }
}
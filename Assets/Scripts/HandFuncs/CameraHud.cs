using UnityEngine;

using UI = UnityEngine.UI;

namespace VRScout.HandFuncs {
  // TODO: This and ModeMenuItem are slightly redundant
  public class CameraHud : MonoBehaviour {
    public Canvas canvas;
    public UI.Image background;
    public UI.Text focalLen;
    public MeshRenderer viewfinder;

    public Vector3 CanvasPos {
      set {
        value.Scale(canvas.transform.localScale);
        canvas.transform.localPosition = value;
      }
    }

    public float CanvasScale {
      set { canvas.transform.localScale = new Vector3(value, value, 1.0f); }
    }

    public Vector2 CanvasSize {
      get { return canvas.GetComponent<RectTransform>().rect.size; }
      set {
        // NOTE: this will become problematic if the canvas contains anything
        //       other than the focal length
        canvas.GetComponent<RectTransform>().sizeDelta = value;
        background.GetComponent<RectTransform>().sizeDelta = value;
        focalLen.GetComponent<RectTransform>().sizeDelta = value;
      }
    }

    public string FocalLen {
      get { return focalLen.text; }
      set { focalLen.text = value; }
    }
  }
}
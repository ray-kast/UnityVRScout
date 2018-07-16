using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class CameraFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Record",
    };

    VRTK_ControllerEvents events;
    Camera cam;
    RenderTexture viewfinderTex;
    GameObject viewfinder;

    public Dictionary<TooltipButtons, string> Tooltips => tooltips;

    public CameraFunction() { }

    public void Enable(IHandController ctl) {
      if (cam != null) GameObject.Destroy(cam);

      events = ctl.Events;
      ctl.Events.TriggerPressed += OnRecord;

      cam = ctl.gameObject.AddComponent<Camera>();

      const int WIDTH = 1200;
      const int HEIGHT = 800;

      viewfinderTex = new RenderTexture(
        new RenderTextureDescriptor(WIDTH, HEIGHT, RenderTextureFormat.ARGB32));

      viewfinder = GameObject.Instantiate(ctl.Player.CamViewfinder, ctl.gameObject.transform, false);

      if (!viewfinderTex.Create())
        Debug.LogError("Failed to create render target for CameraFunction");

      cam.targetTexture = viewfinderTex;

      // TODO: Probably shouldn't hard-code these
      viewfinder.transform.localPosition = new Vector3(0.0f, 0.1f, 0.1f);
      viewfinder.transform.localRotation = Quaternion.Euler(-45.0f, 0.0f, 0.0f);

      {
        var scl = viewfinder.transform.localScale;

        if (WIDTH > HEIGHT) scl.z *= (float)HEIGHT / WIDTH;
        else scl.x *= (float)WIDTH / HEIGHT;

        viewfinder.transform.localScale = scl;
      }

      {
        var renderer = viewfinder.GetComponent<MeshRenderer>();
        var mat = renderer.material;
        mat.SetTexture("_MainTex", viewfinderTex);
      }
    }

    public void Disable(IHandController ctl) {
      ctl.Events.TriggerPressed -= OnRecord;

      GameObject.Destroy(cam);
      GameObject.Destroy(viewfinderTex);
      GameObject.Destroy(viewfinder);
    }

    void OnRecord(object sender, ControllerInteractionEventArgs e) {
      try {
        const int WIDTH = 6000;
        const int HEIGHT = 4000;

        var rtex = new RenderTexture(
          new RenderTextureDescriptor(WIDTH, HEIGHT, RenderTextureFormat.ARGB32));

        cam.targetTexture = rtex;
        cam.Render();

        var tex = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);

        RenderTexture.active = rtex;

        tex.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);

        Directory.CreateDirectory("Snapshots");

        File.WriteAllBytes(Path.Combine("Snapshots", $"snapshot_{DateTime.Now:yyyy-MM-dd-hh-mm-ss-ff}.jpg"), tex.EncodeToJPG(75)); // TODO: Make the JPEG quality an option?
      }
      finally {
        cam.targetTexture = viewfinderTex;
      }
    }
  }
}
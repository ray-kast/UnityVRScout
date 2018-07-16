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
      [TooltipButtons.TouchpadTooltip] = "Zoom",
      [TooltipButtons.TriggerTooltip] = "Record",
    };

    const float MIN_FOCAL_LEN = 16.0f;
    const float MAX_FOCAL_LEN = 72.0f;
    const float LEN_ADJUST_MAX_SPEED = 10.0f;
    const float LEN_ADJUST_DEADBAND = 0.1f;

    float focalLenSpeed, focalLen;
    IPlayerController player;
    Camera cam;
    RenderTexture viewfinderTex;
    GameObject viewfinder;

    public Dictionary<TooltipButtons, string> Tooltips => tooltips;

    public CameraFunction() { }

    public void Enable(IHandController ctl) {
      if (cam != null) GameObject.Destroy(cam);

      player = ctl.Player;
      ctl.Events.TouchpadAxisChanged += OnTouchpadMove;
      ctl.Events.TouchpadTouchStart += OnTouchpadTouchStart;
      ctl.Events.TouchpadTouchEnd += OnTouchpadTouchEnd;
      ctl.Events.TriggerPressed += OnRecord;
      ctl.OnFixedUpdate += FixedUpdate;

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

      focalLenSpeed = 0.0f;
      focalLen = MIN_FOCAL_LEN;
    }

    public void Disable(IHandController ctl) {
      ctl.Events.TouchpadAxisChanged -= OnTouchpadMove;
      ctl.Events.TouchpadTouchStart -= OnTouchpadTouchStart;
      ctl.Events.TouchpadTouchEnd -= OnTouchpadTouchEnd;
      ctl.Events.TriggerPressed -= OnRecord;
      ctl.OnFixedUpdate -= FixedUpdate;

      GameObject.Destroy(cam);
      GameObject.Destroy(viewfinderTex);
      GameObject.Destroy(viewfinder);
    }

    void OnTouchpadMove(object sender, ControllerInteractionEventArgs e) {
      var radius = Mathf.Clamp01((e.touchpadAxis.magnitude) - LEN_ADJUST_DEADBAND) / (1.0f - LEN_ADJUST_DEADBAND);

      if (radius > 1e-5f && Mathf.Abs(e.touchpadAxis.x) > Mathf.Abs(e.touchpadAxis.y))
        focalLenSpeed = Mathf.Sign(e.touchpadAxis.x) * radius;
      else focalLenSpeed = 0.0f;
    }

    void OnTouchpadTouchStart(object sender, ControllerInteractionEventArgs e) {
      OnTouchpadMove(sender, e);
    }

    void OnTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e) {
      focalLenSpeed = 0.0f;
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

    void FixedUpdate() {
      focalLen = Mathf.Clamp(focalLen + focalLenSpeed * LEN_ADJUST_MAX_SPEED * Time.fixedDeltaTime, MIN_FOCAL_LEN, MAX_FOCAL_LEN);
      if (Mathf.Abs(focalLenSpeed) > 1e-5f) Debug.Log(focalLen);
      cam.fieldOfView = 2.0f * Mathf.Rad2Deg * Mathf.Atan(0.5f * player.CamFilmSize / focalLen);
    }
  }
}
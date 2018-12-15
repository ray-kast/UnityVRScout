using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    const float MIN_FOCAL_LEN = 10.0f;
    const float MAX_FOCAL_LEN = 200.0f;
    const float LEN_ADJUST_MAX_SPEED = 30.0f;
    const float LEN_ADJUST_DEADBAND = 0.1f;

    float focalLenSpeed, focalLen = MIN_FOCAL_LEN;
    IPlayerController player;
    GameObject camObj, hudObj;
    CameraHud hud;
    Camera cam;
    RenderTexture viewfinderTex;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public CameraFunction() { }

    public void Enable(IHandController ctl) {
      if (camObj != null) GameObject.Destroy(cam);
      if (hudObj != null) GameObject.Destroy(hudObj);

      player = ctl.Player;
      ctl.Events.TouchpadAxisChanged += OnTouchpadMove;
      ctl.Events.TouchpadTouchStart += OnTouchpadTouchStart;
      ctl.Events.TouchpadTouchEnd += OnTouchpadTouchEnd;
      ctl.Events.TriggerPressed += OnRecord;
      ctl.OnFixedUpdate += FixedUpdate;

      camObj = GameObject.Instantiate(ctl.Player.SnapshotCam, ctl.gameObject.transform, false);
      hudObj = GameObject.Instantiate(ctl.Player.CameraHud, ctl.gameObject.transform, false);

      cam = camObj.GetComponent<Camera>();
      hud = hudObj.GetComponent<CameraHud>();

      cam.enabled = true;

      const int WIDTH = 1200;
      const int HEIGHT = 800;

      viewfinderTex = new RenderTexture(
        new RenderTextureDescriptor(WIDTH, HEIGHT, RenderTextureFormat.ARGB32));

      if (!viewfinderTex.Create())
        Debug.LogError("Failed to create viewfinder render target for CameraFunction");

      cam.targetTexture = viewfinderTex;

      // TODO: Probably shouldn't hard-code these
      camObj.transform.localRotation = Quaternion.Euler(ctl.Player.CamPitchOffset, 0.0f, 0.0f);

      hud.CanvasScale = 0.002f;
      hud.CanvasPos = new Vector3(0.0f, 0.0f, -0.001f);
      hud.CanvasSize = new Vector2(100.0f, 30.0f);

      hud.transform.localRotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);

      hud.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f) +
        hud.transform.localRotation * new Vector3(0.0f, 0.1f, 0.0f);

      // hud.viewfinder.transform.localPosition = new Vector3(0.0f, 0.1f, 0.1f);
      // hud.viewfinder.transform.localRotation = Quaternion.Euler(-45.0f, 0.0f, 0.0f);

      {
        var scl = hud.viewfinder.transform.localScale;

        if (WIDTH > HEIGHT) scl.z *= (float)HEIGHT / WIDTH;
        else scl.x *= (float)WIDTH / HEIGHT;

        hud.viewfinder.transform.localScale = scl;

        hud.canvas.transform.localPosition += new Vector3(
          0.0f,
          5.0f * scl.z,
          0.0f
        );
      }

      {
        var mat = hud.viewfinder.material;
        mat.SetTexture("_MainTex", viewfinderTex);
      }

      focalLenSpeed = 0.0f;

      UpdateFocalLen();
    }

    public void Disable(IHandController ctl) {
      ctl.Events.TouchpadAxisChanged -= OnTouchpadMove;
      ctl.Events.TouchpadTouchStart -= OnTouchpadTouchStart;
      ctl.Events.TouchpadTouchEnd -= OnTouchpadTouchEnd;
      ctl.Events.TriggerPressed -= OnRecord;
      ctl.OnFixedUpdate -= FixedUpdate;

      GameObject.Destroy(camObj);
      GameObject.Destroy(hudObj);
      GameObject.Destroy(viewfinderTex);
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

        if (!rtex.Create())
          Debug.LogError("Failed to create render target for CameraFunction");

        cam.targetTexture = rtex;
        cam.Render();

        var tex = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);

        RenderTexture.active = rtex;

        tex.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);

        try {
          Directory.CreateDirectory("Snapshots");

          // TODO: These files are getting written in the wrong color space
          File.WriteAllBytes(
            Path.Combine("Snapshots", $"snapshot_{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ff}.jpg"),
            tex.EncodeToJPG(player.CamJpegQuality));
        }
        catch (IOException ex) {
          Debug.LogError($"Failed to write snapshot file: {ex}");
        }
      }
      finally {
        cam.targetTexture = viewfinderTex;
      }
    }

    // TODO: fix the focal length calculations to make sure this is accurate
    void UpdateFocalLen() {
      hud.FocalLen = $"{Mathf.Round(focalLen)} mm";
    }

    void FixedUpdate() {
      const float ASPECT = 3.0f / 4.0f;

      focalLen = Mathf.Clamp(focalLen + focalLenSpeed * LEN_ADJUST_MAX_SPEED * Time.fixedDeltaTime, MIN_FOCAL_LEN, MAX_FOCAL_LEN);
      UpdateFocalLen();
      cam.fieldOfView = 2.0f * Mathf.Rad2Deg * Mathf.Atan(0.5f * (player.CamFilmSize * ASPECT) / focalLen);
    }
  }
}
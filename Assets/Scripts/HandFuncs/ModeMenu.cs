using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace VRScout.HandFuncs {
  public class ModeMenu : MonoBehaviour {
    abstract class LayoutElement {
      public Vector2 DesiredSize { get; private set; }

      public void Measure(Vector2 space) {
        DesiredSize = MeasureImpl(space);
      }

      public void Arrange(Rect space) {
        ArrangeImpl(space);
      }

      protected abstract Vector2 MeasureImpl(Vector2 space);

      protected abstract void ArrangeImpl(Rect space);
    }

    class MenuItemElement : LayoutElement {
      ModeMenuItem item;
      float z;

      public MenuItemElement(ModeMenuItem item, float z) {
        this.item = item;
        this.z = z;
      }

      protected override Vector2 MeasureImpl(Vector2 space) => item.Size;

      protected override void ArrangeImpl(Rect space) {
        var pos = new Vector2(space.xMin, space.yMin) + 0.5f * item.Size;
        pos.Scale(new Vector2(1.0f, -1.0f));
        item.Position = new Vector3(pos.x, pos.y, z);
      }
    }

    abstract class ContentElement : LayoutElement {
      public LayoutElement Content { get; set; } // NB: This is not observable
    }

    abstract class ItemsElement : LayoutElement {
      public List<LayoutElement> Children { get; } = new List<LayoutElement>(); // NB: This is not observable
    }

    class VSpacerElement : LayoutElement {
      public float Height { get; set; }

      protected override Vector2 MeasureImpl(Vector2 space) => new Vector2(0.0f, Height);

      protected override void ArrangeImpl(Rect space) { }
    }

    class MarginElement : ContentElement {
      public Vector4 Margin { get; set; } // (left, top, right, bottom)

      Vector2 PosOffset => new Vector2(Margin.x, Margin.y);
      Vector2 SizeOffset => new Vector2(Margin.x + Margin.z, Margin.y + Margin.w);

      protected override Vector2 MeasureImpl(Vector2 space) {
        Content.Measure(space - SizeOffset);
        return Content.DesiredSize + SizeOffset;
      }

      protected override void ArrangeImpl(Rect space) {
        space.position += PosOffset;
        space.size -= SizeOffset;
        Content.Arrange(space);
      }
    }

    class WrapPanelElement : ItemsElement {
      protected override Vector2 MeasureImpl(Vector2 space) { // TODO
        float width = 0.0f, maxHeight = 0.0f;

        var spaceLeft = space;

        foreach (var child in Children) {
          child.Measure(space);

          width += child.DesiredSize.x;
          spaceLeft.x -= child.DesiredSize.x;
          // TODO: handle wrapping
          maxHeight = Mathf.Max(maxHeight, child.DesiredSize.y);
        }

        return new Vector2(width, maxHeight);
      }

      protected override void ArrangeImpl(Rect space) {
        float x = 0.0f;

        foreach (var child in Children) {
          // TODO: handle wrapping
          child.Arrange(new Rect(space.xMin + x, space.yMin, child.DesiredSize.x, DesiredSize.y));
          x += child.DesiredSize.x;
        }
      }
    }

    class VStackPanelElement : ItemsElement {
      protected override Vector2 MeasureImpl(Vector2 space) {
        float maxWidth = 0.0f, height = 0.0f; // TODO: Should this stretch to fit all available space?

        var spaceLeft = space;

        foreach (var child in Children) {
          child.Measure(spaceLeft);

          maxWidth = Mathf.Max(maxWidth, child.DesiredSize.x);
          height += child.DesiredSize.y;
          spaceLeft.y -= child.DesiredSize.y;
        }

        return new Vector2(maxWidth, height);
      }

      protected override void ArrangeImpl(Rect space) {
        float y = 0.0f;

        foreach (var child in Children) {
          child.Arrange(new Rect(space.xMin, space.yMin + y, DesiredSize.x, child.DesiredSize.y));
          y += child.DesiredSize.y;
        }
      }
    }

    const float SCALE = 0.005f;
    const float Z_GAP = 200.0f;
    const float MARGIN = 2.0f;

    ModeMenuItem InstantiateItem(
        GameObject prefab,
        ModeMenuItem.ModeType type,
        int index,
        string text) {
      var itemObj = GameObject.Instantiate(prefab, transform, false);
      var item = itemObj.GetComponent<ModeMenuItem>();

      item.type = type;
      item.index = index;
      item.Scale = SCALE;
      item.Size = new Vector2(100.0f, 30.0f);
      item.SetText(text);

      return item;
    }

    public void Init(IPlayerController player, IHandModeController modeCtl) {
      // Because these are generated values
      var primaryModes = modeCtl.PrimaryModes;
      var gripModes = modeCtl.GripModes;

      var root = new VStackPanelElement();

      var primaryPanel = new WrapPanelElement();
      root.Children.Add(new MarginElement {
        Margin = Vector4.one * -MARGIN,
        Content = primaryPanel,
      });

      root.Children.Add(new VSpacerElement { Height = 20.0f, });

      var gripPanel = new WrapPanelElement();
      root.Children.Add(new MarginElement {
        Margin = Vector4.one * -MARGIN,
        Content = gripPanel,
      });

      for (int i = 0; i < primaryModes.Count; ++i) {
        var mode = primaryModes[i];

        var item = InstantiateItem(player.ModeMenuItem, ModeMenuItem.ModeType.Primary, i, mode.Name);

        if (i == modeCtl.CurrPrimaryMode) item.SetSelected();

        var el = new MenuItemElement(item, Z_GAP);
        primaryPanel.Children.Add(new MarginElement {
          Margin = Vector4.one * MARGIN,
          Content = el,
        });
      }

      for (int i = 0; i < gripModes.Count; ++i) {
        var mode = gripModes[i];

        var item = InstantiateItem(player.ModeMenuItem, ModeMenuItem.ModeType.Grip, i, mode.Name);

        if (i == modeCtl.CurrGripMode) item.SetSelected();

        var el = new MenuItemElement(item, Z_GAP);
        gripPanel.Children.Add(new MarginElement {
          Margin = Vector4.one * MARGIN,
          Content = el,
        });
      }

      root.Measure(new Vector2(1000.0f, float.PositiveInfinity));
      root.Arrange(new Rect(root.DesiredSize * -0.5f, root.DesiredSize));
    }
  }
}
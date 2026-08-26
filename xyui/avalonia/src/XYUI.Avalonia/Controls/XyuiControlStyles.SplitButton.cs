using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void AddEdgeBase(Styles styles)
    {
        var edge = new Style(x => x.OfType<XyuiActionEdge>());
        Set(edge, Border.BackgroundProperty, "XY.Brush.Accent.Strong");
        edge.Setters.Add(new Setter(Border.HeightProperty, XyuiActionEdge.DefaultHeight));
        styles.Add(edge);
    }

    static void AddSplitButton(Styles styles)
    {
        var split = new Style(x => x.OfType<XYSplitButton>().Class("xyui-split-button"));
        Set(split, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        Set(split, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(split, TemplatedControl.FontFamilyProperty, "XY.Font.UI");
        Set(split, TemplatedControl.FontSizeProperty, "XY.FontSize.Body");
        Set(split, TemplatedControl.FontWeightProperty, "XY.FontWeight.Medium");
        Set(split, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        split.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1)));
        split.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(3)));
        split.Setters.Add(new Setter(Control.HeightProperty, 36d));
        split.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(16, 0)));
        styles.Add(split);
        Zone(styles, "xyui-split-main");
        MenuZone(styles, "xyui-split-menu");
        var divider = new Style(x => x.OfType<Border>().Class("xyui-split-divider"));
        Set(divider, Border.BackgroundProperty, "XY.Brush.Border.Color.Subtle");
        styles.Add(divider);
        FocusRing(styles, typeof(XYSplitButton), "xyui-split-button");
        AttenuatedDisabled(styles, typeof(XYSplitButton), "xyui-split-button");
    }

    static void Zone(Styles styles, string cls)
    {
        var zone = new Style(x => x.OfType<Button>().Class(cls));
        zone.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        zone.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(0)));
        styles.Add(zone);
        State(styles, typeof(Button), cls, ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(Button), cls, ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        TemplateSurface(styles, typeof(Button), cls, ":pointerover", "XY.Brush.State.Color.Hover");
        TemplateSurface(styles, typeof(Button), cls, ":pressed", "XY.Brush.State.Color.Pressed");
        var disabled = new Style(x => x.OfType<Button>().Class(cls).Class(":disabled"));
        disabled.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        styles.Add(disabled);
        TemplateSurface(styles, typeof(Button), cls, ":disabled", Brushes.Transparent);
    }

    static void MenuZone(Styles styles, string cls)
    {
        var zone = new Style(x => x.OfType<Button>().Class(cls));
        Set(zone, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.PanelAlt");
        zone.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(0)));
        Set(zone, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Secondary");
        styles.Add(zone);
        State(styles, typeof(Button), cls, ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(Button), cls, ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        State(styles, typeof(Button), cls, ":pointerover", TemplatedControl.ForegroundProperty, "XY.Brush.Accent.Strong");
        State(styles, typeof(Button), cls, ":pressed", TemplatedControl.ForegroundProperty, "XY.Brush.Border.Color.Selected");
        State(styles, typeof(Button), cls, ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text");
        TemplateSurface(styles, typeof(Button), cls, ":pointerover", "XY.Brush.State.Color.Hover");
        TemplateSurface(styles, typeof(Button), cls, ":pressed", "XY.Brush.State.Color.Pressed");
        var disabled = new Style(x => x.OfType<Button>().Class(cls).Class(":disabled"));
        Set(disabled, TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background");
        styles.Add(disabled);
        TemplateSurface(styles, typeof(Button), cls, ":disabled", "XY.Brush.State.Disabled.Background");
    }

    static void TemplateSurface(Styles styles, Type type, string cls, string state, string resource)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state).Template().OfType<ContentPresenter>());
        Set(style, TemplatedControl.BackgroundProperty, resource);
        styles.Add(style);
    }

    static void TemplateSurface(Styles styles, Type type, string cls, string state, IBrush brush)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state).Template().OfType<ContentPresenter>());
        style.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, brush));
        styles.Add(style);
    }
}

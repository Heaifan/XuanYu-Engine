using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Controls;

// XYUI-2-02 Icon Button（方案 2 · Ghost Reveal，Command：Selected≠Checked）
// 与 XYUI-2-03 Toggle Button（方案 1 · Persistent Edge，Command-Toggle）。
public static partial class XyuiControlStyles
{
    // Ghost：Default 仅图标透明容器；Hover 显浅 Surface；Pressed 更深；
    // Selected 持续 Selected Surface + Border.Selected + Action Edge（外部 IsSelected 驱动，点击不切换）。
    static void AddGhostIconButton(Styles styles)
    {
        var icon = new Style(x => x.OfType<XYIconButton>().Class("xyui-icon-button"));
        icon.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XyuiButtonChrome.Create<XYIconButton>(HorizontalAlignment.Center)));
        // Canonical Background.Default = Transparent（字面值，非 token）。
        icon.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        Set(icon, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Secondary");
        icon.Setters.Add(new Setter(TemplatedControl.BorderBrushProperty, Brushes.Transparent));
        icon.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(XyuiSpatialTokens.BorderWidthDefault)));
        icon.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(XyuiSpatialTokens.RadiusButton)));
        icon.Setters.Add(new Setter(Control.WidthProperty, 34d));
        icon.Setters.Add(new Setter(Control.HeightProperty, 34d));
        icon.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(0)));
        styles.Add(icon);
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":selected", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Selected");
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":selected", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Selected");
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":selected", TemplatedControl.ForegroundProperty, "XY.Brush.Accent.Strong");
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":pointerover", TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        State(styles, typeof(XYIconButton), "xyui-icon-button", ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        FocusRing(styles, typeof(XYIconButton), "xyui-icon-button");
        AttenuatedDisabled(styles, typeof(XYIconButton), "xyui-icon-button");
        XyuiEdgeStyles.Hide(styles, typeof(XYIconButton), "xyui-icon-button");
        XyuiEdgeStyles.Show(styles, typeof(XYIconButton), "xyui-icon-button", ":selected");
    }

    // Persistent Edge：OFF 常规 Surface；ON 持续 Action Edge + Active 底 + Border.On=Selected；
    // OffHover/OnHover 同为 State.Hover；IsChecked 承载 ON/OFF。
    static void AddActionToggleButton(Styles styles)
    {
        var toggle = new Style(x => x.OfType<XYToggleButton>().Class("xyui-toggle-button"));
        toggle.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XyuiButtonChrome.Create<XYToggleButton>(HorizontalAlignment.Center)));
        Chrome(toggle);
        styles.Add(toggle);
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":checked", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Active");
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":checked", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Selected");
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        FocusRing(styles, typeof(XYToggleButton), "xyui-toggle-button");
        AttenuatedDisabled(styles, typeof(XYToggleButton), "xyui-toggle-button");
        XyuiEdgeStyles.Hide(styles, typeof(XYToggleButton), "xyui-toggle-button");
        XyuiEdgeStyles.Show(styles, typeof(XYToggleButton), "xyui-toggle-button", ":checked");
        XyuiEdgeStyles.HoverEdge(styles, typeof(XYToggleButton), "xyui-toggle-button");
    }
}

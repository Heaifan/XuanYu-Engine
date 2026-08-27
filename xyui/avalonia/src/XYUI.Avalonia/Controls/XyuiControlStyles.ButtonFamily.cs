using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Controls;

// XYUI-2 Batch 01 按钮家族样式入口。Action Edge 语言：
// Primary=Accent.Strong；Secondary=Divider.Default 弱化；Danger=Error.Text；
// Disabled=State.Disabled.Border 随 Chrome 衰减；Focus 独立走 Foundation Focus Outline。
public static partial class XyuiControlStyles
{
    internal static void AddButtonFamily(Styles styles)
    {
        AddEdgeBase(styles);
        AddActionButton(styles);
        AddGhostIconButton(styles);
        AddActionToggleButton(styles);
        AddSplitButton(styles);
        AddDropDownButton(styles);
    }

    // XYUI-2-01 Button（方案 4 · Action Edge）。
    static void AddActionButton(Styles styles)
    {
        var button = new Style(x => x.OfType<XYButton>().Class("xyui-button"));
        // 用户裁定：家族文字一律统一左对齐（图标 IconButton 除外，保持居中）。
        button.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XyuiButtonChrome.Create<XYButton>(HorizontalAlignment.Left)));
        Chrome(button);
        styles.Add(button);
        State(styles, typeof(XYButton), "xyui-button", ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(XYButton), "xyui-button", ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        DangerVariant(styles);
        VariantEdges(styles);
        FocusRing(styles, typeof(XYButton), "xyui-button");
        AttenuatedDisabled(styles, typeof(XYButton), "xyui-button");
        XyuiEdgeStyles.HoverEdge(styles, typeof(XYButton), "xyui-button");
    }

    // 家族公共 Chrome：Raised 底 / Text.Primary 字 / Default 边框 / Radius.Button / 高 34 / 水平 Padding=Space3。
    // 排版必须显式消费 Foundation token，否则按钮文字会继承宿主主题默认字体，与 XYUI-1 文本控件割裂。
    static void Chrome(Style style)
    {
        Set(style, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        Set(style, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(style, TemplatedControl.FontFamilyProperty, "XY.Font.UI");
        Set(style, TemplatedControl.FontSizeProperty, "XY.FontSize.Body");
        Set(style, TemplatedControl.FontWeightProperty, "XY.FontWeight.Medium");
        Set(style, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        style.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(XyuiSpatialTokens.BorderWidthDefault)));
        style.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(XyuiSpatialTokens.RadiusButton)));
        style.Setters.Add(new Setter(Control.HeightProperty, 34d));
        style.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(XyuiSpatialTokens.Space3, 0)));
    }

    static void DangerVariant(Styles styles)
    {
        var danger = new Style(x => x.OfType<XYButton>().Class("xyui-button-danger"));
        Set(danger, TemplatedControl.BorderBrushProperty, "XY.Brush.Semantic.Error.Border");
        styles.Add(danger);
        XyuiEdgeStyles.Fill(styles, x => x!.OfType<XYButton>().Class("xyui-button-danger"), "XY.Brush.Semantic.Error.Text");
    }

    // Primary 显式声明 Accent Edge（不再只靠 AddEdgeBase 隐式默认，便于契约断言）；
    // Secondary 走"弱化"分支——消费既有 XY.Divider.Default（中性分隔语义），不发明新 token。
    static void VariantEdges(Styles styles)
    {
        XyuiEdgeStyles.Fill(styles, x => x!.OfType<XYButton>().Class("xyui-button-primary"), "XY.Brush.Accent.Strong");
        XyuiEdgeStyles.Fill(styles, x => x!.OfType<XYButton>().Class("xyui-button-secondary"), "XY.Brush.Divider.Default");
    }

    static void FocusRing(Styles styles, Type type, string cls)
    {
        State(styles, type, cls, ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
        State(styles, type, cls, ":focus", TemplatedControl.BorderThicknessProperty, "XY.Border.Width.Focus");
    }

    // Disabled：Chrome 三件套衰减 + Edge 同步切 State.Disabled.Border（注册靠后，覆盖各变体 Edge 色）。
    static void AttenuatedDisabled(Styles styles, Type type, string cls)
    {
        State(styles, type, cls, ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background");
        State(styles, type, cls, ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text");
        State(styles, type, cls, ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border");
        XyuiEdgeStyles.FillState(styles, type, cls, ":disabled", "XY.Brush.State.Disabled.Border");
    }

}

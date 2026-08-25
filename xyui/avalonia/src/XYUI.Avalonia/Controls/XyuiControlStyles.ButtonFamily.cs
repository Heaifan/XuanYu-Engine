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
    }

    // XYUI-2-01 Button（方案 4 · Action Edge）。
    static void AddActionButton(Styles styles)
    {
        var button = new Style(x => x.OfType<XYButton>().Class("xyui-button"));
        button.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XyuiButtonChrome.Create<XYButton>(HorizontalAlignment.Center)));
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

    static void AddEdgeBase(Styles styles)
    {
        var edge = new Style(x => x.OfType<XyuiActionEdge>());
        Set(edge, Border.BackgroundProperty, "XY.Brush.Accent.Strong");
        edge.Setters.Add(new Setter(Border.HeightProperty, XyuiActionEdge.DefaultHeight));
        styles.Add(edge);
    }

    // XYUI-2-04 Split Button（方案 2 · Soft Partition · R2 / AMEND-C）：
    // 复用家族 Chrome，整体保持单一外轮廓；Main/Menu 为独立 Hit Zone（各自 Hover，互不影响）。
    // R2：菜单区不整块高亮（避免"第二颗按钮"感），Hover/Pressed 只作用于区内 Chevron 描边；
    // Divider 更短更淡（Border.Color.Subtle，软分区提示而非硬分界）；Action Edge 为共享单条（AddEdgeBase 常显 Accent）。
    static void AddSplitButton(Styles styles)
    {
        var split = new Style(x => x.OfType<XYSplitButton>().Class("xyui-split-button"));
        Chrome(split);
        styles.Add(split);

        Zone(styles, "xyui-split-main");
        MenuZone(styles, "xyui-split-menu");

        var divider = new Style(x => x.OfType<Border>().Class("xyui-split-divider"));
        Set(divider, Border.BackgroundProperty, "XY.Brush.Border.Color.Subtle");
        styles.Add(divider);

        FocusRing(styles, typeof(XYSplitButton), "xyui-split-button");
        AttenuatedDisabled(styles, typeof(XYSplitButton), "xyui-split-button");
    }

    // Hit Zone（主体）：默认透明（透出 Chrome 底）；Hover/Pressed 只覆盖主区自身；
    // Disabled 时恢复透明（Chrome 已整体衰减）。内容对齐由模板绑定各自区预设。
    // R2.1（AMEND-D）：同步覆盖模板内 ContentPresenter 表面，阻止宿主主题（Fluent/Simple）
    // 的 `:pointerover /template/ ContentPresenter` hover 刷子（Light 浅灰 / 近黑）泄漏到分区。
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

    // R2.1（AMEND-D）：菜单区在共享 Chrome 轮廓内独立使用 Hover/Pressed Surface（ONLY Menu Zone），
    // 不显示独立边框/边缘，不构成"第二颗按钮"；Chevron 同步 Accent（Soft Partition 的独立反馈）。
    // 模板内 ContentPresenter 同步覆盖，防止宿主主题 hover 刷子污染。
    static void MenuZone(Styles styles, string cls)
    {
        var zone = new Style(x => x.OfType<Button>().Class(cls));
        zone.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
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
        disabled.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        styles.Add(disabled);
        TemplateSurface(styles, typeof(Button), cls, ":disabled", Brushes.Transparent);
    }

    // 覆盖宿主主题对模板内 ContentPresenter 的状态表面（/template/ ContentPresenter）。
    // 不带 Name 匹配：Fluent 与内置 Simple 模板的 ContentPresenter 命名不同，统一按类型覆盖。
    static void TemplateSurface(Styles styles, Type type, string cls, string state, string resource)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state)
            .Template().OfType<ContentPresenter>());
        Set(style, TemplatedControl.BackgroundProperty, resource);
        styles.Add(style);
    }

    static void TemplateSurface(Styles styles, Type type, string cls, string state, IBrush brush)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state)
            .Template().OfType<ContentPresenter>());
        style.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, brush));
        styles.Add(style);
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

// XYUI-2-05 DropDown Button 样式：整钮唯一命中区 + 装饰 Chevron Track 同步状态。
// Track 不可命中、自身永不触发伪类，槽底色一律锚定控件级 :pointerover/:pressed；
// 状态合同对齐 Canonical 05：Raised / Hover / Pressed / Focus Outline / Disabled 衰减；
// Action Edge 继承 Button 家族语言（Accent.Strong，Hover 抬升，Disabled 切 Disabled.Border）。
public static partial class XyuiControlStyles
{
    static void AddDropDownButton(Styles styles)
    {
        const string cls = "xyui-dropdown-button";
        var dropdown = new Style(x => x.OfType<XYDropDownButton>().Class(cls));
        dropdown.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYDropDownButton.CreateTemplate()));
        Chrome(dropdown);
        Set(dropdown, XYDropDownButton.ChevronBrushProperty, "XY.Brush.Text.Secondary");
        styles.Add(dropdown);
        State(styles, typeof(XYDropDownButton), cls, ":pointerover",
            TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(XYDropDownButton), cls, ":pressed",
            TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        OpenZone(styles);
        ChevronTrack(styles, cls);
        FocusRing(styles, typeof(XYDropDownButton), cls);
        AttenuatedDisabled(styles, typeof(XYDropDownButton), cls);
        XyuiEdgeStyles.HoverEdge(styles, typeof(XYDropDownButton), cls);
        var disabled = new Style(x => x.OfType<XYDropDownButton>().Class(cls).Class(":disabled"));
        Set(disabled, XYDropDownButton.ChevronBrushProperty, "XY.Brush.State.Disabled.Text");
        styles.Add(disabled);
    }

    // OpenZone：透明底嵌入按钮横跨全钮；状态由控件级 Chrome 表达，禁用时随控件衰减。
    static void OpenZone(Styles styles)
    {
        var zone = new Style(x => x.OfType<Button>().Class("xyui-ddb-zone"));
        zone.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        zone.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(0)));
        styles.Add(zone);
        TemplateSurface(styles, typeof(Button), "xyui-ddb-zone", ":pointerover", Brushes.Transparent);
        TemplateSurface(styles, typeof(Button), "xyui-ddb-zone", ":pressed", Brushes.Transparent);
        var disabled = new Style(x => x.OfType<Button>().Class("xyui-ddb-zone").Class(":disabled"));
        disabled.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent));
        styles.Add(disabled);
        TemplateSurface(styles, typeof(Button), "xyui-ddb-zone", ":disabled", Brushes.Transparent);
    }

    // ChevronTrack 仅保留装饰图标的布局槽；背景透明，所有状态由整钮统一表达。
    static void ChevronTrack(Styles styles, string cls)
    {
        var track = new Style(x => x.OfType<XYDropDownButton>().Class(cls).Descendant()
            .OfType<Border>().Class("xyui-ddb-track"));
        track.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));
        Set(track, Border.WidthProperty, "XY.DropDownButton.ChevronTrack.Width");
        styles.Add(track);
    }
}

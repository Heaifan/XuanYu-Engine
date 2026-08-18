using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddSemantic(Styles styles)
    {
        var dot = new Style(x => x.OfType<XYStatusDot>().Class("xyui-status-dot"));
        dot.Setters.Add(new Setter(Border.WidthProperty, 8d)); dot.Setters.Add(new Setter(Border.HeightProperty, 8d));
        dot.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); styles.Add(dot);
        State(styles, typeof(XYStatusDot), "xyui-status-dot-success", "XY.Brush.Semantic.Success.Text");
        State(styles, typeof(XYStatusDot), "xyui-status-dot-warning", "XY.Brush.Semantic.Warning.Text");
        State(styles, typeof(XYStatusDot), "xyui-status-dot-error", "XY.Brush.Semantic.Error.Text");
        State(styles, typeof(XYStatusDot), "xyui-status-dot-info", "XY.Brush.Semantic.Info.Text");
        State(styles, typeof(XYStatusDot), "xyui-status-dot-neutral", "XY.Brush.Text.Tertiary");
        StateText(styles, "xyui-status-text-success", "XY.Brush.Semantic.Success.Text");
        StateText(styles, "xyui-status-text-warning", "XY.Brush.Semantic.Warning.Text");
        StateText(styles, "xyui-status-text-error", "XY.Brush.Semantic.Error.Text");
        StateText(styles, "xyui-status-text-info", "XY.Brush.Semantic.Info.Text");
        StateText(styles, "xyui-status-text-neutral", "XY.Brush.Text.Secondary");
        StateMark(styles, "xyui-status-mark-success", "XY.Brush.Semantic.Success.Text");
        StateMark(styles, "xyui-status-mark-warning", "XY.Brush.Semantic.Warning.Text");
        StateMark(styles, "xyui-status-mark-error", "XY.Brush.Semantic.Error.Text");
        StateMark(styles, "xyui-status-mark-info", "XY.Brush.Semantic.Info.Text");
        StateMark(styles, "xyui-status-mark-neutral", "XY.Brush.Text.Secondary");
        var separator = new Style(x => x.OfType<XYSeparator>().Class("xyui-separator"));
        Brush(separator, Border.BackgroundProperty, "XY.Brush.Divider.Default"); separator.Setters.Add(new Setter(Border.HeightProperty, 1d)); styles.Add(separator);
        var tooltip = new Style(x => x.OfType<XYTooltip>().Class("xyui-tooltip"));
        Brush(tooltip, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Overlay");
        Brush(tooltip, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Subtle");
        tooltip.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(2, 0, 0, 0)));
        tooltip.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(8, 4))); styles.Add(tooltip);
    }

    static void State(Styles styles, Type type, string cls, string brush)
    {
        var style = new Style(x => x.OfType(type).Class(cls)); Brush(style, Border.BackgroundProperty, brush); styles.Add(style);
    }

    static void StateText(Styles styles, string cls, string brush)
    {
        var style = new Style(x => x.OfType<TextBlock>().Class(cls)); Brush(style, TextBlock.ForegroundProperty, brush); styles.Add(style);
    }

    static void StateMark(Styles styles, string cls, string brush)
    {
        var style = new Style(x => x.OfType<VectorPath>().Class(cls)); Brush(style, VectorPath.FillProperty, brush); styles.Add(style);
    }
}

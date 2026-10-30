using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddSemantic(Styles styles)
    {
        var dot = new Style(x => x.OfType<XYStatusDot>().Class("xyui-status-dot"));
        dot.Setters.Add(new Setter(Border.WidthProperty, XYStatusDot.Diameter)); dot.Setters.Add(new Setter(Border.HeightProperty, XYStatusDot.Diameter));
        dot.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(XYStatusDot.Diameter / 2))); styles.Add(dot);
        foreach (var state in Enum.GetValues<XyuiStatusState>())
        {
            var name = state.ToString().ToLowerInvariant();
            State(styles, typeof(XYStatusDot), $"xyui-status-dot-{name}", XyuiStatusStateTokens.Indicator(state));
            StateText(styles, $"xyui-status-text-{name}", XyuiStatusStateTokens.Foreground(state));
            StateMark(styles, $"xyui-status-mark-{name}", XyuiStatusStateTokens.Foreground(state));
        }
        var dotDisabled = new Style(x => x.OfType<XYStatusDot>().Class("xyui-status-dot").Class(":disabled"));
        Brush(dotDisabled, Border.BackgroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(dotDisabled);
        var badgeDisabled = new Style(x => x.OfType<XYStatusBadge>().Class("xyui-status-badge").Class(":disabled"));
        badgeDisabled.Setters.Add(new Setter(Border.BackgroundProperty, null)); styles.Add(badgeDisabled);
        var badgeTextDisabled = new Style(x => x.OfType<XYStatusBadge>().Class("xyui-status-badge").Class(":disabled").Descendant().OfType<TextBlock>());
        Brush(badgeTextDisabled, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(badgeTextDisabled);
        var badgeMarkDisabled = new Style(x => x.OfType<XYStatusBadge>().Class("xyui-status-badge").Class(":disabled").Descendant().OfType<VectorPath>());
        Brush(badgeMarkDisabled, VectorPath.StrokeProperty, "XY.Brush.State.Disabled.Text"); Brush(badgeMarkDisabled, VectorPath.FillProperty, "XY.Brush.State.Disabled.Text"); styles.Add(badgeMarkDisabled);
        DisabledText(styles, "xyui-code-text-text"); DisabledText(styles, "xyui-empty-text");
        DisabledText(styles, "xyui-search-highlight"); DisabledText(styles, "xyui-truncated-text"); DisabledText(styles, "xyui-mono-data-label");
        DisabledText(styles, "xyui-mono-data-value"); DisabledText(styles, "xyui-mono-data-unit");
        DisabledText(styles, "xyui-icon-label-text");
        var richTextDisabled = new Style(x => x.OfType<XYRichText>().Class("xyui-rich-text").Class(":disabled"));
        Brush(richTextDisabled, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(richTextDisabled);
        FeedbackDisabled(styles, typeof(XYHelpText), "xyui-help-text", "xyui-help-text-text", "xyui-help-text-mark");
        FeedbackDisabled(styles, typeof(XYErrorText), "xyui-error-text", "xyui-error-text-text", "xyui-error-text-mark");
        FeedbackDisabled(styles, typeof(XYWarningText), "xyui-warning-text", "xyui-warning-text-text", "xyui-warning-text-mark");
        var iconLabelTextDisabled = new Style(x => x.OfType<XYIconLabel>().Class("xyui-icon-label").Class(":disabled").Descendant().OfType<TextBlock>().Class("xyui-icon-label-text"));
        Brush(iconLabelTextDisabled, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(iconLabelTextDisabled);
        var iconLabelIconDisabled = new Style(x => x.OfType<XYIconLabel>().Class("xyui-icon-label").Class(":disabled").Descendant().OfType<XYIcon>().Class("xyui-icon-label-icon"));
        Brush(iconLabelIconDisabled, XYIcon.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(iconLabelIconDisabled);
        var separator = new Style(x => x.OfType<XYSeparator>().Class("xyui-separator"));
        Brush(separator, Border.BackgroundProperty, "XY.Brush.Divider.Default"); separator.Setters.Add(new Setter(Border.HeightProperty, XyuiSpatialTokens.BorderWidthDefault)); styles.Add(separator);
        SeparatorVariant(styles, "header", false, 0); SeparatorVariant(styles, "panel", false, 0); SeparatorVariant(styles, "section", false, XyuiSpatialTokens.Space2); SeparatorVariant(styles, "listrow", false, XyuiSpatialTokens.Space4); SeparatorVariant(styles, "verticalsplit", true, 0);
        var tooltip = new Style(x => x.OfType<XYTooltip>().Class("xyui-tooltip"));
        Brush(tooltip, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Overlay");
        Brush(tooltip, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Subtle");
        tooltip.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(XyuiSpatialTokens.BorderWidthStrong, 0, 0, 0)));
        tooltip.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(XyuiSpatialTokens.RadiusPopup)));
        tooltip.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(XyuiSpatialTokens.Space2, XyuiSpatialTokens.Space1)));
        tooltip.Setters.Add(new Setter(TemplatedControl.FontFamilyProperty, new FontFamily(XyuiTypographyTokens.FontUi)));
        tooltip.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, XyuiTypographyTokens.FontSizeCaption));
        tooltip.Setters.Add(new Setter(TemplatedControl.FontWeightProperty, FontWeight.Normal)); styles.Add(tooltip);
    }

    static void SeparatorVariant(Styles styles, string name, bool vertical, double inset)
    {
        var style = new Style(x => x.OfType<XYSeparator>().Class($"xyui-separator-{name}"));
        style.Setters.Add(new Setter(Border.HeightProperty, vertical ? double.NaN : XyuiSpatialTokens.BorderWidthDefault));
        style.Setters.Add(new Setter(Border.WidthProperty, vertical ? XyuiSpatialTokens.BorderWidthDefault : double.NaN));
        style.Setters.Add(new Setter(Border.MarginProperty, new Thickness(inset, 0, inset, 0))); styles.Add(style);
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

    static void DisabledMark(Styles styles, string markClass)
    {
        var style = new Style(x => x.OfType<VectorPath>().Class(markClass).Class(":disabled"));
        Brush(style, VectorPath.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(style);
    }

    static void FeedbackDisabled(Styles styles, Type type, string controlClass, string textClass, string markClass)
    {
        var text = new Style(x => x.OfType(type).Class(controlClass).Class(":disabled").Descendant().OfType<TextBlock>().Class(textClass));
        Brush(text, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(text);
        var mark = new Style(x => x.OfType(type).Class(controlClass).Class(":disabled").Descendant().OfType<VectorPath>().Class(markClass));
        Brush(mark, VectorPath.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(mark);
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddSurfaces(Styles styles)
    {
        Surface(styles, typeof(XYBadge), "xyui-badge", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.PanelAlt");
        Surface(styles, typeof(XYCodeText), "xyui-code-text", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Tertiary", "XY.Brush.Surface.PanelAlt");
        Surface(styles, typeof(XYStatusBadge), "xyui-status-badge", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.Panel");
        Surface(styles, typeof(XYShortcutHint), "xyui-shortcut-hint", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.PanelAlt");
        Surface(styles, typeof(XYHelpText), "xyui-help-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.Panel");
        Surface(styles, typeof(XYErrorText), "xyui-error-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Semantic.Error.Text", "XY.Brush.Surface.Panel");
        Surface(styles, typeof(XYWarningText), "xyui-warning-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Semantic.Warning.Text", "XY.Brush.Surface.Panel");
        var accent = new Style(x => x.OfType<XYBadge>().Class("xyui-badge-accent"));
        Brush(accent, Border.BackgroundProperty, "XY.Brush.Tag.Accent");
        Brush(accent, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle");
        styles.Add(accent);
        var pointer = new Style(x => x.OfType<XYBadge>().Class("xyui-badge"));
        pointer.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0, 4, 4, 0))); styles.Add(pointer);
        InlineSurface(styles, typeof(XYStatusBadge), "xyui-status-badge");
        InlineSurface(styles, typeof(XYHelpText), "xyui-help-text");
        InlineSurface(styles, typeof(XYErrorText), "xyui-error-text");
        InlineSurface(styles, typeof(XYWarningText), "xyui-warning-text");
    }

    static void InlineSurface(Styles styles, Type type, string cls)
    {
        var style = new Style(x => x.OfType(type).Class(cls));
        style.Setters.Add(new Setter(Border.BackgroundProperty, null));
        style.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0)));
        style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0))); styles.Add(style);
    }

    static void Surface(Styles styles, Type type, string cls, string family, double size, int weight, double line, string foreground, string background)
    {
        var parent = new Style(x => x.OfType(type).Class(cls));
        Brush(parent, Border.BackgroundProperty, background);
        parent.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(6, 2)));
        parent.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4)));
        styles.Add(parent);
        styles.Add(Text(typeof(TextBlock), $"{cls}-text", family, size, weight, line, foreground));
    }
}

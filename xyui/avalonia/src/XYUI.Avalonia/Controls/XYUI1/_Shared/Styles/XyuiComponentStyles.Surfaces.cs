using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;
using XYUI.Avalonia.Sizing;
using XYUI.Avalonia;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddSurfaces(Styles styles)
    {
        TagSurface(styles);
        Surface(styles, typeof(XYCodeText), "xyui-code-text", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Tertiary", "XY.Brush.Surface.PanelAlt", XyuiSizingMetrics.For(XYSize.Default).ControlHeight);
        Surface(styles, typeof(XYStatusBadge), "xyui-status-badge", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.Panel");
        ShortcutSurface(styles);
        Surface(styles, typeof(XYHelpText), "xyui-help-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary", "XY.Brush.Surface.Panel");
        Surface(styles, typeof(XYErrorText), "xyui-error-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Semantic.Error.Text", "XY.Brush.Surface.Panel");
        Surface(styles, typeof(XYWarningText), "xyui-warning-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Semantic.Warning.Text", "XY.Brush.Surface.Panel");
        var defaultTag = new Style(x => x.OfType<XyuiBadgeTagPath>().Class("xyui-badge-mark-default"));
        Brush(defaultTag, Shape.FillProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(defaultTag);
        var accentTag = new Style(x => x.OfType<XyuiBadgeTagPath>().Class("xyui-badge-mark-accent"));
        Brush(accentTag, Shape.FillProperty, "XY.Brush.Tag.Accent"); styles.Add(accentTag);
        InlineSurface(styles, typeof(XYStatusBadge), "xyui-status-badge");
        InlineSurface(styles, typeof(XYHelpText), "xyui-help-text");
        InlineSurface(styles, typeof(XYErrorText), "xyui-error-text");
        InlineSurface(styles, typeof(XYWarningText), "xyui-warning-text");
    }

    static void ShortcutSurface(Styles styles)
    {
        var parent = new Style(x => x.OfType<XYShortcutHint>().Class("xyui-shortcut-hint"));
        Brush(parent, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); parent.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0))); styles.Add(parent);
        var key = new Style(x => x.OfType<Border>().Class("xyui-shortcut-keycap"));
        Brush(key, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); Brush(key, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle");
        key.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); key.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); key.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(6, 2))); key.Setters.Add(new Setter(Border.HeightProperty, 22d)); styles.Add(key);
        styles.Add(Text(typeof(TextBlock), "xyui-shortcut-keycap-text", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(TextBlock), "xyui-shortcut-separator", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary"));
    }

    static void InlineSurface(Styles styles, Type type, string cls)
    {
        var style = new Style(x => x.OfType(type).Class(cls));
        style.Setters.Add(new Setter(Border.BackgroundProperty, null));
        style.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0)));
        style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0))); styles.Add(style);
    }

    static void TagSurface(Styles styles)
    {
        var surface = new Style(x => x.OfType<XYBadge>().Class("xyui-badge"));
        surface.Setters.Add(new Setter(Border.BackgroundProperty, null)); surface.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0)));
        surface.Setters.Add(new Setter(Border.HeightProperty, 22d));
        surface.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0))); styles.Add(surface);
        styles.Add(Text(typeof(TextBlock), "xyui-badge-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(TextBlock), "xyui-badge-text-accent", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 500, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Accent.Default"));
    }

    static void Surface(Styles styles, Type type, string cls, string family, double size, int weight, double line, string foreground, string background, double? height = null)
    {
        var parent = new Style(x => x.OfType(type).Class(cls));
        Brush(parent, Border.BackgroundProperty, background);
        parent.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(6, 2)));
        parent.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4)));
        if (height.HasValue) parent.Setters.Add(new Setter(Border.HeightProperty, height.Value));
        styles.Add(parent);
        styles.Add(Text(typeof(TextBlock), $"{cls}-text", family, size, weight, line, foreground));
    }
}

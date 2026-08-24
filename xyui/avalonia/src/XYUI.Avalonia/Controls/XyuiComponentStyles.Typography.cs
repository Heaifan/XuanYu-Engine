using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddTypography(Styles styles)
    {
        styles.Add(Text(typeof(XYText), "xyui-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYLabel), "xyui-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeLabel, 500, XyuiTypographyTokens.LineHeightLabel, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYCaption), "xyui-caption", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(XYHeading), "xyui-heading-panel", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePanelTitle, 600, XyuiTypographyTokens.LineHeightPanelTitle, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYHeading), "xyui-heading-page", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePageTitle, 700, XyuiTypographyTokens.LineHeightPageTitle, "XY.Brush.Text.Primary"));
        var section = new Style(x => x.OfType<XYSectionTitle>().Class("xyui-section-title"));
        section.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0, 0, 0, 4)));
        section.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
        Brush(section, Border.BorderBrushProperty, "XY.Brush.Divider.Section"); styles.Add(section);
        styles.Add(Text(typeof(TextBlock), "xyui-section-title-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeSection, 600, XyuiTypographyTokens.LineHeightSection, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYLink), "xyui-link", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 500, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Link"));
        styles.Add(Text(typeof(XYMonoText), "xyui-mono-text", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Secondary"));
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-icon"));
        Brush(icon, VectorPath.StrokeProperty, "XY.Brush.Text.Secondary"); icon.Setters.Add(new Setter(VectorPath.FillProperty, null)); styles.Add(icon);
        styles.Add(Text(typeof(XYIconLabel), "xyui-icon-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYRichText), "xyui-rich-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(TextBlock), "xyui-selectable-text-content", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(TextBlock), "xyui-selectable-text-technical", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(XYEmptyText), "xyui-empty-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Tertiary"));
        var empty = new Style(x => x.OfType<XYEmptyText>().Class("xyui-empty-text"));
        empty.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)); styles.Add(empty);
        styles.Add(Text(typeof(XYSearchHighlight), "xyui-search-highlight", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYTruncatedText), "xyui-truncated-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        IconSize(styles, "tiny", 12, 1.0); IconSize(styles, "small", 14, 1.25); IconSize(styles, "medium", 16, 1.5); IconSize(styles, "large", 20, 1.75);
        Mark(styles, "xyui-code-text-mark", "XY.Brush.Text.Tertiary", 8, false, 1.0);
        Mark(styles, "xyui-icon-label-mark", "XY.Brush.Text.Primary", 14, false);
        Mark(styles, "xyui-help-text-mark", "XY.Brush.Semantic.Info.Text", 14, false);
        Mark(styles, "xyui-error-text-mark", "XY.Brush.Semantic.Error.Text", 14, false);
        Mark(styles, "xyui-warning-text-mark", "XY.Brush.Semantic.Warning.Text", 14, false);
        Mark(styles, "xyui-search-highlight-mark", "XY.Brush.Text.Secondary", 8, false, 1.0);
        Mark(styles, "xyui-selectable-copy-mark", "XY.Brush.Text.Tertiary", 12, false);
    }

    static void IconSize(Styles styles, string name, double size, double stroke)
    {
        var style = new Style(x => x.OfType<XYIcon>().Class($"xyui-icon-{name}"));
        style.Setters.Add(new Setter(VectorPath.WidthProperty, size)); style.Setters.Add(new Setter(VectorPath.HeightProperty, size));
        style.Setters.Add(new Setter(VectorPath.StrokeThicknessProperty, stroke)); styles.Add(style);
    }

    static void Mark(Styles styles, string cls, string brush, double size, bool fill, double stroke = 1.5)
    {
        var style = new Style(x => x.OfType<VectorPath>().Class(cls));
        style.Setters.Add(new Setter(VectorPath.WidthProperty, size)); style.Setters.Add(new Setter(VectorPath.HeightProperty, size));
        style.Setters.Add(new Setter(VectorPath.StrokeThicknessProperty, stroke));
        Brush(style, fill ? VectorPath.FillProperty : VectorPath.StrokeProperty, brush); styles.Add(style);
    }
}

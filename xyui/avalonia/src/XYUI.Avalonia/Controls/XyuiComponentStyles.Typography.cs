using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddTypography(Styles styles)
    {
        styles.Add(Text(typeof(XYText), "xyui-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(XYLabel), "xyui-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeLabel, 600, XyuiTypographyTokens.LineHeightLabel, "XY.Brush.Text.Primary"));
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
        styles.Add(Text(typeof(XYIcon), "xyui-icon", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(XYIconLabel), "xyui-icon-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYRichText), "xyui-rich-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYSelectableText), "xyui-selectable-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYEmptyText), "xyui-empty-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Tertiary"));
        styles.Add(Text(typeof(XYSearchHighlight), "xyui-search-highlight", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYTruncatedText), "xyui-truncated-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        IconSize(styles, "tiny", 12); IconSize(styles, "small", 14); IconSize(styles, "medium", 16); IconSize(styles, "large", 20);
    }

    static void IconSize(Styles styles, string name, double size)
    {
        var style = new Style(x => x.OfType<XYIcon>().Class($"xyui-icon-{name}"));
        style.Setters.Add(new Setter(TextBlock.FontSizeProperty, size)); styles.Add(style);
    }
}

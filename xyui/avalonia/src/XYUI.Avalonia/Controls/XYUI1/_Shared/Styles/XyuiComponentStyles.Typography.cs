using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Typography;
using XYUI.Avalonia.Sizing;
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
        Brush(section, Border.BackgroundProperty, XyuiSectionTitleResources.HeaderBackgroundKey);
        section.Setters.Add(new Setter(Border.HeightProperty, 28d));
        section.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3)));
        section.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0)));
        section.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0))); styles.Add(section);
        var mark = new Style(x => x.OfType<Border>().Class("xyui-section-title-left-mark"));
        Brush(mark, Border.BackgroundProperty, XyuiSectionTitleResources.LeftMarkKey); styles.Add(mark);
        styles.Add(Text(typeof(TextBlock), "xyui-section-title-text", XyuiTypographyTokens.FontUi, 14, 600, 18, XyuiSectionTitleResources.TextKey));
        styles.Add(Text(typeof(XYLink), "xyui-link", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 500, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Link"));
        LinkStates(styles);
        styles.Add(Text(typeof(TextBlock), "xyui-mono-data-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeMono, 600, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(TextBlock), "xyui-mono-data-value", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Secondary"));
        styles.Add(Text(typeof(TextBlock), "xyui-mono-data-unit", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 600, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Secondary"));
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-icon"));
        Brush(icon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); icon.Setters.Add(new Setter(XYIcon.FillProperty, null)); styles.Add(icon);
        var iconDisabled = new Style(x => x.OfType<XYIcon>().Class("xyui-icon").Class(":disabled"));
        Brush(iconDisabled, XYIcon.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(iconDisabled);
        IconButtonIconTint(styles, ":pointerover", "XY.Brush.Text.Primary");
        IconButtonIconTint(styles, ":selected", "XY.Brush.Accent.Strong");
        IconButtonIconTint(styles, ":disabled", "XY.Brush.State.Disabled.Text");
        styles.Add(Text(typeof(XYIconLabel), "xyui-icon-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(TextBlock), "xyui-icon-label-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        var iconLabelIcon = new Style(x => x.OfType<XYIcon>().Class("xyui-icon-label-icon"));
        Brush(iconLabelIcon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(iconLabelIcon);
        styles.Add(Text(typeof(XYRichText), "xyui-rich-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(SelectableTextBlock), "xyui-selectable-text-content", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(SelectableTextBlock), "xyui-selectable-text-technical", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, 400, XyuiTypographyTokens.LineHeightMono, "XY.Brush.Text.Secondary"));
        var selection = new Style(x => x.OfType<SelectableTextBlock>().Class("xyui-selectable-text-content"));
        Brush(selection, SelectableTextBlock.SelectionBrushProperty, "XY.Brush.Surface.Selected");
        Brush(selection, SelectableTextBlock.SelectionForegroundBrushProperty, "XY.Brush.Text.Primary"); styles.Add(selection);
        styles.Add(Text(typeof(XYEmptyText), "xyui-empty-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, 400, XyuiTypographyTokens.LineHeightCaption, "XY.Brush.Text.Tertiary"));
        var empty = new Style(x => x.OfType<XYEmptyText>().Class("xyui-empty-text"));
        empty.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)); styles.Add(empty);
        styles.Add(Text(typeof(XYSearchHighlight), "xyui-search-highlight", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        styles.Add(Text(typeof(XYTruncatedText), "xyui-truncated-text", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, 400, XyuiTypographyTokens.LineHeightBody, "XY.Brush.Text.Primary"));
        IconSize(styles, "tiny", XyuiIconSizeMetrics.For(XyuiIconSize.Tiny)); IconSize(styles, "small", XyuiIconSizeMetrics.For(XyuiIconSize.Small)); IconSize(styles, "medium", XyuiIconSizeMetrics.For(XyuiIconSize.Medium)); IconSize(styles, "large", XyuiIconSizeMetrics.For(XyuiIconSize.Large)); IconSize(styles, "touch", XyuiIconSizeMetrics.For(XyuiIconSize.Touch));
        Mark(styles, "xyui-code-text-mark", "XY.Brush.Icon.Mark", 8, false, 1.25);
        Mark(styles, "xyui-icon-label-mark", "XY.Brush.Text.Primary", 14, false);
        var smallIcon = XyuiIconSizeMetrics.For(XyuiIconSize.Small);
        Mark(styles, "xyui-help-text-mark", "XY.Brush.Semantic.Info.Text", smallIcon.SizeDip, false, smallIcon.StrokeWidth);
        Mark(styles, "xyui-error-text-mark", "XY.Brush.Semantic.Error.Text", smallIcon.SizeDip, false, smallIcon.StrokeWidth);
        Mark(styles, "xyui-warning-text-mark", "XY.Brush.Semantic.Warning.Text", smallIcon.SizeDip, false, smallIcon.StrokeWidth);
        DisabledMark(styles, "xyui-help-text-mark"); DisabledMark(styles, "xyui-error-text-mark"); DisabledMark(styles, "xyui-warning-text-mark");
        Mark(styles, "xyui-search-highlight-mark", "XY.Brush.Text.Disabled", 8, false, 1.0);
         Mark(styles, "xyui-selectable-copy-mark", "XY.Brush.Text.Disabled", 8, false, 1.0);
    }

    static void IconSize(Styles styles, string name, XyuiIconSizeMetrics metrics)
    {
        var style = new Style(x => x.OfType<XYIcon>().Class($"xyui-icon-{name}"));
        style.Setters.Add(new Setter(Control.WidthProperty, metrics.SizeDip)); style.Setters.Add(new Setter(Control.HeightProperty, metrics.SizeDip));
        style.Setters.Add(new Setter(XYIcon.StrokeThicknessProperty, metrics.StrokeWidth)); styles.Add(style);
    }

    // XYUI-2-02：IconButton 状态驱动内部矢量图标描边（Icon.Hover=Text.Primary / Icon.Selected=Accent.Strong / Disabled 衰减）。
    static void IconButtonIconTint(Styles styles, string state, string brush)
    {
        var tint = new Style(x => x.OfType<XYIconButton>().Class(state).Descendant().OfType<XYIcon>().Class("xyui-icon"));
        Brush(tint, XYIcon.StrokeProperty, brush); styles.Add(tint);
    }

    static void Mark(Styles styles, string cls, string brush, double size, bool fill, double stroke = 1.5)
    {
        var style = new Style(x => x.OfType<VectorPath>().Class(cls));
        style.Setters.Add(new Setter(VectorPath.WidthProperty, size)); style.Setters.Add(new Setter(VectorPath.HeightProperty, size));
        style.Setters.Add(new Setter(VectorPath.StrokeThicknessProperty, stroke));
        Brush(style, fill ? VectorPath.FillProperty : VectorPath.StrokeProperty, brush); styles.Add(style);
    }

    static void DisabledText(Styles styles, string textClass)
    {
        var style = new Style(x => x.OfType<TextBlock>().Class(textClass).Class(":disabled"));
        Brush(style, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(style);
    }

}

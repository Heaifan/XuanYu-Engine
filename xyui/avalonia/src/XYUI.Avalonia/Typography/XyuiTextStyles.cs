using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Typography;

// 语义文本样式类（xyui-text-* / xyui-heading-*，9 角色）：代码构建，颜色消费 R3-F1 Brush
public static class XyuiTextStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        styles.Add(Text("xyui-text-body", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody,
            XyuiTypographyTokens.WeightRegular, XyuiTypographyTokens.LineHeightBody,
            XyuiTypographyTokens.LetterSpacingBody, "XY.Brush.Text.Primary"));
        styles.Add(Text("xyui-text-label", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeLabel,
            XyuiTypographyTokens.WeightMedium, XyuiTypographyTokens.LineHeightLabel,
            XyuiTypographyTokens.LetterSpacingLabel, "XY.Brush.Text.Primary"));
        styles.Add(Text("xyui-text-caption", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption,
            XyuiTypographyTokens.WeightRegular, XyuiTypographyTokens.LineHeightCaption,
            null, "XY.Brush.Text.Secondary"));
        styles.Add(Text("xyui-text-section", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeSection,
            XyuiTypographyTokens.WeightSemibold, XyuiTypographyTokens.LineHeightSection,
            null, "XY.Brush.Text.Primary"));
        styles.Add(Text("xyui-heading-panel", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePanelTitle,
            XyuiTypographyTokens.WeightSemibold, XyuiTypographyTokens.LineHeightPanelTitle,
            XyuiTypographyTokens.LetterSpacingTitle, "XY.Brush.Text.Primary"));
        styles.Add(Text("xyui-heading-page", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePageTitle,
            XyuiTypographyTokens.WeightBold, XyuiTypographyTokens.LineHeightPageTitle,
            XyuiTypographyTokens.LetterSpacingTitle, "XY.Brush.Text.Primary"));
        styles.Add(Text("xyui-text-link", XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody,
            XyuiTypographyTokens.WeightMedium, XyuiTypographyTokens.LineHeightBody,
            null, "XY.Brush.Text.Link"));
        styles.Add(Text("xyui-text-code", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono,
            XyuiTypographyTokens.WeightRegular, XyuiTypographyTokens.LineHeightMono,
            XyuiTypographyTokens.LetterSpacingMono, "XY.Brush.Text.Tertiary"));
        styles.Add(Text("xyui-text-mono", XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono,
            XyuiTypographyTokens.WeightRegular, XyuiTypographyTokens.LineHeightMono,
            XyuiTypographyTokens.LetterSpacingMono, "XY.Brush.Text.Secondary"));
        return styles;
    }

    private static Style Text(string cls, string family, double size, int weight,
        double lineHeight, double? letterSpacing, string brushKey)
    {
        var style = new Style(x => x.OfType<TextBlock>().Class(cls));
        style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(family)));
        style.Setters.Add(new Setter(TextBlock.FontSizeProperty, size));
        style.Setters.Add(new Setter(TextBlock.FontWeightProperty, Weight(weight)));
        style.Setters.Add(new Setter(TextBlock.LineHeightProperty, lineHeight));
        if (letterSpacing is not null)
        {
            style.Setters.Add(new Setter(TextBlock.LetterSpacingProperty, letterSpacing.Value));
        }
        style.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension(brushKey)));
        return style;
    }

    private static FontWeight Weight(int value) => value switch
    {
        500 => FontWeight.Medium,
        600 => FontWeight.SemiBold,
        700 => FontWeight.Bold,
        _ => FontWeight.Normal,
    };
}

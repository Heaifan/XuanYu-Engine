using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Typography;

// Typography 基础资源：字体/字号/字重/行高/字距（主题无关，Light/Dark 共用）
public static class XyuiTypography
{
    public static ResourceDictionary CreateResources()
    {
        var d = new ResourceDictionary();
        d["XY.Font.UI"] = new FontFamily(XyuiTypographyTokens.FontUi);
        d["XY.Font.Default"] = new FontFamily(XyuiTypographyTokens.FontDefault);
        d["XY.Font.Mono"] = new FontFamily(XyuiTypographyTokens.FontMono);
        d["XY.Font.Technical"] = new FontFamily(XyuiTypographyTokens.FontTechnical);
        d["XY.Font.Fallback.CJK"] = new FontFamily(XyuiTypographyTokens.FontFallbackCjk);
        d["XY.Font.Fallback.Mono"] = new FontFamily(XyuiTypographyTokens.FontFallbackMono);
        d["XY.FontSize.Caption"] = XyuiTypographyTokens.FontSizeCaption;
        d["XY.FontSize.Auxiliary"] = XyuiTypographyTokens.FontSizeAuxiliary;
        d["XY.FontSize.Body"] = XyuiTypographyTokens.FontSizeBody;
        d["XY.FontSize.Label"] = XyuiTypographyTokens.FontSizeLabel;
        d["XY.FontSize.Section"] = XyuiTypographyTokens.FontSizeSection;
        d["XY.FontSize.PanelTitle"] = XyuiTypographyTokens.FontSizePanelTitle;
        d["XY.FontSize.PageTitle"] = XyuiTypographyTokens.FontSizePageTitle;
        d["XY.FontSize.Mono"] = XyuiTypographyTokens.FontSizeMono;
        d["XY.FontWeight.Regular"] = FontWeight.Normal;
        d["XY.FontWeight.Medium"] = FontWeight.Medium;
        d["XY.FontWeight.Semibold"] = FontWeight.SemiBold;
        d["XY.FontWeight.Bold"] = FontWeight.Bold;
        d["XY.LineHeight.Caption"] = XyuiTypographyTokens.LineHeightCaption;
        d["XY.LineHeight.Auxiliary"] = XyuiTypographyTokens.LineHeightAuxiliary;
        d["XY.LineHeight.Body"] = XyuiTypographyTokens.LineHeightBody;
        d["XY.LineHeight.Label"] = XyuiTypographyTokens.LineHeightLabel;
        d["XY.LineHeight.Section"] = XyuiTypographyTokens.LineHeightSection;
        d["XY.LineHeight.PanelTitle"] = XyuiTypographyTokens.LineHeightPanelTitle;
        d["XY.LineHeight.PageTitle"] = XyuiTypographyTokens.LineHeightPageTitle;
        d["XY.LineHeight.Mono"] = XyuiTypographyTokens.LineHeightMono;
        d["XY.LetterSpacing.Body"] = XyuiTypographyTokens.LetterSpacingBody;
        d["XY.LetterSpacing.Label"] = XyuiTypographyTokens.LetterSpacingLabel;
        d["XY.LetterSpacing.Title"] = XyuiTypographyTokens.LetterSpacingTitle;
        d["XY.LetterSpacing.Caps"] = XyuiTypographyTokens.LetterSpacingCaps;
        d["XY.LetterSpacing.Mono"] = XyuiTypographyTokens.LetterSpacingMono;
        return d;
    }
}

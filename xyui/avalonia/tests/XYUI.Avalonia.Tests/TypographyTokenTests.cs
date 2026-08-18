using System.Globalization;
using System.Text.Json;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

// Canonical 对照：Typography token 常量必须与 token-canonical-map.json 逐条一致
public class TypographyTokenTests
{
    static readonly string MapPath = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
        "tokens", "architecture", "token-canonical-map.json");

    static string MapValue(string tokenId)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(MapPath));
        var e = doc.RootElement.GetProperty("entries").EnumerateArray()
            .First(x => x.GetProperty("canonical_token_id").GetString() == tokenId);
        return e.GetProperty("value").GetString()!;
    }

    static double Dim(string v) => double.Parse(v.Replace(" DIP", ""), CultureInfo.InvariantCulture);

    static double Line(string v) => double.Parse(v.Split('/')[1], CultureInfo.InvariantCulture);

    static double Num(string v) => double.Parse(v, CultureInfo.InvariantCulture);

    [Fact]
    public void Font_Family_Matches_Canonical()
    {
        Assert.Equal(XyuiTypographyTokens.FontUi, MapValue("XY.Font.UI"));
        Assert.Equal(XyuiTypographyTokens.FontMono, MapValue("XY.Font.Mono"));
        Assert.Equal(XyuiTypographyTokens.FontFallbackCjk, MapValue("XY.Font.Fallback.CJK"));
        Assert.Equal(XyuiTypographyTokens.FontFallbackMono, MapValue("XY.Font.Fallback.Mono"));
        Assert.Equal(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontDefault);
        Assert.Equal(XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontTechnical);
    }

    [Fact]
    public void Font_Size_Matches_Canonical()
    {
        Assert.Equal(XyuiTypographyTokens.FontSizeCaption, Dim(MapValue("XY.FontSize.Caption")));
        Assert.Equal(XyuiTypographyTokens.FontSizeAuxiliary, Dim(MapValue("XY.FontSize.Auxiliary")));
        Assert.Equal(XyuiTypographyTokens.FontSizeBody, Dim(MapValue("XY.FontSize.Body")));
        Assert.Equal(XyuiTypographyTokens.FontSizeLabel, Dim(MapValue("XY.FontSize.Label")));
        Assert.Equal(XyuiTypographyTokens.FontSizeSection, Dim(MapValue("XY.FontSize.Section")));
        Assert.Equal(XyuiTypographyTokens.FontSizePanelTitle, Dim(MapValue("XY.FontSize.PanelTitle")));
        Assert.Equal(XyuiTypographyTokens.FontSizePageTitle, Dim(MapValue("XY.FontSize.PageTitle")));
        Assert.Equal(XyuiTypographyTokens.FontSizeMono, Dim(MapValue("XY.FontSize.Mono")));
    }

    [Fact]
    public void Font_Weight_Matches_Canonical()
    {
        Assert.Equal(XyuiTypographyTokens.WeightRegular, Num(MapValue("XY.FontWeight.Regular")));
        Assert.Equal(XyuiTypographyTokens.WeightMedium, Num(MapValue("XY.FontWeight.Medium")));
        Assert.Equal(XyuiTypographyTokens.WeightSemibold, Num(MapValue("XY.FontWeight.Semibold")));
        Assert.Equal(XyuiTypographyTokens.WeightBold, Num(MapValue("XY.FontWeight.Bold")));
    }

    [Fact]
    public void Line_Height_Matches_Canonical()
    {
        Assert.Equal(XyuiTypographyTokens.LineHeightCaption, Line(MapValue("XY.LineHeight.Caption")));
        Assert.Equal(XyuiTypographyTokens.LineHeightBody, Line(MapValue("XY.LineHeight.Body")));
        Assert.Equal(XyuiTypographyTokens.LineHeightLabel, Line(MapValue("XY.LineHeight.Label")));
        Assert.Equal(XyuiTypographyTokens.LineHeightSection, Line(MapValue("XY.LineHeight.Section")));
        Assert.Equal(XyuiTypographyTokens.LineHeightPanelTitle, Line(MapValue("XY.LineHeight.PanelTitle")));
        Assert.Equal(XyuiTypographyTokens.LineHeightPageTitle, Line(MapValue("XY.LineHeight.PageTitle")));
        Assert.Equal(XyuiTypographyTokens.LineHeightMono, Line(MapValue("XY.LineHeight.Mono")));
    }

    [Fact]
    public void Letter_Spacing_Matches_Canonical()
    {
        Assert.Equal(XyuiTypographyTokens.LetterSpacingBody, Num(MapValue("XY.LetterSpacing.Body")));
        Assert.Equal(XyuiTypographyTokens.LetterSpacingLabel, Num(MapValue("XY.LetterSpacing.Label")));
        Assert.Equal(XyuiTypographyTokens.LetterSpacingTitle, Num(MapValue("XY.LetterSpacing.Title")));
        Assert.Equal(XyuiTypographyTokens.LetterSpacingCaps, Num(MapValue("XY.LetterSpacing.Caps")));
        Assert.Equal(XyuiTypographyTokens.LetterSpacingMono, Num(MapValue("XY.LetterSpacing.Mono")));
    }
}

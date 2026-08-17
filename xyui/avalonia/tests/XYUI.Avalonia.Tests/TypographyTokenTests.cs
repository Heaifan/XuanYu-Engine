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
        Assert.Equal(MapValue("XY.Font.UI"), XyuiTypographyTokens.FontUi);
        Assert.Equal(MapValue("XY.Font.Mono"), XyuiTypographyTokens.FontMono);
        Assert.Equal(MapValue("XY.Font.Fallback.CJK"), XyuiTypographyTokens.FontFallbackCjk);
        Assert.Equal(MapValue("XY.Font.Fallback.Mono"), XyuiTypographyTokens.FontFallbackMono);
        Assert.Equal(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontDefault);
        Assert.Equal(XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontTechnical);
    }

    [Fact]
    public void Font_Size_Matches_Canonical()
    {
        Assert.Equal(Dim(MapValue("XY.FontSize.Caption")), XyuiTypographyTokens.FontSizeCaption);
        Assert.Equal(Dim(MapValue("XY.FontSize.Auxiliary")), XyuiTypographyTokens.FontSizeAuxiliary);
        Assert.Equal(Dim(MapValue("XY.FontSize.Body")), XyuiTypographyTokens.FontSizeBody);
        Assert.Equal(Dim(MapValue("XY.FontSize.Label")), XyuiTypographyTokens.FontSizeLabel);
        Assert.Equal(Dim(MapValue("XY.FontSize.Section")), XyuiTypographyTokens.FontSizeSection);
        Assert.Equal(Dim(MapValue("XY.FontSize.PanelTitle")), XyuiTypographyTokens.FontSizePanelTitle);
        Assert.Equal(Dim(MapValue("XY.FontSize.PageTitle")), XyuiTypographyTokens.FontSizePageTitle);
        Assert.Equal(Dim(MapValue("XY.FontSize.Mono")), XyuiTypographyTokens.FontSizeMono);
    }

    [Fact]
    public void Font_Weight_Matches_Canonical()
    {
        Assert.Equal(Num(MapValue("XY.FontWeight.Regular")), XyuiTypographyTokens.WeightRegular);
        Assert.Equal(Num(MapValue("XY.FontWeight.Medium")), XyuiTypographyTokens.WeightMedium);
        Assert.Equal(Num(MapValue("XY.FontWeight.Semibold")), XyuiTypographyTokens.WeightSemibold);
        Assert.Equal(Num(MapValue("XY.FontWeight.Bold")), XyuiTypographyTokens.WeightBold);
    }

    [Fact]
    public void Line_Height_Matches_Canonical()
    {
        Assert.Equal(Line(MapValue("XY.LineHeight.Caption")), XyuiTypographyTokens.LineHeightCaption);
        Assert.Equal(Line(MapValue("XY.LineHeight.Body")), XyuiTypographyTokens.LineHeightBody);
        Assert.Equal(Line(MapValue("XY.LineHeight.Label")), XyuiTypographyTokens.LineHeightLabel);
        Assert.Equal(Line(MapValue("XY.LineHeight.Section")), XyuiTypographyTokens.LineHeightSection);
        Assert.Equal(Line(MapValue("XY.LineHeight.PanelTitle")), XyuiTypographyTokens.LineHeightPanelTitle);
        Assert.Equal(Line(MapValue("XY.LineHeight.PageTitle")), XyuiTypographyTokens.LineHeightPageTitle);
        Assert.Equal(Line(MapValue("XY.LineHeight.Mono")), XyuiTypographyTokens.LineHeightMono);
    }

    [Fact]
    public void Letter_Spacing_Matches_Canonical()
    {
        Assert.Equal(Num(MapValue("XY.LetterSpacing.Body")), XyuiTypographyTokens.LetterSpacingBody);
        Assert.Equal(Num(MapValue("XY.LetterSpacing.Label")), XyuiTypographyTokens.LetterSpacingLabel);
        Assert.Equal(Num(MapValue("XY.LetterSpacing.Title")), XyuiTypographyTokens.LetterSpacingTitle);
        Assert.Equal(Num(MapValue("XY.LetterSpacing.Caps")), XyuiTypographyTokens.LetterSpacingCaps);
        Assert.Equal(Num(MapValue("XY.LetterSpacing.Mono")), XyuiTypographyTokens.LetterSpacingMono);
    }
}

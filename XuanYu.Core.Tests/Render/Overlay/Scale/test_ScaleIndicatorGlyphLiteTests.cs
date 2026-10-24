using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Overlay;

public sealed class ScaleIndicatorGlyphLiteTests
{
    [Fact]
    public void Metric_labels_are_encoded_without_font_dependencies()
    {
        Span<int> glyphs = stackalloc int[ScaleIndicatorGlyphLite.MaxGlyphs];
        var length = ScaleIndicatorGlyphLite.EncodeLabel("1.5 km", glyphs);

        Assert.Equal(6, length);
        Assert.Equal([1, 12, 5, 13, 11, 10, 13, 13], glyphs.ToArray());
    }

    [Theory]
    [InlineData('0', 0)]
    [InlineData('9', 9)]
    [InlineData('m', 10)]
    [InlineData('k', 11)]
    [InlineData('.', 12)]
    [InlineData(' ', 13)]
    public void Supported_glyphs_have_stable_codes(char glyph, int code) =>
        Assert.Equal(code, ScaleIndicatorGlyphLite.Encode(glyph));
}

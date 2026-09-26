using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class MapLabelRasterizationAlphaRegressionTests : IClassFixture<UiHeadlessFixture>
{
    readonly UiHeadlessFixture _fixture;

    public MapLabelRasterizationAlphaRegressionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Cjk_label_keeps_transparent_pixels_outside_glyphs()
    {
        var bitmap = _fixture.Run(() => MapLabelRasterizer.Rasterize(
            new("区域1", "alpha-test", default, 13, 1, new(.9, .9, .9, 1))));
        var alpha = bitmap.Pixels.Where((_, index) => index % 4 == 3);
        Assert.Contains((byte)0, alpha);
        Assert.Contains(alpha, value => value > 0);
        Assert.Equal(RenderLabelPixelFormat.Bgra8888Unpremultiplied, bitmap.PixelFormat);
    }
}

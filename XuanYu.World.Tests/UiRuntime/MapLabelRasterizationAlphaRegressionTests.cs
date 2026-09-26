using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class MapLabelRasterizationAlphaRegressionTests : IClassFixture<UiHeadlessFixture>
{
    readonly UiHeadlessFixture _fixture;

    public MapLabelRasterizationAlphaRegressionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Cjk_label_is_a_bounded_grayscale_coverage_mask()
    {
        var bitmap = _fixture.Run(() => MapLabelRasterizer.Rasterize(
            new("区域1", "alpha-test", default, 13, 1, new(.9, .9, .9, 1))));
        var coverage = Enumerable.Range(0, bitmap.Width * bitmap.Height)
            .Select(index => bitmap.Pixels[index * 4 + 2]).ToArray();
        Assert.Equal(0, coverage[0]);
        Assert.Equal(0, coverage[bitmap.Width - 1]);
        Assert.Equal(0, coverage[^bitmap.Width]);
        Assert.Equal(0, coverage[^1]);
        Assert.Contains(coverage, value => value > 0);
        Assert.Contains(coverage, value => value > 0 && value < 255);
        Assert.True(coverage.Count(value => value > 0) < coverage.Length);
        Assert.All(Enumerable.Range(0, coverage.Length), index =>
        {
            Assert.Equal(bitmap.Pixels[index * 4 + 2], bitmap.Pixels[index * 4]);
            Assert.Equal(bitmap.Pixels[index * 4 + 2], bitmap.Pixels[index * 4 + 1]);
        });
        Assert.Equal(RenderLabelPixelFormat.Bgra8888Unpremultiplied, bitmap.PixelFormat);
    }
}

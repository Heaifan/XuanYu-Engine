using XuanYu.Render.Abstractions;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class MapLabelRasterizerTests : IClassFixture<UiHeadlessFixture>
{
    readonly UiHeadlessFixture _fixture;

    public MapLabelRasterizerTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Rasterizer_returns_real_bgra_pixels_for_cjk()
    {
        var label = Label("台湾北部", 1);
        var bitmap = _fixture.Run(() => MapLabelRasterizer.Rasterize(label));
        Assert.Equal(RenderLabelPixelFormat.Bgra8888Unpremultiplied, bitmap.PixelFormat);
        Assert.True(bitmap.Width > 0 && bitmap.Height > 0);
        Assert.Equal(bitmap.Stride * bitmap.Height, bitmap.Pixels.Length);
        Assert.Contains(bitmap.Pixels, pixel => pixel != 0);
    }

    [Fact]
    public void Cache_hits_without_camera_and_invalidates_text_or_dpi()
    {
        var cache = new MapLabelBitmapCache();
        var first = _fixture.Run(() => cache.GetOrCreate(Label("区域1", 1)));
        var second = _fixture.Run(() => cache.GetOrCreate(Label("区域1", 1)));
        var changed = _fixture.Run(() => cache.GetOrCreate(Label("台湾北部", 1.5)));
        Assert.Same(first, second);
        Assert.NotSame(first, changed);
        Assert.Equal(2, cache.Count);
    }

    static RenderVectorOverlayLabel Label(string text, double dpi) => new(
        text, $"test|{text}|13|{dpi:0.###}", default, 13, dpi, new(.9, .9, .9, 1));
}

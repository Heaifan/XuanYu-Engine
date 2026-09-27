using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4TypographyGalleryContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4TypographyGalleryContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Hover_gallery_captions_use_shared_vertical_layout() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.01");
        var window = XyuiBatchTestHost.Show(preview);
        Assert.Equal(4, preview.GetVisualDescendants().OfType<XYCaption>().Count());
        Assert.All(preview.GetVisualDescendants().OfType<XYCaption>(), x => Assert.IsType<TranslateTransform>(x.RenderTransform));
        window.Close();
    });
}

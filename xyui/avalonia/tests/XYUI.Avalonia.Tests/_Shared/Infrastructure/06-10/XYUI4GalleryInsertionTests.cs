using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4GalleryInsertionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fixture;
    public XYUI4GalleryInsertionTests(XyuiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Insertion_gallery_reserves_anchor_space_and_draws_indicator_above_content() => _fixture.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.13");
        var rows = preview.GetVisualDescendants().OfType<Border>().Where(x => x.Width == 300).ToArray();
        Assert.Equal(3, rows.Length);
        foreach (var row in rows)
        {
            var grid = Assert.IsType<Grid>(row.Child);
            var indicator = Assert.IsType<XYInsertionIndicator>(grid.Children[^1]);
            var text = Assert.Single(grid.Children.OfType<TextBlock>());
            Assert.True(text.Margin.Left >= 10);
            Assert.Same(indicator, grid.Children[^1]);
        }
    });
}

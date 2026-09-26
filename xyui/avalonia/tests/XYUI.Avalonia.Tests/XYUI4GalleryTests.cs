using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;
using XYUI.Avalonia.Controls;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4GalleryTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4GalleryTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Gallery_registers_xyui4_components_and_routes_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var vm = new XYUI1DocumentationViewModel();
        Assert.Equal("3/3", vm.XYUI4CountText);
        Assert.Equal("XYUI-4-4.14", vm.XYUI4Items[0].Id);
        Assert.Equal("XYLoadingIndicator", vm.XYUI4Items[0].CanonicalName);
        Assert.Equal("XYSpinner", vm.XYUI4Items[1].CanonicalName);
        vm.Select("XYUI-4-4.15");
        Assert.Equal("XYUI-4-4.15", vm.SelectedXYUI4Item?.Id);
        Assert.True(vm.IsXYUI4Expanded);
        Assert.IsType<XYUI1ComponentDocumentView>(vm.SelectedDocument);
    });

    [Fact]
    public void ProgressBar_gallery_exposes_four_visual_forms() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.16");
        var bars = preview.GetVisualDescendants().OfType<XYProgressBar>().ToArray();
        Assert.True(bars.Length >= 7);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.Labeled);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.SegmentedStage);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.InlineCompact);
        Assert.IsType<XYProgressBar>(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.16")
            .GetVisualDescendants().OfType<XYProgressBar>().First());
    });
}

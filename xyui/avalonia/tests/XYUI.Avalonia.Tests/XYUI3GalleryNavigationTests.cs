using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3GalleryNavigationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3GalleryNavigationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void XYUI3_sidebar_count_and_default_follow_latest_catalog_entry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel(); Assert.Equal(25, vm.XYUI3Items.Count(x => x.Document is not null)); Assert.Equal("25/25", vm.XYUI3CountText); Assert.Equal(XYUI3GalleryCatalog.ContextToolbarId, vm.SelectedXYUI3Item?.Id);
    });
}

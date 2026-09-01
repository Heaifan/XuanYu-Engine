using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3GalleryNavigationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3GalleryNavigationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void XYUI3_sidebar_count_and_default_follow_latest_catalog_entry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel(); Assert.Equal(20, vm.XYUI3Items.Count); Assert.Equal("20/20", vm.XYUI3CountText); Assert.Equal("XYUI-3-3.20", vm.SelectedXYUI3Item?.Id);
    });
}

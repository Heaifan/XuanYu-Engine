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

    [Fact] public void XYUI3_navigation_uses_numbered_consistent_labels() => _fx.Run(() =>
    {
        var vm = new XYUI1DocumentationViewModel();
        var context = vm.XYUI3Items.Single(x => x.Id == XYUI3GalleryCatalog.ContextToolbarId);
        Assert.Equal("上下文工具栏", context.ChineseName);
        Assert.Equal("Context Toolbar", context.CanonicalName);
        Assert.Equal("3.17~3.22", context.NavigationNumber);
        Assert.Equal("3.17~3.22 · 上下文工具栏", context.DisplayChineseName);
        Assert.Equal("XYUI-3-3.17~3.22", context.Document!.DisplayId);
        Assert.Null(vm.XYUI3Items[0].NavigationNumber);
        Assert.All(vm.XYUI3Items.Skip(1), item => Assert.NotNull(item.NavigationNumber));
    });
}

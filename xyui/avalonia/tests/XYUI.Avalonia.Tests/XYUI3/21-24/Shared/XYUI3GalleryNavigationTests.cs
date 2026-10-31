using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3GalleryNavigationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3GalleryNavigationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Gallery_default_follows_latest_catalog_entry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel(); Assert.Equal(25, vm.XYUI3Items.Count(x => x.Document is not null)); Assert.Equal("25/25", vm.XYUI3CountText); Assert.Equal(XYUI4DocumentationCatalog.LatestComponentId, vm.SelectedXYUI4Item?.Id);
    });

    [Fact] public void XYUI3_navigation_uses_numbered_consistent_labels() => _fx.Run(() =>
    {
        var vm = new XYUI1DocumentationViewModel();
        var context = vm.XYUI3Items.Single(x => x.Id == XYUI3GalleryCatalog.ContextToolbarId);
        Assert.Equal("上下文工具栏", context.ChineseName);
        Assert.Equal("Context Toolbar", context.CanonicalName);
        Assert.Equal("3.25", context.NavigationNumber);
        Assert.Equal("3.25 · 上下文工具栏", context.DisplayChineseName);
        Assert.Equal("XYUI-3-3.25", context.Document!.DisplayId);
        Assert.Null(vm.XYUI3Items[0].NavigationNumber);
        Assert.All(vm.XYUI3Items.Skip(1), item => Assert.NotNull(item.NavigationNumber));
    });

    [Fact] public void Foundation_navigation_can_collapse_and_reopens_on_selection() => _fx.Run(() =>
    {
        var vm = new XYUI1DocumentationViewModel();
        Assert.False(vm.IsFoundationExpanded);
        Assert.Equal("0.01 · 色彩", vm.FoundationItems[0].DisplayChineseName);
        Assert.Equal("0.13 · 组合模板", vm.FoundationItems[^1].DisplayChineseName);
        vm.IsFoundationExpanded = false;
        Assert.False(vm.IsFoundationExpanded);
        vm.SelectFoundation("shape");
        Assert.True(vm.IsFoundationExpanded);
        Assert.Equal("shape", vm.SelectedFoundation?.Id);
    });
}

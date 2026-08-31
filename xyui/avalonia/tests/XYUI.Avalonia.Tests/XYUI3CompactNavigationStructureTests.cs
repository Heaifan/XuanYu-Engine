using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3CompactNavigationStructureTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3CompactNavigationStructureTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void TabBar_contains_real_XYTabs() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var bar = Assert.IsType<XYTabBar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.09"));
        Assert.IsType<XYTabs>(bar.Tabs); Assert.Equal(5, bar.Tabs.Items.Count); Assert.All(bar.Tabs.Items, item => Assert.IsType<XYTab>(item));
    });

    [Fact] public void DockTab_reuses_XYTab() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var dock = Assert.IsType<XYDockTabs>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.10"));
        Assert.All(dock.Items, item => Assert.IsType<XYTab>(item.Tab)); Assert.All(dock.Items, item => Assert.Contains(item.Tab, item.GetVisualDescendants().OfType<XYTab>()));
    });

    [Fact] public void Breadcrumb_uses_vectors_and_current_collapsed_states() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var breadcrumb = Assert.IsType<XYBreadcrumb>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.11"));
        Assert.Contains(breadcrumb.GetVisualDescendants().OfType<XYIcon>(), x => x.Icon == XyuiVectorIcon.ChevronRight);
        Assert.Single(breadcrumb.Items, x => x.IsCurrent); Assert.Single(breadcrumb.Items, x => x.IsCollapsed);
    });

    [Fact] public void TreeNavigation_has_compact_guides_and_selected_semantics() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var tree = Assert.IsType<XYTreeNavigation>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.12"));
        Assert.Equal(28, XyuiCompactNavigationTokens.TreeRowHeight); Assert.Contains(tree.GetVisualDescendants().OfType<XYIcon>(), x => x.Icon == XyuiVectorIcon.ChevronRight);
        Assert.Contains(tree.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-tree-guide-default"));
        Assert.Contains(tree.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-tree-guide-active"));
        Assert.Single(tree.Items, x => x.IsSelected); Assert.Contains("xyui-tree-selected", tree.Items.Single(x => x.IsSelected).Classes);
    });
}

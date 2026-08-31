using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
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

    [Fact] public void TabBar_centers_labels_and_exposes_working_actions() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var bar = new XYTabBar(Enumerable.Range(1, 8).Select(x => new XYTab { Label = $"页签-{x}-较长名称" }).ToArray()) { Width = 260 };
        var window = XyuiBatchTestHost.Show(bar); Assert.All(bar.Tabs.Items, tab => Assert.Equal(VerticalAlignment.Center, tab.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-tab-label")).VerticalAlignment));
        Assert.Equal(new Thickness(1, 1, 1, 0), bar.BorderThickness);
        var created = 0; bar.NewRequested += (_, _) => created++; bar.NewButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, created);
        bar.ScrollBy(120); Assert.True(bar.HorizontalOffset > 0); bar.OverflowButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.True(bar.OverflowPopup.IsOpen); window.Close();
    });

    [Fact] public void DockTabs_have_one_accent_and_manage_selection_close_order() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var dock = Assert.IsType<XYDockTabs>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.10")); var window = XyuiBatchTestHost.Show(dock);
        Assert.All(dock.Items, item => Assert.False(item.Tab.ShowSelectedAccent));
        Assert.DoesNotContain(dock.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-tab-accent") && x.IsVisible);
        Assert.Single(dock.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-dock-accent") && x.IsVisible);
        var first = dock.Items[0]; dock.Select(first.Tab); Assert.True(first.Tab.IsSelected); var last = dock.Items[^1]; dock.Move(last, 0); Assert.Same(last, dock.Items[0]);
        dock.Close(last); Assert.DoesNotContain(last, dock.Items); window.Close();
    });

    [Fact] public void Breadcrumb_routes_navigation_and_dropdown_requests() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var breadcrumb = Assert.IsType<XYBreadcrumb>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.11"));
        var changed = 0; var dropdown = 0; breadcrumb.CurrentChanged += (_, _) => changed++; breadcrumb.DropdownRequested += (_, _) => dropdown++;
        breadcrumb.Items[0].Invoke(); Assert.True(breadcrumb.Items[0].IsCurrent); Assert.Equal(1, changed);
        breadcrumb.Items.Single(x => x.IsCollapsed).Invoke(); Assert.Equal(1, dropdown);
    });

    [Fact] public void TreeNavigation_collapses_descendants_and_keeps_single_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var tree = Assert.IsType<XYTreeNavigation>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.12"));
        var root = tree.Items[0]; Assert.True(tree.VisibleItems.Count > 1); root.ToggleExpansion(); Assert.Single(tree.VisibleItems);
        root.ToggleExpansion(); var target = tree.Items[1]; tree.Select(target); Assert.Same(target, tree.Items.Single(x => x.IsSelected));
    });
}

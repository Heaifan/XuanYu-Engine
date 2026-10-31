using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Round2HeadlessContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3Round2HeadlessContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void NavigationRail_XamlItemsShareNavigationState() => _fx.Run(() => { var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code)], "map"); var rail = new XYNavigationRail { NavigationState = state }; Assert.Equal(2, rail.Items.Count); Assert.Same(state, rail.NavigationState); Assert.Equal("map", state.CurrentDestinationId); });
    [Fact] public void NavigationRail_DisabledAndContextContract() => _fx.Run(() => { var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("locked", "锁定", XyuiVectorIcon.Info, IsEnabled: false)], "map"); var rail = new XYNavigationRail(state, new Dictionary<string, IReadOnlyList<XYNavigationEntry>> { ["map"] = [new("base", "基础", XyuiVectorIcon.Section)] }); Assert.False(state.RequestNavigation("locked")); Assert.Single(rail.ContextItems); });
    [Fact] public void NavigationRail_ExpandTrigger_Raises_Request_In_Headless() => _fx.Run(() => { var rail = new XYNavigationRail(new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate)]), new Dictionary<string, IReadOnlyList<XYNavigationEntry>>(), showExpandButton: true); var requested = 0; rail.ExpandRequested += (_, _) => requested++; Assert.Contains(rail.GetVisualDescendants().OfType<XYIconButton>(), x => x.Classes.Contains("xyui-rail-expand")); rail.GetVisualDescendants().OfType<XYIconButton>().Single(x => x.Classes.Contains("xyui-rail-expand")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, requested); });
    [Fact] public void Tabs_CanonicalSelectionAndStableModifiedSlot() => _fx.Run(() => { var tabs = new XYTabs(new XYTab { Id = "a", Label = "A" }, new XYTab { Id = "b", Label = "B", IsModified = true }); tabs.Select("b"); Assert.Equal("b", tabs.SelectedTabId); Assert.Same(tabs.Items[1], tabs.SelectedItem); Assert.Contains(tabs.Items[1].GetVisualDescendants().OfType<XYIcon>(), x => !x.IsVisible); });
    [Fact] public void Tabs_DisabledCannotBecomeSelected() => _fx.Run(() => { var tabs = new XYTabs(new XYTab { Id = "a", Label = "A" }, new XYTab { Id = "b", Label = "B", IsEnabled = false }); tabs.Select("b"); Assert.Equal("a", tabs.SelectedTabId); });
    [Fact] public void Tabs_keyboard_navigation_uses_only_enabled_items() => _fx.Run(() =>
    {
        var tabs = new XYTabs(new XYTab { Id = "a", Label = "A" }, new XYTab { Id = "b", Label = "B", IsEnabled = false }, new XYTab { Id = "c", Label = "C" });
        tabs.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Right });
        Assert.Equal("c", tabs.SelectedTabId);
        tabs.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Home });
        Assert.Equal("a", tabs.SelectedTabId);
        tabs.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.End });
        Assert.Equal("c", tabs.SelectedTabId);
    });
    [Fact] public void TabBar_DefaultCollectionAndSelectionContract() => _fx.Run(() => { var bar = new XYTabBar(); bar.Items.Add(new XYTab { Id = "a", Label = "A" }); bar.Items.Add(new XYTab { Id = "b", Label = "B" }); bar.SelectedTabId = "b"; Assert.Equal("b", bar.SelectedTabId); Assert.Same(bar.Items, bar.Tabs.Items); });
    [Fact] public void DockTabs_ActiveIdCloseReorderAndHandoff() => _fx.Run(() => { var first = new XYDockTab(new XYTab { Id = "a", Label = "A", IsSelected = true }); var second = new XYDockTab(new XYTab { Id = "b", Label = "B" }); var dock = new XYDockTabs(first, second); XYDockHandoffRequest? handoff = null; dock.DockHandoffRequested += (_, request) => handoff = request; dock.Select("b"); Assert.Equal("b", dock.ActiveTabId); dock.Move(second, 0); Assert.Same(second, dock.Items[0]); Assert.Equal("b", handoff!.TabId); dock.Close(second); Assert.Single(dock.Items); Assert.Equal("a", dock.ActiveTabId); });
    [Fact] public void Breadcrumb_CurrentDoesNotNavigateToItself() => _fx.Run(() => { var current = new XYBreadcrumbItem { Label = "当前", IsCurrent = true }; var breadcrumb = new XYBreadcrumb(new XYBreadcrumbItem { Label = "祖先" }, current); var requested = 0; breadcrumb.NavigationRequested += (_, _) => requested++; breadcrumb.Navigate(current); Assert.Equal(0, requested); breadcrumb.Navigate(breadcrumb.Items[0]); Assert.Equal(1, requested); });
    [Fact] public void TreeNavigation_ChildrenDeriveHierarchyAndBadgeStatus() => _fx.Run(() => { var child = new XYTreeNode { Id = "child", Label = "子节点", Badge = "3", Status = XyuiStatusState.Warning }; var root = new XYTreeNode { Id = "root", Label = "根", IsExpanded = true }; root.Children.Add(child); var tree = new XYTreeNavigation(root); Assert.Contains(child, tree.VisibleItems); Assert.Equal(1, child.Depth); Assert.Contains(child.GetVisualDescendants().OfType<XYStatusBadge>(), x => x.Text == "3"); root.ToggleExpansion(); Assert.DoesNotContain(child, tree.VisibleItems); Assert.Same(child, tree.SelectedNode ?? child); });
    [Fact] public void TreeNavigation_DisabledCannotSelectAndExpandIsSeparate() => _fx.Run(() => { var root = new XYTreeNode { Label = "根", IsExpanded = false }; var child = new XYTreeNode { Label = "子", IsEnabled = false }; root.Children.Add(child); var tree = new XYTreeNavigation(root); root.ToggleExpansion(); Assert.False(root.IsSelected); Assert.False(child.IsSelected); tree.Select(child); Assert.False(child.IsSelected); });
}

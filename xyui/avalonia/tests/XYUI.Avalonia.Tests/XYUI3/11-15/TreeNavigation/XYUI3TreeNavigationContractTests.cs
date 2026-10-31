using Avalonia.Metadata;
using Avalonia.Input;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3TreeNavigationContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3TreeNavigationContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void TreeNavigation_XamlNestedChildren_Loads() => _fx.Run(() => { var root = new XYTreeNode { Label = "地图系统", IsExpanded = true }; root.Children.Add(new XYTreeNode { Label = "基础要素" }); var tree = new XYTreeNavigation { Items = { root } }; Assert.NotNull(typeof(XYTreeNode).GetProperty(nameof(XYTreeNode.Children))!.GetCustomAttributes(typeof(ContentAttribute), true).SingleOrDefault()); Assert.Contains(root.Children[0], tree.VisibleItems); });
    [Fact] public void TreeNavigation_Parent_IsDerived() => _fx.Run(() => { var root = new XYTreeNode { IsExpanded = true }; var child = new XYTreeNode(); root.Children.Add(child); var tree = new XYTreeNavigation(root); tree.Focus(child); child.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Left }); Assert.Same(root, tree.FocusedNode); });
    [Fact] public void TreeNavigation_Depth_IsDerived() => _fx.Run(() => { var root = new XYTreeNode { IsExpanded = true }; var child = new XYTreeNode { IsExpanded = true }; var leaf = new XYTreeNode(); child.Children.Add(leaf); root.Children.Add(child); _ = new XYTreeNavigation(root); Assert.Equal(0, root.Depth); Assert.Equal(1, child.Depth); Assert.Equal(2, leaf.Depth); });
    [Fact] public void TreeNavigation_HasChildren_IsDerived() => _fx.Run(() => { var root = new XYTreeNode(); Assert.False(root.HasChildren); root.Children.Add(new XYTreeNode()); Assert.True(root.HasChildren); root.Children.Clear(); Assert.False(root.HasChildren); });
    [Fact] public void TreeNavigation_Disabled_CannotSelect() => _fx.Run(() => { var node = new XYTreeNode { IsEnabled = false }; var tree = new XYTreeNavigation(node); tree.Select(node); node.ToggleExpansion(); Assert.False(node.IsSelected); Assert.False(node.IsExpanded); });
    [Fact] public void TreeNavigation_Badge_IsRendered() => _fx.Run(() => { var node = new XYTreeNode { Badge = "3", Status = XyuiStatusState.Warning }; _ = new XYTreeNavigation(node); var badge = node.GetVisualDescendants().OfType<XYStatusBadge>().Single(); Assert.Equal("3", badge.Text); Assert.Equal(XyuiStatusState.Warning, badge.State); });
    [Fact] public void TreeNavigation_Status_IsRendered() => _fx.Run(() => { var node = new XYTreeNode { Badge = "!", Status = XyuiStatusState.Error }; _ = new XYTreeNavigation(node); Assert.Contains("xyui-tree-badge", node.GetVisualDescendants().OfType<XYStatusBadge>().Single().Classes); });
    [Fact] public void TreeNavigation_Collapse_PreservesSelectedChild() => _fx.Run(() => { var root = new XYTreeNode { IsExpanded = true }; var child = new XYTreeNode(); root.Children.Add(child); var tree = new XYTreeNavigation(root); tree.Select(child); root.ToggleExpansion(); Assert.Same(child, tree.SelectedNode); Assert.DoesNotContain(child, tree.VisibleItems); root.ToggleExpansion(); Assert.Contains(child, tree.VisibleItems); Assert.True(child.IsSelected); });
}

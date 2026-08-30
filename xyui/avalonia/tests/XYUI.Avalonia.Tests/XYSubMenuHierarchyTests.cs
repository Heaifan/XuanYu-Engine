using Avalonia.Controls;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYSubMenuHierarchyTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYSubMenuHierarchyTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void SubMenu_child_cannot_be_visible_without_parent() => _fx.Run(() => { var t = Tree(); t.Root.Close(); Assert.False(t.Child.EffectiveVisible); Assert.False(t.Child.ChildMenu.IsVisible); });
    [Fact] public void SubMenu_grandchild_cannot_be_visible_without_child() => _fx.Run(() => { var t = Tree(); t.Child.Close(); Assert.False(t.Grandchild.EffectiveVisible); Assert.False(t.Grandchild.ChildMenu.IsVisible); });
    [Fact] public void SubMenu_open_child_keeps_parent_visible() => _fx.Run(() => { var t = Tree(); t.Root.Close(); t.Root.Open(); t.Child.Open(); Assert.True(t.Root.EffectiveVisible); Assert.True(t.Child.EffectiveVisible); });
    [Fact] public void SubMenu_open_grandchild_keeps_parent_and_child_visible() => _fx.Run(() => { var t = Tree(); t.Root.Close(); t.Root.Open(); t.Child.Open(); t.Grandchild.Open(); Assert.True(t.Root.EffectiveVisible); Assert.True(t.Child.EffectiveVisible); Assert.True(t.Grandchild.EffectiveVisible); });

    [Fact] public void SubMenu_close_parent_closes_child() => _fx.Run(() => { var t = Tree(); t.Root.Close(); Assert.False(t.Child.IsOpen); });
    [Fact] public void SubMenu_close_parent_closes_all_descendants() => _fx.Run(() => { var t = Tree(); t.Root.Close(); Assert.False(t.Root.IsOpen); Assert.False(t.Child.IsOpen); Assert.False(t.Grandchild.IsOpen); });
    [Fact] public void SubMenu_close_child_closes_grandchild() => _fx.Run(() => { var t = Tree(); t.Child.Close(); Assert.True(t.Root.IsOpen); Assert.False(t.Child.IsOpen); Assert.False(t.Grandchild.IsOpen); });
    [Fact] public void SubMenu_close_grandchild_does_not_close_parent() => _fx.Run(() => { var t = Tree(); t.Grandchild.Close(); Assert.True(t.Root.IsOpen); Assert.True(t.Child.IsOpen); Assert.False(t.Grandchild.IsOpen); });
    [Fact] public void SubMenu_close_grandchild_does_not_close_child() => _fx.Run(() => { var t = Tree(); t.Grandchild.Close(); Assert.True(t.Child.IsOpen); });

    [Fact] public void SubMenu_connector_visible_only_when_parent_and_child_visible() => _fx.Run(() => { var t = Tree(); Assert.True(t.Root.Connector.IsVisible); t.Root.Close(); Assert.False(t.Root.Connector.IsVisible); });
    [Fact] public void SubMenu_connector_hidden_when_child_closed() => _fx.Run(() => { var t = Tree(); t.Child.Close(); Assert.True(t.Root.Connector.IsVisible); Assert.False(t.Child.Connector.IsVisible); });
    [Fact] public void SubMenu_connector_hidden_when_parent_closed() => _fx.Run(() => { var t = Tree(); t.Root.Close(); Assert.False(t.Root.Connector.IsVisible); Assert.False(t.Child.Connector.IsVisible); });
    [Fact] public void SubMenu_grandchild_connector_requires_child_and_grandchild() => _fx.Run(() => { var t = Tree(); Assert.True(t.Grandchild.Connector.IsVisible); t.Grandchild.Close(); Assert.False(t.Grandchild.Connector.IsVisible); });
    [Fact] public void SubMenu_closing_child_hides_all_descendant_connectors() => _fx.Run(() => { var t = Tree(); t.Child.Close(); Assert.False(t.Child.Connector.IsVisible); Assert.False(t.Grandchild.Connector.IsVisible); });

    [Fact] public void SubMenu_opening_sibling_closes_previous_sibling_branch() => _fx.Run(() => { var t = Siblings(); t.First.Open(); t.Second.Open(); Assert.False(t.First.IsOpen); Assert.True(t.Second.IsOpen); });
    [Fact] public void SubMenu_opening_sibling_closes_previous_grandchildren() => _fx.Run(() => { var t = Siblings(); t.First.Open(); t.FirstGrandchild.Open(); t.Second.Open(); Assert.False(t.First.IsOpen); Assert.False(t.FirstGrandchild.IsOpen); });
    [Fact] public void SubMenu_open_left_uses_same_hierarchy_rules() => _fx.Run(() => { var t = Tree(true); t.Root.Close(); t.Root.Open(); t.Child.Open(); Assert.True(t.Child.EffectiveVisible); t.Child.Close(); Assert.False(t.Grandchild.EffectiveVisible); });
    [Fact] public void SubMenu_open_left_connector_requires_both_levels() => _fx.Run(() => { var t = Tree(true); t.Root.Close(); Assert.False(t.Root.Connector.IsVisible); t.Root.Open(); Assert.True(t.Root.Connector.IsVisible); });
    [Fact] public void SubMenu_gallery_runtime_preview_builds() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var preview = new XYSubMenuHierarchyDebugPreview(); var window = XyuiBatchTestHost.Show(preview); Dispatcher.UIThread.RunJobs(); Assert.NotNull(preview.Child); window.Close(); });

    static (XYSubMenu Root, XYSubMenu Child, XYSubMenu Grandchild) Tree(bool openLeft = false)
    {
        var root = new XYSubMenu { OpenLeft = openLeft, ParentMenu = new XYMenu(), ChildMenu = new XYMenu() };
        var child = new XYSubMenu { OpenLeft = openLeft, ParentMenu = new XYMenu(), ChildMenu = new XYMenu(), ParentSubMenu = root };
        _ = new XYSubMenu { OpenLeft = openLeft, ParentMenu = new XYMenu(), ChildMenu = new XYMenu(), ParentSubMenu = child };
        var grandchild = child.ChildSubMenus.Single(); return (root, child, grandchild);
    }
    static (XYSubMenu Root, XYSubMenu First, XYSubMenu Second, XYSubMenu FirstGrandchild) Siblings()
    {
        var root = new XYSubMenu { ParentMenu = new XYMenu(), ChildMenu = new XYMenu() };
        var first = new XYSubMenu { ParentMenu = new XYMenu(), ChildMenu = new XYMenu(), ParentSubMenu = root };
        var second = new XYSubMenu { ParentMenu = new XYMenu(), ChildMenu = new XYMenu(), ParentSubMenu = root };
        var grandchild = new XYSubMenu { ParentMenu = new XYMenu(), ChildMenu = new XYMenu(), ParentSubMenu = first };
        return (root, first, second, grandchild);
    }
}

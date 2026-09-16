using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYMenuCascadeR1Tests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYMenuCascadeR1Tests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Fixed_root_has_three_sibling_submenus() => _fx.Run(() =>
    { var t = Tree(); Assert.Equal(3, t.Root.SubMenusForTest().Count); Assert.All(t.Items, x => Assert.Same(t.Root, x.SubMenu!.ParentMenu)); });

    [Fact] public void Submenu_does_not_rehost_parent_visual() => _fx.Run(() =>
    { var t = Single(); Assert.DoesNotContain(t.Root, t.Item.SubMenu!.GetVisualDescendants()); });

    [Fact] public void Root_is_attached_once_when_hosted() => _fx.Run(() =>
    { var t = Tree(); var host = new Border { Child = t.Root }; var window = XyuiBatchTestHost.Show(host); Assert.Same(host, t.Root.GetVisualParent()); window.Close(); });

    [Fact] public void Hover_line_opens_road_child() => _fx.Run(() =>
    { var t = Tree(); var window = XyuiBatchTestHost.Show(t.Root); XyuiBatchTestHost.Hover(window, t.Line); Assert.True(t.Line.SubMenu!.IsOpen); Assert.True(t.Road.IsOpen); window.Close(); });

    [Fact] public void Invoke_line_opens_child_without_leaf_dispatch() => _fx.Run(() =>
    { var t = Tree(); var calls = 0; t.Line.Command = (Action)(() => calls++); t.Line.Activate(); Assert.Equal(0, calls); Assert.True(t.Line.SubMenu!.IsOpen); });

    [Fact] public void Click_line_opens_child_without_leaf_dispatch() => _fx.Run(() =>
    { var t = Tree(); var window = XyuiBatchTestHost.Show(t.Root); t.Root.Open(); t.Line.SubMenu!.Close(); var point = t.Line.TranslatePoint(new Point(t.Line.Bounds.Width / 2, t.Line.Bounds.Height / 2), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.True(t.Line.SubMenu.IsOpen); window.MouseUp(point, MouseButton.Left); window.Close(); });

    [Fact] public void Opening_surface_closes_line_branch() => _fx.Run(() =>
    { var t = Tree(); t.Line.SubMenu!.Open(); t.Surface.SubMenu!.Open(); Assert.False(t.Line.SubMenu.IsOpen); Assert.True(t.Surface.SubMenu.IsOpen); });

    [Fact] public void Root_close_recursively_closes_all_siblings() => _fx.Run(() =>
    { var t = Tree(); t.Root.Open(); t.Line.SubMenu!.Open(); t.Surface.SubMenu!.Open(); t.Root.Close(); Assert.All(t.Items, x => Assert.False(x.SubMenu!.IsOpen)); });

    [Fact] public void Leaf_invocation_runs_once_and_closes_branch() => _fx.Run(() =>
    { var calls = 0; var t = Tree(() => calls++); t.Root.Open(); t.Line.SubMenu!.Open(); t.Road.ChildMenu.Items.OfType<XYMenuItem>().Single().Activate(); Assert.Equal(1, calls); Assert.False(t.Line.SubMenu.IsOpen); });

    [Fact] public void FromModels_builds_three_sibling_relations() => _fx.Run(() =>
    { var root = XYMenu.FromModels(Models()); var items = root.Items.OfType<XYMenuItem>().ToArray(); Assert.Equal(3, items.Length); Assert.Equal(new[] { "地图标记", "道路", "区域面" }, items.Select(x => x.SubMenu!.ChildMenu.Items.OfType<XYMenuItem>().Single().Label)); Assert.All(items, x => Assert.Same(root, x.SubMenu!.ParentMenu)); });

    static (XYMenu Root, XYMenuItem[] Items, XYMenuItem Line, XYMenuItem Surface, XYSubMenu Road) Tree(Action? road = null)
    { var root = new XYMenu(); var point = new XYMenuItem { Label = "点" }; var line = new XYMenuItem { Label = "线" }; var surface = new XYMenuItem { Label = "面" }; root.Items = [point, line, surface]; _ = Link(root, point, "地图标记"); var roadSub = Link(root, line, "道路", road); _ = Link(root, surface, "区域面"); return (root, [point, line, surface], line, surface, roadSub); }
    static (XYMenu Root, XYMenuItem Item) Single() { var root = new XYMenu(); var item = new XYMenuItem { Label = "线" }; root.Items = [item]; Link(root, item, "道路"); return (root, item); }
    static XYSubMenu Link(XYMenu root, XYMenuItem trigger, string label, Action? action = null) { var leaf = new XYMenuItem { Label = label, Command = action }; var submenu = new XYSubMenu { ParentMenu = root, ChildMenu = new XYMenu(leaf), Trigger = trigger }; trigger.SubMenu = submenu; return submenu; }
    static IReadOnlyList<XYMenuItemModel> Models() => [new("point", "点", Children: [new("marker", "地图标记")]), new("line", "线", Children: [new("road", "道路")]), new("surface", "面", Children: [new("region", "区域面")])];
}

static class XYMenuCascadeTestExtensions
{
    public static IReadOnlyList<XYSubMenu> SubMenusForTest(this XYMenu menu) => menu.Items.OfType<XYMenuItem>().Select(x => x.SubMenu!).ToArray();
}

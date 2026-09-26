using System.Reflection;
using Avalonia.Controls;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainContextLeafHoverFixTests
{
    readonly UiHeadlessFixture _fixture;
    public TerrainContextLeafHoverFixTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Hovering_terrain_leaf_closes_region_submenu()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            var region = board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "area");
            var terrain = board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "terrain");
            InvokeHover(board.Menu, region); Assert.True(region.SubMenu!.IsOpen);
            InvokeHover(board.Menu, terrain);
            var result = (region.SubMenu.IsOpen, terrain.SubMenu);
            window.Close();
            return result;
        });

        Assert.False(state.Item1);
        Assert.Null(state.Item2);
    }

    [Fact]
    public void Hovering_point_after_region_switches_to_point_submenu()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            var region = board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "area");
            var point = board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "point");
            InvokeHover(board.Menu, region); InvokeHover(board.Menu, point);
            var result = (region.SubMenu!.IsOpen, point.SubMenu!.IsOpen);
            window.Close();
            return result;
        });

        Assert.False(state.Item1);
        Assert.True(state.Item2);
    }

    [Fact]
    public void Clicking_terrain_still_switches_context_and_closes_board()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "terrain").Activate();
            var result = (vm.IsTerrainContext, board.IsOpen);
            window.Close();
            return result;
        });

        Assert.True(state.Item1);
        Assert.False(state.Item2);
    }

    static void InvokeHover(XYMenu menu, XYMenuItem item) =>
        typeof(XYMenu).GetMethod("OnItemPointerEntered", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(menu, [item, null]);
}

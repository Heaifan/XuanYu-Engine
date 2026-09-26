using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainContextRuntimeFixTests
{
    readonly UiHeadlessFixture _fixture;
    public TerrainContextRuntimeFixTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Terrain_category_is_a_leaf_context_switch_and_closes_menu()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            var terrain = board.Menu.Items.OfType<XYMenuItem>().Single(x => x.Id == "terrain");
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            terrain.Activate(); Dispatcher.UIThread.RunJobs();
            window.Close();
            return (terrain.SubMenu, vm.IsTerrainContext, board.IsOpen);
        });

        Assert.Null(result.SubMenu);
        Assert.True(result.IsTerrainContext);
        Assert.False(result.IsOpen);
    }

    [Fact]
    public void Terrain_context_collapses_the_hidden_region_host_slot()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var top = new Top { DataContext = vm }; host.Show(top, 1200, 180);
            vm.EnterRegionContext(); Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            var edit = UiRuntimeTestHost.Descendants<EditToolsModule>(top).Single();
            var regionWidth = edit.Bounds.Width;
            vm.EnterTerrainContext(); Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            return (regionWidth, terrainWidth: edit.Bounds.Width);
        });

        Assert.True(result.regionWidth > 0);
        Assert.Equal(0, result.terrainWidth, 1);
    }
}

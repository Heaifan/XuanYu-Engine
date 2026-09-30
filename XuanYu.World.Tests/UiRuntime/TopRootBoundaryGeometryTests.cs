using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TopRootBoundaryGeometryTests
{
    readonly UiHeadlessFixture _fixture;
    public TopRootBoundaryGeometryTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Root_top_boundary_stays_stable_when_entering_region_editing()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var root = new UiRoot { DataContext = vm };
            host.Show(root, 1362, 852);
            root.UpdateLayout();
            var grid = root.FindControl<Grid>("RootGrid")!;
            var main = root.FindControl<Grid>("MainLayoutGrid")!;
            var management = (grid.RowDefinitions[0].ActualHeight, main.Bounds.Y);
            vm.ToggleEditorMode();
            vm.SwitchWorkspaceCommand.Execute("RegionEditor");
            Dispatcher.UIThread.RunJobs();
            root.UpdateLayout();
            return (management, editing: (grid.RowDefinitions[0].ActualHeight, main.Bounds.Y));
        });

        Assert.Equal(state.management.Item1, state.editing.Item1, precision: 3);
        Assert.Equal(state.management.Item2, state.editing.Item2, precision: 3);
    }
}

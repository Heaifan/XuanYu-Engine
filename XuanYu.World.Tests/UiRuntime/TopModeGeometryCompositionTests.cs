using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TopModeGeometryCompositionTests
{
    readonly UiHeadlessFixture _fixture;
    public TopModeGeometryCompositionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Top_height_stays_stable_when_entering_region_editing()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var top = new Top { DataContext = vm };
            host.Show(top, 1200, 180);
            top.UpdateLayout();
            var managementHeight = top.Bounds.Height;
            var editTools = UiRuntimeTestHost.Descendants<EditToolsModule>(top).Single();
            var editToolsRoot = editTools.FindControl<Border>("EditToolsRoot")!;
            var managementVisible = editToolsRoot.IsEffectivelyVisible;
            vm.ToggleEditorMode();
            vm.SwitchWorkspaceCommand.Execute("RegionEditor");
            Dispatcher.UIThread.RunJobs();
            top.UpdateLayout();
            return (managementHeight, editingHeight: top.Bounds.Height,
                managementVisible, editingVisible: editToolsRoot.IsEffectivelyVisible);
        });

        Assert.False(state.managementVisible);
        Assert.True(state.editingVisible);
        Assert.Equal(state.managementHeight, state.editingHeight, precision: 3);
    }
}

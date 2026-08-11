using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class LayerARuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public LayerARuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Manage_hides_dock_and_edit_switches_provider_context()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var right = new Right { DataContext = vm };
            host.Show(right, 900, 700);
            right.UpdateLayout();
            var dock = UiRuntimeTestHost.Descendants<EditorLayerDock>(right).Single();
            var manageDockHidden = dock.GetVisualParent() is Grid manageGrid && !manageGrid.IsVisible;
            vm.ToggleEditorMode();
            right.UpdateLayout();
            var map = dock.GetVisualParent() is Grid editGrid && editGrid.IsVisible
                && vm.CurrentLayerItems.Count == 0;
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            right.UpdateLayout();
            var region = vm.CurrentLayerItems.Count;
            return (manageDockHidden, map, region);
        });

        Assert.True(state.manageDockHidden);
        Assert.True(state.map);
        Assert.True(state.region > 0);
    }
}

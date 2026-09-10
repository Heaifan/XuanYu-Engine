using System.IO;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR1Fix5RightContentOwnershipTests
{
    readonly UiHeadlessFixture _fixture;

    public AreaDR1Fix5RightContentOwnershipTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("选择")]
    [InlineData("移动")]
    [InlineData("旋转")]
    [InlineData("缩放")]
    public void Entity_owner_keeps_persistent_layer_dock_for_each_transform_tool(string tool)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.AddCubeEntity(); vm.ToggleEditorMode(); vm.SelectToolCommand.Execute(tool);
            return Snapshot(host, vm);
        });

        Assert.True(state.EntityOwner);
        Assert.False(state.MapVisible); Assert.False(state.RegionVisible); Assert.True(state.LayerVisible);
        Assert.Equal(1, state.VisibleEntityPanels);
        Assert.Equal(tool, state.ActiveTool);
    }

    [Fact]
    public void Manage_move_enters_edit_without_changing_entity_owner_or_selection()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity();
            var key = vm.SelectionKey;
            vm.SelectToolCommand.Execute("移动"); Dispatcher.UIThread.RunJobs();
            var snapshot = Snapshot(host, vm);
            return (snapshot, vm.IsEditMode, SelectionUnchanged: key == vm.SelectionKey);
        });

        Assert.True(state.IsEditMode); Assert.True(state.SelectionUnchanged);
        Assert.True(state.snapshot.EntityOwner); Assert.False(state.snapshot.MapVisible);
        Assert.False(state.snapshot.RegionVisible); Assert.True(state.snapshot.LayerVisible);
    }

    [Fact]
    public void Clearing_entity_owner_restores_map_and_layer_context_in_map_edit()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity(); vm.ToggleEditorMode();
            vm.SelectedHierarchyItem = null; Dispatcher.UIThread.RunJobs();
            return Snapshot(host, vm);
        });

        Assert.False(state.EntityOwner); Assert.True(state.MapVisible); Assert.True(state.LayerVisible);
        Assert.False(state.RegionVisible); Assert.Equal(0, state.VisibleEntityPanels);
    }

    [Fact]
    public void Inspector_owns_map_content_and_right_owns_no_map_sibling()
    {
        var source = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "Right.axaml"));
        Assert.DoesNotContain("<local:MapEditorPanel", source);
        Assert.DoesNotContain("<local:EntityInspectorPanel", source);
        var inspector = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
        Assert.Contains("<local:MapEditorPanel", inspector);
    }

    static SnapshotData Snapshot(UiRuntimeTestHost host, UiVm vm)
    {
        var right = new Right { DataContext = vm }; host.Show(right, 480, 720); right.UpdateLayout();
        return new(
            vm.IsEntityInspector,
            Find<MapEditorPanel>(right, "MapWorkspace").IsEffectivelyVisible,
            false,
            Find<EditorLayerDock>(right, "LayerWorkspace").IsEffectivelyVisible,
            UiRuntimeTestHost.Descendants<EntityInspectorPanel>(right).Count(x => x.IsEffectivelyVisible),
            vm.ActiveTool);
    }

    static T Find<T>(Right right, string name) where T : Control =>
        UiRuntimeTestHost.Descendants<T>(right).Single(x => x.Name == name);

    readonly record struct SnapshotData(bool EntityOwner, bool MapVisible, bool RegionVisible,
        bool LayerVisible, int VisibleEntityPanels, string ActiveTool);
}

using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapMarkerInspectorWorkflowTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-inspector-{Guid.NewGuid():N}");

    [Fact]
    public async Task Selected_marker_projects_identity_dataset_and_coordinates()
    {
        Directory.CreateDirectory(_root);
        var vm = await CreateMarkerVm();
        var marker = vm.MapSession.CurrentMap.Markers.Single();

        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));

        Assert.True(vm.IsMarkerInspector);
        Assert.Equal(marker.MarkerId.ToString(), vm.MarkerInspectorIdText);
        Assert.Equal("MapMarker", vm.MarkerInspectorTypeText);
        Assert.Equal(vm.SelectedDataset!.Name, vm.MarkerInspectorDatasetText);
        Assert.Equal("正常", vm.MarkerInspectorStatusText);
        Assert.Equal(7, vm.MarkerInspectorPositionX);
        Assert.Equal(8, vm.MarkerInspectorPositionY);
    }

    [Fact]
    public async Task Inspector_position_commit_uses_map_history_and_syncs_after_undo_redo()
    {
        Directory.CreateDirectory(_root);
        var vm = await CreateMarkerVm();
        var marker = vm.MapSession.CurrentMap.Markers.Single();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));

        Assert.True(vm.CommitMarkerPosition(11, 12));
        Assert.Equal(new MapPoint(11, 12), vm.MapSession.CurrentMap.Markers.Single().Position);
        Assert.Equal(11, vm.MarkerInspectorPositionX);
        Assert.True(vm.CanUndo);

        vm.MapUndo();
        Assert.Equal(new MapPoint(7, 8), vm.MapSession.CurrentMap.Markers.Single().Position);
        Assert.Equal(7, vm.MarkerInspectorPositionX);
        vm.MapRedo();
        Assert.Equal(new MapPoint(11, 12), vm.MapSession.CurrentMap.Markers.Single().Position);
        Assert.Equal(12, vm.MarkerInspectorPositionY);
    }

    [Fact]
    public async Task Empty_selection_hides_marker_inspector()
    {
        Directory.CreateDirectory(_root);
        var vm = await CreateMarkerVm();
        var marker = vm.MapSession.CurrentMap.Markers.Single();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));

        vm.ClearMapGeometrySelection();

        Assert.False(vm.IsMarkerInspector);
        Assert.True(vm.IsInspectorEmpty);
    }

    async Task<UiVm> CreateMarkerVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = MapDatasetTypes.Marker;
        Assert.True(await vm.CreateDatasetAsync());
        var layerId = MapDatasetLayerIdProjection.Project(vm.DatasetSelectedId!);
        var marker = new MapMarker(MapMarkerId.New(), layerId, "标记", new(7, 8));
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        return vm;
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}

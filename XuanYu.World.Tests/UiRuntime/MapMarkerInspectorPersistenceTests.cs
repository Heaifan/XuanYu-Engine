using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapMarkerInspectorPersistenceTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-persistence-{Guid.NewGuid():N}");

    [Fact]
    public async Task Selecting_another_marker_replaces_the_inspector_identity()
    {
        var vm = await CreateMarkerVm();
        var first = vm.MapSession.CurrentMap.Markers.Single();
        var second = new MapMarker(MapMarkerId.New(), first.LayerId, "第二个标记", new(21, 22));
        Assert.True(vm.MapSession.CreateMarker(second).IsSuccess);

        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, first.MarkerId.ToString()));
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, second.MarkerId.ToString()));

        Assert.Equal(second.MarkerId.ToString(), vm.MarkerInspectorIdText);
        Assert.Equal("第二个标记", vm.InspectorSelectionTitle);
        Assert.Equal(21, vm.MarkerInspectorPositionX);
    }

    [Fact]
    public async Task Inspector_position_save_reload_preserves_the_marker()
    {
        var path = Path.Combine(_root, "map.json");
        var vm = await CreateMarkerVm();
        var marker = vm.MapSession.CurrentMap.Markers.Single();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
        Assert.True(vm.CommitMarkerPosition(31, 32));
        Assert.True(await vm.SaveMapManifestAsync(path));

        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Equal(new MapPoint(31, 32), reopened.MapSession.CurrentMap.Markers.Single().Position);
    }

    async Task<UiVm> CreateMarkerVm()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = MapDatasetTypes.Marker; Assert.True(await vm.CreateDatasetAsync());
        var layerId = MapDatasetLayerIdProjection.Project(vm.DatasetSelectedId!);
        Assert.True(vm.MapSession.CreateMarker(new(MapMarkerId.New(), layerId, "标记", new(7, 8))).IsSuccess);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        return vm;
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}

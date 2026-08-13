using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapMarkerPlacementTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-{Guid.NewGuid():N}");

    [Fact]
    public async Task Placement_creates_one_marker_and_returns_to_select()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Marker;
        Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("地图标记");
        Assert.True(await vm.BeginMarkerPlacementAsync());
        Assert.True(vm.IsMarkerPlacementTool);
        Assert.True(vm.MarkerPlacementPointerPressed(400, 300, Viewport));
        Assert.Single(vm.MapSession.CurrentMap.Markers);
        Assert.True(vm.IsSelectTool);
        Assert.Contains("地图标记", vm.SelectedMapGeometryText);
    }

    [Fact]
    public async Task Marker_save_reload_preserves_feature_dataset_and_layer_identity()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = MapDatasetTypes.Marker; Assert.True(await vm.CreateDatasetAsync());
        var datasetId = vm.DatasetSelectedId!; var layerId = MapDatasetLayerIdProjection.Project(datasetId);
        var marker = new MapMarker(MapMarkerId.New(), layerId, "保存标记", new(7, 8));
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
        Assert.True(await vm.SaveMapManifestAsync(path));
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        var loaded = reopened.MapSession.CurrentMap.Markers.Single();
        Assert.Equal(marker.MarkerId, loaded.MarkerId); Assert.Equal(datasetId, reopened.DatasetItems.Single().Id);
        Assert.Equal(layerId, loaded.LayerId); Assert.Equal(marker.Position, loaded.Position);
    }

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}

using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RoadVertexSelectionD1Tests
{
    async Task<(UiVm Vm, MapRoad Road)> CreateAsync(
        bool layerVisible = true, bool layerLocked = false, bool roadVisible = true, bool roadLocked = false)
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Road;
        Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("道路"); vm.SelectToolCommand.Execute("选择");
        var road = new MapRoad(MapRoadId.New(), vm.MapSession.ActiveRegionLayerId, "道路", "generic",
            [new(-1, -1), new(0, 0), new(1, 1)], roadVisible, roadLocked);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        if (!layerVisible) Assert.True(vm.MapSession.SetLayerVisibility(road.LayerId, false).IsSuccess);
        if (layerLocked) Assert.True(vm.MapSession.SetLayerLocked(road.LayerId, true).IsSuccess);
        return (vm, road);
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var world = MapCoordinateContract.MapToWorld(point, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        var screen = projection.ProjectWorldPoint(world);
        return (screen.X, screen.Y);
    }
}

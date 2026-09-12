using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class FeatureEditSelectionResetTests
{
    [Fact]
    public void Selecting_road_exits_marker_geometry_editing()
    {
        var (vm, marker, road, _) = Create();
        vm.SelectMapGeometry(marker); vm.ToggleGeometryEditingCommand.Execute(null);
        vm.SelectMapGeometry(road);
        Assert.False(vm.IsGeometryEditingActive);
    }

    [Fact]
    public void Selecting_region_exits_road_geometry_editing()
    {
        var (vm, _, road, region) = Create();
        vm.SelectMapGeometry(road); vm.ToggleGeometryEditingCommand.Execute(null);
        vm.SelectMapGeometry(region);
        Assert.False(vm.IsGeometryEditingActive);
    }

    [Fact]
    public void Clearing_region_selection_exits_geometry_editing()
    {
        var (vm, _, _, region) = Create();
        vm.SelectMapGeometry(region); vm.ToggleGeometryEditingCommand.Execute(null);
        vm.ClearMapGeometrySelection();
        Assert.False(vm.IsGeometryEditingActive);
    }

    static (UiVm Vm, MapGeometrySelection Marker, MapGeometrySelection Road, MapGeometrySelection Region) Create()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        var layer = vm.MapSession.ActiveRegionLayerId;
        var marker = new MapMarker(MapMarkerId.New(), layer, "标记", new(0, 0));
        var road = new MapRoad(MapRoadId.New(), layer, "道路", "generic", [new(0, 0), new(1, 1)]);
        var region = new MapRegion(MapRegionId.New(), layer, "区域", MapRegionKind.Generic,
            [new(0, 0), new(1, 0), new(0, 1)]);
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        return (vm,
            new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()),
            new(MapGeometryFeatureKind.Road, road.RoadId.ToString()),
            new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
    }
}

using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class FeatureEditSelectionResetTests
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

    [Fact]
    public void Map_region_selection_exposes_geometry_editing_target()
    {
        var (vm, _, _, region) = Create();
        Assert.True(vm.MapSession.SelectRegion(MapRegionIdFrom(region)).IsSuccess);

        Assert.True(vm.IsMapGeometrySelected);
        Assert.Equal("已选择区域", vm.SelectedMapGeometryText);
    }

    [Fact]
    public void Selected_region_vertex_can_begin_drag_without_toggle()
    {
        var (vm, _, _, region) = Create();
        vm.ToggleEditorModeCommand.Execute(null);
        vm.SelectMapGeometry(region);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var screen = projection.ProjectWorldPoint(new(0, 0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        Assert.True(vm.TryBeginMapGeometryPointer(screen.X, screen.Y, viewport));
        Assert.True(vm.IsMapGeometryDragActive);
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

    static MapRegionId MapRegionIdFrom(MapGeometrySelection selection) =>
        MapRegionId.TryParse(selection.FeatureId, out var id) ? id : default;
}

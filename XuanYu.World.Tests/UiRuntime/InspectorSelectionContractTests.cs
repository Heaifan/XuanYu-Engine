using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class InspectorSelectionContractTests
{
    [Fact]
    public void None_selection_resolves_to_empty_even_in_map_edit_mode()
    {
        var vm = NewVm().Vm;
        vm.MapSession.ClearSelection();
        vm.ToggleEditorMode();

        Assert.Equal(InspectorObjectKind.Empty, vm.InspectorIdentity);
        Assert.True(vm.IsInspectorEmpty);
        Assert.False(vm.IsMapInspector);
        Assert.False(vm.IsMarkerInspector);
        Assert.False(vm.IsRoadInspector);
        Assert.False(vm.IsRegionInspector);
    }

    [Fact]
    public void Explicit_map_selection_resolves_to_map()
    {
        var vm = NewVm().Vm;
        vm.MapSession.SelectMap();

        Assert.Equal(InspectorObjectKind.Map, vm.InspectorIdentity);
        Assert.True(vm.IsMapInspector);
    }

    [Theory]
    [InlineData(MapGeometryFeatureKind.Marker, InspectorObjectKind.Marker)]
    [InlineData(MapGeometryFeatureKind.Road, InspectorObjectKind.Road)]
    [InlineData(MapGeometryFeatureKind.Region, InspectorObjectKind.Region)]
    public void Geometry_selection_resolves_to_its_object_kind(
        MapGeometryFeatureKind geometry, InspectorObjectKind expected)
    {
        var fixture = NewVm();
        var selection = geometry switch
        {
            MapGeometryFeatureKind.Marker => fixture.Marker,
            MapGeometryFeatureKind.Road => fixture.Road,
            _ => fixture.Region
        };
        fixture.Vm.SelectMapGeometry(selection);

        Assert.Equal(expected, fixture.Vm.InspectorIdentity);
    }

    [Fact]
    public void Road_identity_survives_mode_and_geometry_tool_changes()
    {
        var fixture = NewVm();
        var vm = fixture.Vm;
        vm.SelectMapGeometry(fixture.Road);
        vm.ToggleEditorMode();
        vm.IsGeometryEditingActive = true;

        Assert.Equal(InspectorObjectKind.Road, vm.InspectorIdentity);
        Assert.True(vm.IsRoadInspector);
    }

    [Fact]
    public void Clear_selection_resolves_to_empty_without_map_fallback()
    {
        var fixture = NewVm();
        var vm = fixture.Vm;
        vm.SelectMapGeometry(fixture.Road);
        vm.ClearMapGeometrySelection();
        vm.MapSession.ClearSelection();

        Assert.Equal(InspectorObjectKind.Empty, vm.InspectorIdentity);
    }

    static (UiVm Vm, MapGeometrySelection Marker, MapGeometrySelection Road, MapGeometrySelection Region) NewVm()
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

using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorSingleFocusSectionTests
{
    [Fact]
    public void Section_toggle_allows_zero_or_one_expanded_section()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        Assert.Null(vm.ExpandedInspectorSection);
        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Geometry);
        Assert.Equal(InspectorSectionId.Geometry, vm.ExpandedInspectorSection);
        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Status);
        Assert.Equal(InspectorSectionId.Status, vm.ExpandedInspectorSection);
        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Status);
        Assert.Null(vm.ExpandedInspectorSection);
    }

    [Fact]
    public void Inspector_identity_change_collapses_all_sections()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Basic);
        vm.AddCubeEntity();
        Assert.Null(vm.ExpandedInspectorSection);

        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Basic);
        vm.SelectedHierarchyItem = null;
        vm.MapSession.SelectMap();
        Assert.Null(vm.ExpandedInspectorSection);
    }

    [Fact]
    public void Road_geometry_state_does_not_leak_into_marker_inspector()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        var layer = vm.MapSession.ActiveRegionLayerId;
        var road = new MapRoad(MapRoadId.New(), layer, "道路", "generic", [new(0, 0), new(1, 1)]);
        var marker = new MapMarker(MapMarkerId.New(), layer, "标记", new(2, 2));
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);

        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Geometry);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
        Assert.Null(vm.ExpandedInspectorSection);
    }
}

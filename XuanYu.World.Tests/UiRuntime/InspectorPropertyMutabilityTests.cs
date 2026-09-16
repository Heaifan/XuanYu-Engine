using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorPropertyMutabilityTests
{
    [Fact]
    public void Road_name_is_editable_but_geometry_result_is_readonly()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));

        vm.SelectInspectorCategoryCommand.Execute("基础");
        Assert.True(vm.InspectorProperties.Single(x => x.Key == "Road.Basic.Name").IsEditable);
        vm.SelectInspectorCategoryCommand.Execute("几何");
        Assert.False(vm.InspectorProperties.Single(x => x.Key == "Road.Geometry.Points").IsEditable);
    }

    [Fact]
    public void Road_name_commit_updates_map_through_edit_session()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        vm.SelectInspectorCategoryCommand.Execute("基础");

        Assert.True(vm.CommitInspectorProperty("Road.Basic.Name", "主干道"));
        Assert.Equal("主干道", vm.MapSession.CurrentMap.Roads.Single().DisplayName);
    }

    static (UiVm Vm, MapRoad Road) RoadVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        var road = new MapRoad(MapRoadId.New(), vm.MapSession.ActiveRegionLayerId, "道路", "generic", [new(0, 0), new(1, 1)]);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        return (vm, road);
    }
}

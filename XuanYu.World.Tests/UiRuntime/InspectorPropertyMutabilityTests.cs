using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using System.Reflection;

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

    [Fact]
    public void Successful_feature_name_commit_enters_recent_properties()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        Assert.True(vm.CommitInspectorProperty("Road.Basic.Name", "主干道"));
        vm.SelectInspectorCategoryCommand.Execute("最近");
        Assert.Contains(vm.InspectorProperties, row => row.Key == "Road.Basic.Name");
    }

    [Fact]
    public void Dataset_selection_does_not_expose_feature_properties_or_commit_path()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var row = new MapDatasetRow("道路数据", "road", "road-dataset", "正常", "data/road.json");
        typeof(UiVm).GetField("_datasetItems", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(vm, new[] { row });
        vm.DatasetSelectedId = row.Id;
        Assert.Equal("Dataset", vm.InspectorIdentity.ToString());
        Assert.DoesNotContain(vm.InspectorProperties, item => item.Key == "Road.Basic.Name");
        Assert.False(vm.CommitInspectorProperty("Road.Basic.Name", "错误编辑"));
    }

    [Fact]
    public void Feature_name_value_reads_authoritative_model_and_failed_edit_reverts_projection()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        vm.SelectInspectorCategoryCommand.Execute("基础");
        Assert.Equal("道路", vm.InspectorProperties.Single(item => item.Key == "Road.Basic.Name").Value);
        Assert.False(vm.CommitInspectorProperty("Road.Basic.Name", ""));
        Assert.Equal("道路", vm.InspectorProperties.Single(item => item.Key == "Road.Basic.Name").Value);
    }

    [Fact]
    public void Feature_status_value_reports_visibility_and_lock_state()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        vm.SelectInspectorCategoryCommand.Execute("状态");
        Assert.Equal("可见：是；锁定：否", vm.InspectorFeatureStatusText);
    }

    [Fact]
    public void Feature_name_validation_uses_feature_wording()
    {
        var (vm, road) = RoadVm();
        var result = vm.MapSession.RenameRoad(road.RoadId, "");
        Assert.Contains("要素名称", result.Error!.Value.Message);
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

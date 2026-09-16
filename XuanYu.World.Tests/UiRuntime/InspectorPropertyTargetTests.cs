using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class InspectorPropertyTargetTests
{
    [Fact]
    public void Entity_edit_target_survives_selection_switch()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: true);
        var first = vm.HierarchyItems.First(item => item.IsEntity);
        var second = vm.HierarchyItems.Last(item => item.IsEntity);
        vm.SelectedHierarchyItem = first;
        vm.SelectInspectorCategoryCommand.Execute("基础");
        var target = vm.InspectorProperties.Single(item => item.Key == "Entity.Basic.Name").EditTarget;

        vm.SelectedHierarchyItem = second;
        Assert.True(vm.CommitInspectorProperty(target, "第一个实体"));
        Assert.Equal("第一个实体", vm.HierarchyItems.Single(item => item.Key == first.Key).Title);
        Assert.Equal(second.Title, vm.HierarchyItems.Single(item => item.Key == second.Key).Title);
    }

    [Fact]
    public void Region_edit_target_survives_selection_switch()
    {
        var (vm, first, second) = RegionVm();
        Assert.True(CommitAfterSwitch(vm, first.RegionId.ToString(), second.RegionId.ToString(),
            "Region.Basic.Name", "第一个区域"));
        Assert.Equal("第一个区域", vm.MapSession.CurrentMap.Regions.Single(item => item.RegionId == first.RegionId).DisplayName);
        Assert.Equal("第二区域", vm.MapSession.CurrentMap.Regions.Single(item => item.RegionId == second.RegionId).DisplayName);
    }

    [Fact]
    public void Marker_edit_target_survives_selection_switch()
    {
        var (vm, first, second) = MarkerVm();
        Assert.True(CommitAfterSwitch(vm, first.MarkerId.ToString(), second.MarkerId.ToString(),
            "Marker.Basic.Name", "第一个标记"));
        Assert.Equal("第一个标记", vm.MapSession.CurrentMap.Markers.Single(item => item.MarkerId == first.MarkerId).DisplayName);
        Assert.Equal("第二标记", vm.MapSession.CurrentMap.Markers.Single(item => item.MarkerId == second.MarkerId).DisplayName);
    }

    [Fact]
    public void Recent_is_scoped_to_target_identity_after_selection_switch()
    {
        var (vm, first, second) = RoadVm();
        var target = CaptureTarget(vm, first.RoadId.ToString(), "Road.Basic.Name");
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, second.RoadId.ToString()));
        Assert.True(vm.CommitInspectorProperty(target, "第一道路"));

        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, second.RoadId.ToString()));
        vm.SelectInspectorCategoryCommand.Execute("最近");
        Assert.Empty(vm.InspectorProperties);
        Assert.Equal("第一道路", vm.MapSession.CurrentMap.Roads.Single(item => item.RoadId == first.RoadId).DisplayName);
        Assert.Equal("第二道路", vm.MapSession.CurrentMap.Roads.Single(item => item.RoadId == second.RoadId).DisplayName);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, first.RoadId.ToString()));
        vm.SelectInspectorCategoryCommand.Execute("最近");
        Assert.Contains(vm.InspectorProperties, item => item.Key == "Road.Basic.Name");
    }

}

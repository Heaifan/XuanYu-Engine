using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class InspectorPropertyTargetTests
{
    static bool CommitAfterSwitch(UiVm vm, string firstId, string secondId, string key, string name)
    {
        var target = CaptureTarget(vm, firstId, key);
        var kind = key[..key.IndexOf('.')].ToString() switch
        {
            "Region" => MapGeometryFeatureKind.Region,
            "Marker" => MapGeometryFeatureKind.Marker,
            _ => MapGeometryFeatureKind.Road
        };
        vm.SelectMapGeometry(new(kind, secondId));
        return vm.CommitInspectorProperty(target, name);
    }

    static InspectorEditTarget CaptureTarget(UiVm vm, string id, string key)
    {
        var kind = key.StartsWith("Road") ? MapGeometryFeatureKind.Road :
            key.StartsWith("Region") ? MapGeometryFeatureKind.Region : MapGeometryFeatureKind.Marker;
        vm.SelectMapGeometry(new(kind, id));
        vm.SelectInspectorCategoryCommand.Execute("基础");
        return vm.InspectorProperties.Single(item => item.Key == key).EditTarget;
    }

    static (UiVm, MapRoad, MapRoad) RoadVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        var first = new MapRoad(MapRoadId.New(), vm.MapSession.ActiveRegionLayerId, "第一道路", "generic", [new(0, 0), new(1, 1)]);
        var second = new MapRoad(MapRoadId.New(), first.LayerId, "第二道路", "generic", [new(2, 2), new(3, 3)]);
        Assert.True(vm.MapSession.CreateRoad(first).IsSuccess); Assert.True(vm.MapSession.CreateRoad(second).IsSuccess);
        return (vm, first, second);
    }

    static (UiVm, MapRegion, MapRegion) RegionVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null); var layer = vm.MapSession.ActiveRegionLayerId;
        var first = new MapRegion(MapRegionId.New(), layer, "第一地区", MapRegionKind.Generic, [new(0, 0), new(1, 0), new(0, 1)]);
        var second = new MapRegion(MapRegionId.New(), layer, "第二区域", MapRegionKind.Generic, [new(2, 2), new(3, 2), new(2, 3)]);
        Assert.True(vm.MapSession.CreateRegion(first).IsSuccess); Assert.True(vm.MapSession.CreateRegion(second).IsSuccess);
        return (vm, first, second);
    }

    static (UiVm, MapMarker, MapMarker) MarkerVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null); var layer = vm.MapSession.ActiveRegionLayerId;
        var first = new MapMarker(MapMarkerId.New(), layer, "第一标记", new(0, 0));
        var second = new MapMarker(MapMarkerId.New(), layer, "第二标记", new(2, 2));
        Assert.True(vm.MapSession.CreateMarker(first).IsSuccess); Assert.True(vm.MapSession.CreateMarker(second).IsSuccess);
        return (vm, first, second);
    }
}

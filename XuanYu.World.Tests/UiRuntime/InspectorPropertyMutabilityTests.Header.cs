using XuanYu.Editor.MapEditing;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class InspectorPropertyMutabilityTests
{
    [Fact]
    public void Feature_header_uses_authoritative_display_name()
    {
        var (vm, road) = RoadVm();
        Assert.True(vm.MapSession.RenameRoad(road.RoadId, "道路1").IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        Assert.Equal("道路1", vm.InspectorSelectionTitle);
    }
}

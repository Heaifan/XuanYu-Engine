using XuanYu.Editor.MapEditing;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class InspectorPropertyMutabilityTests
{
    [Fact]
    public void Selecting_feature_opens_basic_page_with_name_property()
    {
        var (vm, road) = RoadVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));

        Assert.Equal("基础", vm.InspectorCategory);
        Assert.Contains(vm.InspectorProperties, row => row.Key == "Road.Basic.Name");
    }
}

using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorRegionColorTests
{
    [Fact]
    public void Region_fill_color_property_commits_to_selected_region()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: true);
        var region = new MapRegion(
            MapRegionId.New(),
            vm.MapSession.ActiveRegionLayerId,
            "区域1",
            MapRegionKind.Generic,
            [new(0, 0), new(100, 0), new(100, 100)]);

        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));

        var row = vm.InspectorProperties.Single(item => item.Key == "Region.Style.FillColor");
        Assert.True(row.IsEditable);
        Assert.Equal("#338CE6", row.Value);

        Assert.True(vm.CommitInspectorProperty(row.EditTarget, "#CC6633"));
        Assert.Equal(0x00CC6633u, vm.MapSession.CurrentMap.Regions[0].FillColorRgb);
        Assert.Equal("#CC6633", vm.InspectorRegionFillColorText);

        Assert.False(vm.CommitInspectorProperty(row.EditTarget, "red"));
        Assert.Equal(0x00CC6633u, vm.MapSession.CurrentMap.Regions[0].FillColorRgb);
    }
}

using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorRegionColorPreviewTests
{
    [Fact]
    public void Preview_does_not_mutate_domain_and_confirm_commits_once()
    {
        var (vm, region) = RegionVm();
        var row = vm.InspectorProperties.Single(item => item.Key == "Region.Style.FillColor");
        var sequence = vm.MapSession.ChangeSequence;

        Assert.True(vm.BeginRegionFillColorPreview(row.EditTarget));
        vm.PreviewRegionFillColor(row.EditTarget, 0x00CC6633);
        vm.PreviewRegionFillColor(row.EditTarget, 0x00112233);

        Assert.Equal(MapRegion.DefaultFillColorRgb, vm.MapSession.CurrentMap.Regions[0].FillColorRgb);
        Assert.Equal(sequence, vm.MapSession.ChangeSequence);

        Assert.True(vm.CommitRegionFillColorPreview(row.EditTarget, 0x00112233));
        Assert.Equal(0x00112233u, vm.MapSession.CurrentMap.Regions[0].FillColorRgb);
        Assert.Equal(sequence + 1, vm.MapSession.ChangeSequence);

        Assert.True(vm.MapSession.Undo().IsSuccess);
        Assert.Equal(MapRegion.DefaultFillColorRgb, vm.MapSession.CurrentMap.Regions[0].FillColorRgb);
    }

    [Fact]
    public void Selection_change_cancels_locked_target_preview()
    {
        var (vm, first) = RegionVm();
        var second = NewRegion(vm, "区域2", 200);
        Assert.True(vm.MapSession.CreateRegion(second).IsSuccess);
        var row = vm.InspectorProperties.Single(item => item.Key == "Region.Style.FillColor");

        Assert.True(vm.BeginRegionFillColorPreview(row.EditTarget));
        vm.PreviewRegionFillColor(row.EditTarget, 0x00AA5500);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, second.RegionId.ToString()));

        Assert.False(vm.CommitRegionFillColorPreview(row.EditTarget, 0x00AA5500));
        Assert.Equal(MapRegion.DefaultFillColorRgb,
            vm.MapSession.CurrentMap.Regions.Single(item => item.RegionId == first.RegionId).FillColorRgb);
        Assert.Equal(MapRegion.DefaultFillColorRgb,
            vm.MapSession.CurrentMap.Regions.Single(item => item.RegionId == second.RegionId).FillColorRgb);
    }

    static (UiVm Vm, MapRegion Region) RegionVm()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: true);
        var region = NewRegion(vm, "区域1", 0);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
        return (vm, region);
    }

    static MapRegion NewRegion(UiVm vm, string name, double x) => new(
        MapRegionId.New(), vm.MapSession.ActiveRegionLayerId, name, MapRegionKind.Generic,
        [new(x, 0), new(x + 100, 0), new(x + 100, 100)]);
}

using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawContextSyncFix1Tests
{
    [Fact]
    public void Tool_change_clears_region_hover_and_preserves_selection()
    {
        var vm = RegionDrawingTestVm.Create();
        var region = new MapRegion(MapRegionId.New(), vm.MapSession.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
        vm.SelectToolCommand.Execute("区域绘制");
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var screen = projection.ProjectWorldPoint(new(0, 0,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        Assert.True(vm.RegionDrawingPointerMoved(screen.X, screen.Y, viewport));
        Assert.NotNull(vm.MapGeometryPreview);
        vm.SelectToolCommand.Execute("选择");
        Assert.Null(vm.MapGeometryPreview);
        Assert.True(vm.IsMapGeometrySelected);
        Assert.Equal("已选择区域", vm.SelectedMapGeometryText);
    }
}

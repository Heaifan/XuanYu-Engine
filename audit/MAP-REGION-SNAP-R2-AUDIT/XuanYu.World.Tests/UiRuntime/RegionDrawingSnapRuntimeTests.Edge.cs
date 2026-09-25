using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    [Fact]
    public void Region_drawing_snaps_to_existing_edge_projection()
    {
        var vm = CreateWithExistingRegion(out _);
        vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var start = vm.MapSession.CurrentMap.Regions[0].Vertices[0];
        var end = vm.MapSession.CurrentMap.Regions[0].Vertices[1];
        var pointer = projection.ProjectWorldPoint(new((start.X + end.X) / 2,
            (start.Y + end.Y) / 2, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        var screen = new ScreenPoint(pointer.X, pointer.Y + 4);
        vm.RegionDrawingPointerPressed(500, 500, Viewport);

        Assert.True(vm.RegionDrawingPointerMoved(screen.X, screen.Y, Viewport));
        Assert.True(vm.IsRegionDrawingSnapActive);
        Assert.Equal("边吸附", vm.RegionDrawingSnapStatus);
        Assert.True(RegionEdgeSnapGeometry.TryClosestPoint(screen,
            projection.ProjectWorldPoint(new(start.X, start.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters)),
            projection.ProjectWorldPoint(new(end.X, end.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters)),
            out _, out var t));
        Assert.Equal(new MapPoint(start.X + (end.X - start.X) * t,
            start.Y + (end.Y - start.Y) * t), vm.RegionDrawingCursor);
    }
}

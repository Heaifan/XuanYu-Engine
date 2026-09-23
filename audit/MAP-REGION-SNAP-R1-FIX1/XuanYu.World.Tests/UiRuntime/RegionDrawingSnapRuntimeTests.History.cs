using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    [Fact]
    public void Snapped_vertex_undo_redo_preserves_exact_coordinate()
    {
        var vm = CreateWithExistingRegion(out var target);
        vm.SelectToolCommand.Execute("区域绘制");
        var start = FindHit(vm, 500, 500);
        vm.RegionDrawingPointerPressed(start.X, start.Y, Viewport);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(
            target.X, target.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        vm.RegionDrawingPointerPressed(screen.X + 4, screen.Y, Viewport);
        var third = FindHit(vm, 620, 450);
        vm.RegionDrawingPointerPressed(third.X, third.Y, Viewport);
        Assert.True(vm.UndoRegionDrawingVertex());
        Assert.True(vm.RedoRegionDrawingVertex());
        Assert.True(vm.CommitRegionDrawingFromEnter());

        Assert.Contains(vm.MapSession.CurrentMap.Regions[1].Vertices, point => point == target);
    }
}

using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Region_drawing_reuses_existing_vertex_coordinate_and_reports_target()
    {
        var vm = CreateWithExistingRegion(out var target);
        vm.SelectToolCommand.Execute("区域绘制");
        var start = FindHit(vm, 500, 500);
        vm.RegionDrawingPointerPressed(start.X, start.Y, Viewport);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(target.X, target.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        Assert.True(vm.RegionDrawingPointerMoved(screen.X + 4, screen.Y, Viewport));
        Assert.True(vm.IsRegionDrawingSnapActive);
        Assert.Equal("顶点吸附", vm.RegionDrawingSnapStatus);
        Assert.Equal(target, vm.RegionDrawingSnapTargetPoint);
        Assert.True(vm.RegionDrawingPointerPressed(screen.X + 4, screen.Y, Viewport));
        Assert.Equal(2, vm.RegionDrawingDraftVertexCount);
        Assert.Equal(target, vm.RegionDrawingCursor);
    }

    [Fact]
    public void Alt_suppresses_snap_and_release_reacquires_without_pointer_motion()
    {
        var vm = CreateWithExistingRegion(out var target);
        vm.SelectToolCommand.Execute("区域绘制");
        var start = FindHit(vm, 500, 500);
        vm.RegionDrawingPointerPressed(start.X, start.Y, Viewport);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(target.X, target.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        vm.RegionDrawingPointerMoved(screen.X + 4, screen.Y, Viewport);
        Assert.True(vm.RegionDrawingPointerMoved(screen.X + 4, screen.Y, Viewport, snapSuppressed: true));
        Assert.False(vm.IsRegionDrawingSnapActive);
        Assert.Equal("Alt 已取消吸附", vm.RegionDrawingSnapStatus);
        Assert.NotEqual(target, vm.RegionDrawingCursor);
        Assert.True(vm.RegionDrawingPointerMoved(screen.X + 4, screen.Y, Viewport));
        Assert.True(vm.IsRegionDrawingSnapActive);
        Assert.Equal("顶点吸附", vm.RegionDrawingSnapStatus);
    }

    [Fact]
    public void Turning_snap_off_suppresses_region_vertex_snap()
    {
        var vm = CreateWithExistingRegion(out var target);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        vm.ToggleSnapCommand.Execute(null);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.SelectToolCommand.Execute("区域绘制");
        var start = FindHit(vm, 500, 500);
        vm.RegionDrawingPointerPressed(start.X, start.Y, Viewport);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(target.X, target.Y,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        vm.RegionDrawingPointerMoved(screen.X + 4, screen.Y, Viewport);

        Assert.False(vm.IsSnapEnabled);
        Assert.False(vm.IsRegionDrawingSnapActive);
        Assert.NotEqual(target, vm.RegionDrawingCursor);
    }

    static UiVm CreateWithExistingRegion(out MapPoint target)
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        var points = new[] { FindHit(vm, 180, 180), FindHit(vm, 620, 180), FindHit(vm, 620, 420) };
        target = Pick(vm, points[0]);
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        return vm;
    }

    static (double X, double Y) FindHit(UiVm vm, double x, double y) =>
        MapSurfacePicker.TryPick(vm.MapSession.CurrentMap,
            ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport), x, y, out _)
            ? (x, y) : throw new InvalidOperationException("test point did not hit map");

    static MapPoint Pick(UiVm vm, (double X, double Y) screen) =>
        MapSurfacePicker.TryPick(vm.MapSession.CurrentMap,
            ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport), screen.X, screen.Y, out var point)
            ? point : throw new InvalidOperationException("test point did not hit map");
}

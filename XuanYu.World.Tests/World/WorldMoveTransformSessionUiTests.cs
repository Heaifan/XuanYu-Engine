using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldMoveTransformUiTests
{
    [Fact]
    public void Escape_cancel_ignores_late_pointerup_and_keeps_next_session()
    {
        var vm = MoveVm();
        var first = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, first.X, first.Y, first.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, first.EndX, first.EndY));
        vm.CancelInteractionFromEscape();
        Assert.False(vm.CommitViewportPointer(7, first.EndX, first.EndY));
        Assert.Equal(Vector3d.Zero, vm.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(0, HistoryOf(vm).Count);

        var second = AxisHit(vm, MoveGizmoAxis.Y);
        Assert.True(vm.TryBeginMoveGizmoCapture(8, second.X, second.Y, second.Viewport, true));
        Assert.False(vm.CommitViewportPointer(7, first.EndX, first.EndY));
        Assert.True(vm.HasInteractionCaptureForPointer(8));
        Assert.True(vm.CommitViewportPointer(8, second.EndX, second.EndY));
        Assert.Equal(1, HistoryOf(vm).Count);
    }

    [Fact]
    public void Resize_cancels_move_session_without_history()
    {
        var vm = MoveVm();
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        vm.UpdateViewportFrame(900, 600);

        Assert.False(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(Vector3d.Zero, vm.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    [Theory]
    [InlineData("PointerCaptureLost")]
    [InlineData("WM_CANCELMODE")]
    public void Native_cancel_restores_start_without_history(string reason)
    {
        var vm = MoveVm();
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        vm.CancelInteractionFromNativePointer(reason);

        Assert.False(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(Vector3d.Zero, vm.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    [Fact]
    public void Move_session_blocks_camera_picking_tool_switch_and_second_move()
    {
        var vm = MoveVm();
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.False(vm.BeginCameraNavigation(9, 10, 10, false, 800, 600));
        Assert.False(vm.DollyCamera(1));
        Assert.False(vm.PickViewportPointer(1, 1, 800, 600, 800, 600, 1, 1, true));
        vm.SelectToolCommand.Execute("选择");
        Assert.Equal("移动", vm.ActiveTool);
        Assert.False(vm.TryBeginMoveGizmoCapture(8, hit.X, hit.Y, hit.Viewport, true));
    }
}

using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldRotateTransformUiTests
{
    [Fact]
    public void Rotate_with_no_pointer_movement_creates_no_history()
    {
        var vm = RotateVm();
        var hit = RingHit(vm, RotateGizmoAxis.Z);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.StartX, hit.StartY));
        Assert.True(vm.CommitViewportPointer(7, hit.StartX, hit.StartY));

        Assert.Equal(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    [Fact]
    public void Rotate_commit_and_cancel_clear_drag_state()
    {
        var vm = RotateVm();
        var hit = RingHit(vm, RotateGizmoAxis.Z);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Null(DragOf(vm));

        Assert.True(vm.TryBeginRotateGizmoCapture(8, hit.StartX, hit.StartY, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(8, hit.MidX, hit.MidY));
        vm.CancelInteractionFromEscape();
        Assert.Null(DragOf(vm));
    }

    static object? DragOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_rotateDrag", BindingFlags.Instance | BindingFlags.NonPublic);
        return field!.GetValue(vm);
    }
}

using XuanYu.Core.Gizmo;
using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.World;

public sealed partial class RotateTransformUiTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Escape)]
    [InlineData(EditorPointerEventKind.Cancel)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    public void Direct_rotate_begin_rejects_late_commit_after_terminal(
        EditorPointerEventKind terminal)
    {
        var vm = RotateVm();
        var hit = RingHit(vm, RotateGizmoAxis.Z);
        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, hit.Viewport, true));
        vm.ViewportInput.Terminate(terminal);
        Assert.False(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(0, HistoryOf(vm).Count);
    }
}

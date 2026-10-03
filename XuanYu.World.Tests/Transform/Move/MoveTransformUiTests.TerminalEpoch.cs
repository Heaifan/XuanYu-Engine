using XuanYu.Core.Gizmo;
using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.World;

public sealed partial class MoveTransformUiTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Escape)]
    [InlineData(EditorPointerEventKind.Cancel)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    public void Direct_move_begin_rejects_late_commit_after_terminal(
        EditorPointerEventKind terminal)
    {
        var vm = MoveVm();
        var hit = AxisHit(vm, MoveGizmoAxis.X);
        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        vm.ViewportInput.Terminate(terminal);
        Assert.False(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(0, HistoryOf(vm).Count);
    }
}

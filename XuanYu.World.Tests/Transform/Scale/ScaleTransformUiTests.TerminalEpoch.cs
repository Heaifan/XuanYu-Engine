using XuanYu.Core.Gizmo;
using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.World;

public sealed partial class ScaleTransformUiTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Escape)]
    [InlineData(EditorPointerEventKind.Cancel)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    public void Direct_scale_begin_rejects_late_commit_after_terminal(
        EditorPointerEventKind terminal)
    {
        var vm = ScaleVm();
        var (x, y, viewport) = ScaleHit(vm, ScaleGizmoHandle.Uniform);
        Assert.True(vm.TryBeginScaleGizmoCapture(7, x, y, viewport, true));
        vm.ViewportInput.Terminate(terminal);
        Assert.False(vm.CommitViewportPointer(7, x, y - 60));
        Assert.Equal(0, HistoryOf(vm).Count);
    }
}

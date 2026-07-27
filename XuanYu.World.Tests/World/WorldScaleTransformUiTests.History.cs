using XuanYu.Core.Gizmo;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldScaleTransformUiTests
{
    [Fact]
    public void Scale_drag_commits_once_and_undo_redo_restores()
    {
        var vm = ScaleVm();
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));

        Assert.NotEqual(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
        Assert.Equal(1, HistoryOf(vm).Count);
        vm.RunCommand.Execute("撤销");
        Assert.Equal(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
        Assert.Equal(0, HistoryOf(vm).Count);
        vm.RunCommand.Execute("重做");
        Assert.NotEqual(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
    }

    [Fact]
    public void Scale_cancel_discards_preview_without_history()
    {
        var vm = ScaleVm();
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        vm.CancelInteractionFromEscape();

        Assert.False(vm.CommitViewportPointer(7, px, py));
        Assert.Equal(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
        Assert.Equal(0, HistoryOf(vm).Count);
    }
}

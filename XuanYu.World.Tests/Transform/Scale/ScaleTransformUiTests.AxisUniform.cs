using XuanYu.Core.Gizmo;

namespace XuanYu.World.Tests.World;

public sealed partial class ScaleTransformUiTests
{
    [Fact]
    public void y_and_z_axis_still_scale_only_their_component()
    {
        AxisScalesOnlyComponent(ScaleGizmoHandle.Y);
        AxisScalesOnlyComponent(ScaleGizmoHandle.Z);
    }

    [Fact]
    public void uniform_drag_commits_once_and_undo_redo_restores()
    {
        var vm = ScaleVm();
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.Uniform);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.Uniform, vp, 110);
        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));
        var committed = vm.RenderSnapshot.Entity.Transform.Scale.X;
        Assert.NotEqual(1.0, committed);
        Assert.Equal(1, HistoryOf(vm).Count);
        vm.RunCommand.Execute("撤销");
        Assert.Equal(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
        Assert.Equal(0, HistoryOf(vm).Count);
        vm.RunCommand.Execute("重做");
        Assert.Equal(committed, vm.RenderSnapshot.Entity.Transform.Scale.X);
    }

    [Fact]
    public void uniform_escape_cancel_rejects_late_mouse_up()
    {
        var vm = ScaleVm();
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.Uniform);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.Uniform, vp, 110);
        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.NotEqual(1.0, vm.RenderSnapshot.RenderTransform.Scale.X);
        vm.CancelInteractionFromEscape();
        Assert.False(vm.CommitViewportPointer(7, px, py));
        Assert.Equal(1.0, vm.RenderSnapshot.Entity.Transform.Scale.X);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    static void AxisScalesOnlyComponent(ScaleGizmoHandle handle)
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        var (hx, hy, vp) = ScaleHit(vm, handle);
        var (px, py) = ScalePull(vm, handle, vp, 110);
        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));
        Assert.True(scene.TryGetEntity(XuanYu.Core.Identity.EntityId.FromInt(1), out var a));
        if (handle == ScaleGizmoHandle.Y)
        {
            Assert.Equal(1.0, a.Transform.Scale.X);
            Assert.True(a.Transform.Scale.Y > 1.5);
            Assert.Equal(1.0, a.Transform.Scale.Z);
        }
        else
        {
            Assert.Equal(1.0, a.Transform.Scale.X);
            Assert.Equal(1.0, a.Transform.Scale.Y);
            Assert.True(a.Transform.Scale.Z > 1.5);
        }
    }
}

using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Space;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldScaleTransformUiTests
{
    [Fact]
    public void Blank_click_does_not_begin_scale()
    {
        var vm = ScaleVm();
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        Assert.False(vm.TryBeginScaleGizmoCapture(7, -9999, -9999, viewport, true));
    }

    [Fact]
    public void Switch_to_B_then_first_drag_scales_B_not_A()
    {
        var vm = ScaleVmTwoEntities(out var scene, out var bKey);
        var aKey = EntityId.FromInt(1);
        var (bx, by, bvp) = ScreenOf(scene, bKey, vm);
        Assert.True(vm.PickViewportPointer(bx, by, (int)bvp.LogicalWidth,
            (int)bvp.LogicalHeight, bvp.PhysicalWidth, bvp.PhysicalHeight, bvp.DpiScale,
            bvp.Revision, true));
        Assert.Equal(bKey, vm.RenderSnapshot.Entity.EntityKey);

        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);
        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));

        Assert.True(scene.TryGetEntity(bKey, out var b));
        Assert.True(b.Transform.Scale.X > 1.5);
        Assert.True(scene.TryGetEntity(aKey, out var a));
        Assert.Equal(1.0, a.Transform.Scale.X);
        Assert.Equal(1, HistoryOf(vm).Count);
    }

    [Fact]
    public void Scale_gizmo_world_axis_length_positive_and_bounded_after_resize()
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        vm.UpdateViewportFrame(400, 300);
        var viewport = new ViewportState(0, 0, 400, 300, 400, 300, 1, 1);
        scene.TryGetEntity(EntityId.FromInt(1), out var entity);
        var r = ScaleGizmoScreenSize.ComputeWorldAxisLength(
            vm.RenderSnapshot.CameraState, viewport, entity.Transform.Position);
        Assert.True(r > 0.0);
        Assert.True(r < 5.0, $"世界轴长 {r} 过大，易误触");
    }
}

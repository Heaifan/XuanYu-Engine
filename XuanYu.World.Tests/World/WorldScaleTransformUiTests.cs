using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

// R5：Scale Gizmo 缩放变换闭环集成测试。复用既有 SelectionKey / TransformSession / History 体系，
// 钉死：单轴只改对应分量、Uniform 三轴同倍、实时预览、单次提交、Esc 取消、Undo/Redo、工具内切换。
public sealed partial class WorldScaleTransformUiTests
{
    // ① Begin Scale 后一次 Preview、尚未 Commit：渲染快照已变化，但 World 正式 Transform 尚未变化。
    [Fact]
    public void Begin_scale_then_single_preview_updates_render_scale_before_commit()
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        var aKey = EntityId.FromInt(1);
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));

        Assert.NotEqual(1.0, vm.RenderSnapshot.RenderTransform.Scale.X); // 实时预览
        Assert.True(scene.TryGetEntity(aKey, out var aEntity));
        Assert.Equal(1.0, aEntity.Transform.Scale.X); // 正式值未提交
    }

    // ② Preview → Commit 前后不跳变。
    [Fact]
    public void Preview_to_commit_keeps_render_scale_stable()
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        var previewScale = vm.RenderSnapshot.RenderTransform.Scale.X;
        Assert.True(vm.CommitViewportPointer(7, px, py));
        var committedScale = vm.RenderSnapshot.Entity.Transform.Scale.X;

        Assert.Equal(previewScale, committedScale);
        Assert.True(scene.TryGetEntity(EntityId.FromInt(1), out var aEntity));
        Assert.Equal(committedScale, aEntity.Transform.Scale.X);
    }

    // ③ X 单轴提交后只改 X，Y/Z 不变（World 正式值）。
    [Fact]
    public void Commit_x_axis_scales_only_x_component()
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        var aKey = EntityId.FromInt(1);
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.X);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.X, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));

        Assert.True(scene.TryGetEntity(aKey, out var a));
        Assert.True(a.Transform.Scale.X > 1.5);
        Assert.Equal(1.0, a.Transform.Scale.Y);
        Assert.Equal(1.0, a.Transform.Scale.Z);
    }

    // ④ Uniform 提交后三轴同倍放大。
    [Fact]
    public void Commit_uniform_scales_all_three_equally()
    {
        var vm = ScaleVmTwoEntities(out var scene, out _);
        var aKey = EntityId.FromInt(1);
        var (hx, hy, vp) = ScaleHit(vm, ScaleGizmoHandle.Uniform);
        var (px, py) = ScalePull(vm, ScaleGizmoHandle.Uniform, vp, 110);

        Assert.True(vm.TryBeginScaleGizmoCapture(7, hx, hy, vp, true));
        Assert.True(vm.PreviewViewportPointer(7, px, py));
        Assert.True(vm.CommitViewportPointer(7, px, py));

        Assert.True(scene.TryGetEntity(aKey, out var a));
        Assert.True(a.Transform.Scale.X > 1.5 && a.Transform.Scale.Y > 1.5 && a.Transform.Scale.Z > 1.5);
        Assert.Equal(a.Transform.Scale.X, a.Transform.Scale.Y);
        Assert.Equal(a.Transform.Scale.Y, a.Transform.Scale.Z);
    }

}

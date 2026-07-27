using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldRotateTransformUiTests
{
    // R4-R3-R1：旋转预览必须是实时的，且“选中轮廓”改用单 Draw 重心坐标边缘高亮后，
    // 不得再出现“点击其他实体才应用旋转”的视觉假象。以下测试钉死预览链语义。

    // B：Begin Rotate 后一次有效 Preview、尚未 Commit 时，
    //    该实体（EntityKey=A）的 Rotation 在 RenderSnapshot 中已变化，但 World 正式 Transform 尚未变化。
    [Fact]
    public void Begin_rotate_then_single_preview_updates_render_rotation_before_commit()
    {
        var vm = RotateVmTwoEntities(out var scene, out _);
        var aKey = EntityId.FromInt(1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var hit = RingHitFor(vm, viewport);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));

        // 预览阶段：渲染消费快照的 RenderTransform（含 Preview）已变化（实时预览）。
        Assert.NotEqual(0.0, vm.RenderSnapshot.RenderTransform.Rotation.Z);
        Assert.Equal(aKey, vm.RenderSnapshot.Entity.EntityKey);
        // 但正式 World Transform 尚未提交。
        Assert.True(scene.TryGetEntity(aKey, out var aEntity));
        Assert.Equal(0.0, aEntity.Transform.Rotation.Z);
    }

    // C：Preview → Commit 前后，该实体的 Render Rotation 不跳变
    //    （Commit 用与 Preview 完全相同的 Transform 写正式值，不应在 MouseUp/切换选择时突跳）。
    [Fact]
    public void Preview_to_commit_keeps_render_rotation_stable()
    {
        var vm = RotateVmTwoEntities(out var scene, out _);
        var aKey = EntityId.FromInt(1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var hit = RingHitFor(vm, viewport);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        var previewRotation = vm.RenderSnapshot.RenderTransform.Rotation.Z;
        Assert.NotEqual(0.0, previewRotation);

        // 在同一 Preview 位置（mid）提交，不额外移动：证明从预览到提交该旋转值不突变。
        Assert.True(vm.CommitViewportPointer(7, hit.MidX, hit.MidY));
        var committedRotation = vm.RenderSnapshot.Entity.Transform.Rotation.Z;

        // 预览值 == 提交后值：不跳变。
        Assert.Equal(previewRotation, committedRotation);
        Assert.True(scene.TryGetEntity(aKey, out var aEntity));
        Assert.Equal(committedRotation, aEntity.Transform.Rotation.Z);
    }

    // D：Commit A 后选择 B，不得再次改变 A 的 Rotation；
    //    切换选择只改变 IsSelected 与当前实体，不“补应用”A 的 Transform。
    [Fact]
    public void Commit_A_then_select_B_does_not_mutate_A_rotation()
    {
        var vm = RotateVmTwoEntities(out var scene, out var bKey);
        var aKey = EntityId.FromInt(1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var hit = RingHitFor(vm, viewport);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.True(scene.TryGetEntity(aKey, out var aEntity));
        var aRotationAfterCommit = aEntity.Transform.Rotation.Z;
        Assert.NotEqual(0.0, aRotationAfterCommit);

        // 切换选择到 B（点选 B 的屏幕位置）。
        var (bx, by, bvp) = ScreenOf(scene, bKey, vm);
        Assert.True(vm.PickViewportPointer(bx, by,
            (int)bvp.LogicalWidth, (int)bvp.LogicalHeight,
            bvp.PhysicalWidth, bvp.PhysicalHeight, bvp.DpiScale, bvp.Revision, true));

        // 选择切到 B。
        Assert.Equal(bKey, vm.RenderSnapshot.Entity.EntityKey);
        // A 的正式 Rotation 不受选择切换影响。
        Assert.True(scene.TryGetEntity(aKey, out var aEntityAfter));
        Assert.Equal(aRotationAfterCommit, aEntityAfter.Transform.Rotation.Z);
    }
}

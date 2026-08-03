using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldRotateTransformUiTests
{
    // R4-R2：旋转工具激活时点击其他实体必须立即切换选择，且工具保持 Rotate；
    // Selected / Gizmo / Inspector 目标必须一致；第一次拖动即修改新实体，不污染旧实体。
    [Fact]
    public void Rotate_tool_click_other_entity_switches_selection_and_keeps_rotate_tool()
    {
        var vm = RotateVmTwoEntities(out var scene, out var bKey);
        var (bx, by, viewport) = ScreenOf(scene, bKey, vm);

        Assert.True(vm.PickViewportPointer(bx, by, (int)viewport.LogicalWidth, (int)viewport.LogicalHeight,
            viewport.PhysicalWidth, viewport.PhysicalHeight, viewport.DpiScale, viewport.Revision, true));

        Assert.Equal("旋转", vm.ActiveTool);
        Assert.True(vm.HasSelection);
        Assert.Equal(bKey.ToString(), vm.SelectionKey); // Selected = B
        Assert.Equal(bKey, vm.RenderSnapshot.Entity.EntityKey); // Gizmo / Inspector / Render / Transform 目标统一 = B
    }

    [Fact]
    public void Rotate_tool_switch_then_first_drag_modifies_B_not_A()
    {
        var vm = RotateVmTwoEntities(out var scene, out var bKey);
        var aKey = EntityId.FromInt(1);
        var (bx, by, viewport) = ScreenOf(scene, bKey, vm);

        // 切换选择到 B（同一次交互周期内）
        Assert.True(vm.PickViewportPointer(bx, by, (int)viewport.LogicalWidth, (int)viewport.LogicalHeight,
            viewport.PhysicalWidth, viewport.PhysicalHeight, viewport.DpiScale, viewport.Revision, true));
        Assert.Equal(bKey, vm.RenderSnapshot.Entity.EntityKey);

        // 第一次拖动：旋转 B 的 Z 环，无需第二次点击
        var hit = RingHitFor(vm, viewport);
        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        Assert.NotEqual(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
        Assert.True(scene.TryGetEntity(aKey, out var aEntity));
        Assert.Equal(0.0, aEntity.Transform.Rotation.Z); // A 不变
        Assert.Equal(1, HistoryOf(vm).Count);

        vm.RunCommand.Execute("撤销");
        Assert.Equal(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z); // Undo 只恢复 B
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    // 屏幕空间尺寸合理：同一目标在典型深度下世界半径应为正且有界，避免巨大环误触。
    [Fact]
    public void Rotate_gizmo_world_radius_is_positive_and_bounded_for_typical_depth()
    {
        var vm = RotateVmTwoEntities(out var scene, out var bKey);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        scene.TryGetEntity(bKey, out var entity);
        var r = RotateGizmoScreenRadius.ComputeWorldRadius(vm.RenderSnapshot.CameraState, viewport, entity.Transform.Position);
        Assert.True(r > 0.0);
        Assert.True(r < 5.0, $"世界半径 {r} 过大，易误触");
    }
}

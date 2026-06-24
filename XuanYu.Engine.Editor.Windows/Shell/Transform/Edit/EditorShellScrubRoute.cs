using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Windows.Panels.Inspector.Transform;
using FluidWarfare.Editor.Windows.Shell.Transform;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;

namespace FluidWarfare.Editor.Windows.Shell.Transform.Edit;

/// <summary>数值拖拽 Scrub 路由。负责 Inspector 数值拖拽事件转发。</summary>
sealed class EditorShellScrubRoute(
    EditorTransformApplyRoute transformApplyRoute,
    EditorSelectionRoute selectionRoute,
    Func<WorldState?> getWorldState,
    Func<EntityTransformCommit?> getCommitApplier,
    Action<VulkanScene3dFrameReason> scheduleFrame,
    Action<string> appendInfoLog)
{
    public void HandleScrubValueChanged(string entityId, TransformPositionAxis axis, double value)
    {
        if (selectionRoute.State.SelectedWorldEntity is null) return;
        if (selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId)
        { appendInfoLog("数值拖拽目标实体已变化，忽略本次更新。"); return; }
        var pos = getWorldState()?.FindPosition(selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var cur = pos.Value.Value;
        var newPos = axis switch
        {
            TransformPositionAxis.X => new Vector3d(value, cur.Y, cur.Z),
            TransformPositionAxis.Y => new Vector3d(cur.X, value, cur.Z),
            _ => new Vector3d(cur.X, cur.Y, value)
        };
        transformApplyRoute.Apply(
            transformApplyRoute.CurrentEntityTransform(selectionRoute, getWorldState())
                with { Position = newPos },
            EditorEntityTransformOrigin.DragScrub, selectionRoute, getWorldState(),
            getCommitApplier(), scheduleFrame, appendInfoLog);
    }

    public void HandleScrubCompleted(string entityId, TransformPositionAxis axis, double value) =>
        appendInfoLog($"数值拖拽完成：{axis} = {value:F3}");

    public void HandleScrubCancelled(string entityId, TransformPositionAxis axis, double initialValue)
    {
        if (selectionRoute.State.SelectedWorldEntity is null
            || selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId)
            return;
        var pos = getWorldState()?.FindPosition(selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var cur = pos.Value.Value;
        var restored = axis switch
        {
            TransformPositionAxis.X => new Vector3d(initialValue, cur.Y, cur.Z),
            TransformPositionAxis.Y => new Vector3d(cur.X, initialValue, cur.Z),
            _ => new Vector3d(cur.X, cur.Y, initialValue)
        };
        transformApplyRoute.Apply(
            transformApplyRoute.CurrentEntityTransform(selectionRoute, getWorldState())
                with { Position = restored },
            EditorEntityTransformOrigin.DragScrub, selectionRoute, getWorldState(),
            getCommitApplier(), scheduleFrame, appendInfoLog);
        appendInfoLog("数值拖拽已取消");
    }
}

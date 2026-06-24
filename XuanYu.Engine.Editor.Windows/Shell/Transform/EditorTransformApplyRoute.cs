using FluidWarfare.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Selection;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using FluidWarfare.Engine.World;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Transform;

/// <summary>Transform 提交 / Preview / Cancel / Inspector 编排。不持有 Shell 面板。</summary>
public sealed class EditorTransformApplyRoute
{
    public EditorTransformApplyResult Apply(
        SceneTransform transform, EditorEntityTransformOrigin origin,
        EditorSelectionRoute selection, WorldState? world,
        EntityTransformCommit? commit, Action<VulkanScene3dFrameReason> scheduleFrame,
        Action<string> infoLog)
    {
        if (selection.State.SelectedWorldEntity is null || commit is null) return new(false, false);
        commit.Apply(transform, selection.State.SelectedWorldEntity.EntityId);
        scheduleFrame(VulkanScene3dFrameReason.EntityTransformChanged);
        if (origin != EditorEntityTransformOrigin.DragScrub && origin != EditorEntityTransformOrigin.MoveTool)
            infoLog($"实体 {selection.State.SelectedWorldEntity.DisplayName} 坐标修改为 ({transform.Position.X:F2}, {transform.Position.Y:F2}, {transform.Position.Z:F2})。");
        return new(true, true);
    }

    public EditorTransformApplyResult Preview(
        EditorSelectionRoute selection, EntityTransformPreview? preview,
        TransformPointerRoute pointer, Action<VulkanScene3dFrameReason> scheduleFrame)
    {
        if (selection.State.SelectedWorldEntity is null || preview is null) return new(false, false);
        preview.Apply(pointer.Session.PreviewTransform, selection.State.SelectedWorldEntity.EntityId);
        scheduleFrame(VulkanScene3dFrameReason.TransformPreview);
        return new(true, true);
    }

    public EditorTransformApplyResult Cancel(
        TransformInteractionResult r,
        EditorSelectionRoute selection, EntityTransformCancel? cancel,
        Action<VulkanScene3dFrameReason> scheduleFrame)
    {
        if (r.Action != TransformInteractionAction.Cancelled || selection.State.SelectedWorldEntity is null) return new(false, false);
        cancel?.Apply(r.Transform, selection.State.SelectedWorldEntity.EntityId);
        scheduleFrame(VulkanScene3dFrameReason.TransformPreview);
        return new(true, true);
    }

    public SceneTransform CurrentEntityTransform(EditorSelectionRoute selection, WorldState? world)
    {
        if (selection.State.SelectedWorldEntity is null) return default;
        var pos = world?.FindPosition(selection.State.SelectedWorldEntity.EntityId);
        return pos is not null ? new SceneTransform(pos.Value.Value, default, default) : default;
    }

    public EditorTransformApplyResult HandleInspectorApply(
        string xText, string yText, string zText,
        EditorSelectionRoute selection, WorldState? world,
        EntityTransformCommit? commit, Action<VulkanScene3dFrameReason> scheduleFrame,
        Action<string> infoLog, Action<string> showError)
    {
        if (selection.State.SelectedWorldEntity is null) return new(false, false);
        if (!EditorEntityTransformValidation.TryParse(xText, yText, zText, out var newPos, out var error))
        { showError(error); return new(false, false); }
        return Apply(CurrentEntityTransform(selection, world) with { Position = newPos }, EditorEntityTransformOrigin.InspectorInput, selection, world, commit, scheduleFrame, infoLog);
    }
}

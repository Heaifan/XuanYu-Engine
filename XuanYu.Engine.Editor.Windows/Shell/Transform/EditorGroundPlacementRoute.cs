using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Selection;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.World;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Transform;

public sealed class EditorGroundPlacementRoute
{
    public bool Toggle(
        EditorSelectionRoute selection, EditorGroundPlacementState state,
        bool sessionActive, Scene3dSessionLifecycle lifecycle,
        InspectorPanel? inspector, StatusBarPanel? statusBar,
        Action<string> warnLog)
    {
        if (selection.State.SelectedWorldEntity is null) return false;
        if (!sessionActive || lifecycle.State.Session?.Status != VulkanScene3dSessionStatus.Active)
        { warnLog("Scene3D 未激活，无法进入放置模式。"); return false; }

        if (state.IsActive)
        { state.Cancel(); inspector?.SetPlacementMode(false); statusBar?.SetCurrentSelection(selection.State.SelectedWorldEntity?.DisplayName ?? "无"); }
        else
        { state.Begin(selection.State.SelectedWorldEntity.EntityId.Value.ToString()); inspector?.SetPlacementMode(true); }
        return true;
    }

    public EditorGroundPlacementResult Complete(
        Vector3d groundPosition,
        EditorSelectionRoute selection, EditorGroundPlacementState state,
        WorldState? world, EntityTransformCommit? commit,
        Action<VulkanScene3dFrameReason> scheduleFrame,
        InspectorPanel? inspector,
        Action<string> infoLog)
    {
        if (!state.IsActive || selection.State.SelectedWorldEntity is null) return new(false, false);
        var entityPos = new Vector3d(groundPosition.X, groundPosition.Y, 0);
        var cur = selection.State.SelectedWorldEntity;
        var pos = world?.FindPosition(cur.EntityId);
        if (pos is null) return new(false, false);
        var transform = new SceneTransform(entityPos, default, default);
        commit?.Apply(transform, cur.EntityId);
        scheduleFrame(VulkanScene3dFrameReason.EntityTransformChanged);
        if (state.IsActive)
        {
            state.Complete();
            inspector?.SetPlacementMode(false);
            infoLog($"实体 {cur.DisplayName} 已放置到 X {entityPos.X:F2}，Y {entityPos.Y:F2}，Z {entityPos.Z:F2}。");
        }
        return new(true, true);
    }
}

using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Selection;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Shell.Transform;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;
using XuanYu.Engine.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Shell.Transform.Edit;

/// <summary>Transform 编辑路由。负责 Inspector Transform 面板事件转发。</summary>
sealed class EditorShellTransformRoute(
    EditorTransformApplyRoute transformApplyRoute,
    EditorSelectionRoute selectionRoute,
    EditorGroundPlacementState groundPlacementState,
    EditorGroundPlacementRoute groundPlacementRoute,
    InspectorPanel? inspectorPanel,
    StatusBarPanel? statusBarPanel,
    Func<WorldState?> getWorldState,
    Func<bool> isSessionActive,
    Scene3dSessionLifecycle lifecycle,
    Func<EntityTransformCommit?> getCommitApplier,
    Action<VulkanScene3dFrameReason> scheduleFrame,
    Action<string> appendInfoLog,
    Action<string> appendWarningLog)
{
    public void HandleTransformApply(string xText, string yText, string zText)
    {
        transformApplyRoute.HandleInspectorApply(xText, yText, zText, selectionRoute, getWorldState(),
            getCommitApplier(), scheduleFrame, appendInfoLog,
            err => { if (inspectorPanel is not null) inspectorPanel.ShowTransformError(err); });
    }

    public void HandleTransformReset()
    {
        if (selectionRoute.State.SelectedWorldEntity is null) return;
        var pos = getWorldState()?.FindPosition(selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var v = pos.Value.Value;
        inspectorPanel?.SetTransformTexts(
            v.X.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            v.Y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            v.Z.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        inspectorPanel?.SetTransformDraftState(false, false, null);
    }

    public void HandleTransformDraftChanged(string xText, string yText, string zText)
    {
        if (selectionRoute.State.SelectedWorldEntity is null)
        {
            inspectorPanel?.SetTransformDraftState(false, false, null);
            return;
        }

        if (!EditorEntityTransformValidation.TryParse(xText, yText, zText,
                out var newPos, out var error))
        {
            inspectorPanel?.SetTransformDraftState(false, true, error);
            return;
        }

        var currentPos = getWorldState()?.FindPosition(selectionRoute.State.SelectedWorldEntity.EntityId);
        if (currentPos is null)
        {
            inspectorPanel?.SetTransformDraftState(false, false, null);
            return;
        }

        var changed = newPos != currentPos.Value.Value;
        inspectorPanel?.SetTransformDraftState(
            canApply: changed && !groundPlacementState.IsActive,
            canReset: changed,
            error: null);
    }

    public void HandleGroundPlacementToggle()
    {
        groundPlacementRoute.Toggle(selectionRoute, groundPlacementState, isSessionActive(),
            lifecycle, inspectorPanel, statusBarPanel, appendWarningLog);
    }

    public void ApplyEntityTransform(SceneTransform transform, EditorEntityTransformOrigin origin)
    {
        transformApplyRoute.Apply(transform, origin, selectionRoute, getWorldState(),
            getCommitApplier(), scheduleFrame, appendInfoLog);
    }

    public SceneTransform GetCurrentEntityTransform()
    {
        return transformApplyRoute.CurrentEntityTransform(selectionRoute, getWorldState());
    }
}

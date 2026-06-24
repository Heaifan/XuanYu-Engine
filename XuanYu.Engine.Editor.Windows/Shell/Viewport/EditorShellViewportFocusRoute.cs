using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Shell.Panels;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Selection.Focus;
using FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Viewport;

/// <summary>视口焦点路由。负责 Viewport 聚焦事件和世界实体选择展示转发。</summary>
sealed class EditorShellViewportFocusRoute(
    ViewportFocusSelectionRoute viewportFocusRoute,
    EditorPanelApplyRoute panelApplyRoute,
    WorldEntitySelectionPresenter worldSelectionPresenter,
    ViewportRenderSceneStore renderSceneStore,
    Func<bool> isSessionActive,
    Scene3dSessionLifecycle lifecycle,
    Func<WorldState?> getWorldState,
    EditorSelectionRoute selectionRoute,
    Action<string> appendInfoLog,
    Action<string> appendWarningLog)
{
    public void HandleViewportFocused()
    {
        var r = viewportFocusRoute.Focus(getWorldState(), selectionRoute);
        panelApplyRoute.ShowViewportFocused(r.InspectorSelection, r.StatusBarText, r.ShowEmptyWorld);
        foreach (var m in r.LogMessages) appendInfoLog(m);
        foreach (var w in r.LogWarnings) appendWarningLog(w);
        if (r.EntityToShow is not null) ShowWorldEntitySelection(r.EntityToShow);
    }

    public void ShowWorldEntitySelection(WorldEntityInfo entityInfo)
    {
        var is3d = isSessionActive() && lifecycle.State.Session?.Status == VulkanScene3dSessionStatus.Active;
        var r = worldSelectionPresenter.Present(entityInfo, getWorldState(), renderSceneStore.Current, is3d);
        panelApplyRoute.ApplyEntitySelection(r.InspectorSelection, r.InspectorEntityId, r.EntityPosition,
            r.EntitySourcePath, r.GroundPlaceEnabled, r.StatusBarSelection,
            r.ViewportSummary, r.LogMessage, appendInfoLog);
    }
}

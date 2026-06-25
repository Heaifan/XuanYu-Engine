using XuanYu.Engine.Editor.Selection;
using XuanYu.Engine.Editor.Windows.Shell.Panels;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Shell.Selection;

/// <summary>选择同步路由。负责实体选择在视口/WorldTree/Inspector/Scene 之间的联动同步。</summary>
sealed class EditorShellSelectionSyncRoute(
    EditorSelectionRoute selectionRoute,
    EditorPanelApplyRoute panelApplyRoute,
    Func<WorldState?> getWorldState,
    Func<bool> isSessionActive,
    Scene3dSessionLifecycle lifecycle,
    Action<VulkanScene3dFrameReason> scheduleFrame)
{
    public void ApplyEntitySelection(string? entityIdStr, EditorEntitySelectionOrigin origin,
        Action<WorldEntityInfo> showWorldEntitySelection)
    {
        var reason = MapReason(origin);
        var result = entityIdStr is null
            ? selectionRoute.ClearSelection(reason)
            : selectionRoute.SelectEntity(new(entityIdStr, reason, getWorldState()));
        if (!result.IsChanged) return;
        if (result.Entity is not null)
        {
            EditorProbe.Write("Selection", "Changed",
                $"origin={origin} entity={result.Entity.EntityId.Value} name={result.Entity.DisplayName}");
            showWorldEntitySelection(result.Entity);
            SyncSceneSelection(result.Entity.EntityId.Value.ToString());
        }
        else
            panelApplyRoute.ClearSelection();
    }

    public void SyncSceneSelection(string entityId)
    {
        if (lifecycle.State.Session is null || !isSessionActive()) return;
        if (lifecycle.State.Session.SetSelectedEntity(entityId))
        {
            EditorProbe.Write("Selection", "SceneSync", $"entity={entityId} schedule=SelectionChanged");
            scheduleFrame(VulkanScene3dFrameReason.SelectionChanged);
        }
    }

    public void ClearSelection()
    {
        selectionRoute.ClearSelection(EditorSelectionReason.SelectionRestore);
        panelApplyRoute.ClearSelection();
    }

    static EditorSelectionReason MapReason(EditorEntitySelectionOrigin o) => o switch
    {
        EditorEntitySelectionOrigin.ViewportPicking => EditorSelectionReason.ViewportPicking,
        EditorEntitySelectionOrigin.WorldHierarchy => EditorSelectionReason.WorldHierarchy,
        _ => EditorSelectionReason.ViewportFocused,
    };
}

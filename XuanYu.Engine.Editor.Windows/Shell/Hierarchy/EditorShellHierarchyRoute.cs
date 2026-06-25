using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Editor.ProjectContentTreeModel;
using XuanYu.Engine.Editor.WorldHierarchy;
using XuanYu.Engine.Editor.Windows.Panels.LeftDock;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Project.Metadata;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;

using XuanYu.Engine.Editor.Windows.Panels.LeftDock;
namespace XuanYu.Engine.Editor.Windows.Shell.Hierarchy;

/// <summary>层级树路由。负责世界层级树与项目内容树构建显示。</summary>
sealed class EditorShellHierarchyRoute(
    ProjectWorldDockPanel? dockPanel,
    Func<GameProjectInfo?> getProjectInfo,
    Func<WorldState?> getWorldState,
    ViewportRenderSceneStore renderSceneStore,
    Action<string> appendErrorLog)
{
    public void RebuildAndShowHierarchy()
    {
        var projectInfo = getProjectInfo();
        var worldState = getWorldState();
        if (projectInfo is not null)
            try { dockPanel?.ShowProjectContent(ProjectContentTreeBuilder.Build(projectInfo)); }
            catch (Exception ex) { appendErrorLog($"项目内容树构建失败：{ex.Message}"); }
        if (worldState is not null)
            try { GizmoDragProbe.MarkUiRefreshed(); GizmoDragProbe.Log("WorldHierarchy 刷新"); dockPanel?.ShowWorldHierarchy(WorldHierarchyTreeBuilder.Build(worldState, BuildGroupLookup())); }
            catch (Exception ex) { appendErrorLog($"世界层级树构建失败：{ex.Message}"); }
    }

    Dictionary<EntityId, string>? BuildGroupLookup()
    {
        if (renderSceneStore.Current.Objects.Count == 0) return null;
        var map = new Dictionary<EntityId, string>();
        foreach (var obj in renderSceneStore.Current.Objects)
            map[obj.EntityId] = obj.VisualKind == RenderObjectVisualKind.UnitMarker ? "单位" : "其他";
        return map;
    }
}

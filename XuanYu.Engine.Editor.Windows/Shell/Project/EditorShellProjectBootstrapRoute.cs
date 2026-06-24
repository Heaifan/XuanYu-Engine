using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Hierarchy;
using XuanYu.Engine.Editor.Windows.Shell.Panels;
using XuanYu.Engine.Editor.Windows.Shell.Startup;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Presentation;
using XuanYu.Engine.World;
using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Project.Metadata;

namespace XuanYu.Engine.Editor.Windows.Shell.Project;

/// <summary>项目加载 + World Bootstrap 路由。负责项目加载、加载结果应用、World 初始化。</summary>
sealed class EditorShellProjectBootstrapRoute(
    EditorStartupBootstrapRoute startupRoute,
    EditorPanelApplyRoute panelApplyRoute,
    EditorShellHierarchyRoute hierarchyRoute,
    ViewportSelectionPresenter viewportSelectionPresenter,
    Action<string> appendInfoLog,
    Action<string> appendWarningLog,
    Action<string> appendErrorLog,
    Action<GameProjectInfo?> setProjectInfo,
    Action<IReadOnlyList<GameContentFileInfo>?> setContentFiles,
    Action<WorldState?> setWorldState)
{
    public void LoadSampleProject()
    {
        var result = startupRoute.LoadSampleProject();
        ApplyBootstrapResult(result);
    }

    void ApplyBootstrapResult(EditorStartupBootstrapResult result)
    {
        if (!result.Success)
        {
            panelApplyRoute.ShowProjectLoadFailure(result.FailureMessage ?? "未知错误", appendErrorLog);
            return;
        }

        setProjectInfo(result.Project);
        setContentFiles(result.Project?.ContentFiles);
        hierarchyRoute.RebuildAndShowHierarchy();

        setWorldState(result.WorldResult?.World);
        var summary = result.WorldResult is not null
            ? viewportSelectionPresenter.CreateRenderSceneSummary(result.WorldResult.RenderScene)
            : ViewportRenderSceneSummary.Empty;
        panelApplyRoute.ApplyStartupWorld(new(result.WorldResult?.HasEntities ?? false, summary));

        foreach (var m in result.LogMessages) appendInfoLog(m);
        foreach (var w in result.LogWarnings) appendWarningLog(w);
    }
}

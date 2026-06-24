using XuanYu.Engine.Editor.Windows.Viewport.Project;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.World.Bootstrap;

namespace XuanYu.Engine.Editor.Windows.Shell.Startup;

/// <summary>启动引导编排路由。协调 ProjectBootstrapRoute、WorldBootstrapRoute、ViewportRenderSceneStore、EditorSelectionRoute。</summary>
public sealed class EditorStartupBootstrapRoute
{
    private readonly ProjectBootstrapRoute _projectBootstrap;
    private readonly WorldBootstrapRoute _worldBootstrap;
    private readonly ViewportRenderSceneStore _renderSceneStore;
    private readonly EditorSelectionRoute _selectionRoute;

    public EditorStartupBootstrapRoute(
        ProjectBootstrapRoute projectBootstrap,
        WorldBootstrapRoute worldBootstrap,
        ViewportRenderSceneStore renderSceneStore,
        EditorSelectionRoute selectionRoute)
    {
        _projectBootstrap = projectBootstrap;
        _worldBootstrap = worldBootstrap;
        _renderSceneStore = renderSceneStore;
        _selectionRoute = selectionRoute;
    }

    public EditorStartupBootstrapResult LoadSampleProject()
    {
        var result = _projectBootstrap.LoadSampleProject();
        if (!result.Success)
            return EditorStartupBootstrapResult.Failed(result.LogMessage);

        var project = _projectBootstrap.Project;
        if (project is null)
            return EditorStartupBootstrapResult.Failed("项目加载后状态为空。");

        EditorStartupWorldResult? worldResult = null;
        var contentFiles = project.ContentFiles;
        if (contentFiles is { Count: > 0 })
        {
            var input = new WorldBootstrapInput(project, contentFiles);
            var buildResult = _worldBootstrap.Build(input);
            if (buildResult.HasEntities)
            {
                _renderSceneStore.Initialize(buildResult.RenderScene);
                _selectionRoute.State.SetFirstEntityId(buildResult.FirstEntityId);
                worldResult = EditorStartupWorldResult.Created(
                    buildResult.World, buildResult.RenderScene,
                    buildResult.FirstEntityId, buildResult.SeedSourcePaths);
            }
        }

        return EditorStartupBootstrapResult.Succeeded(project, worldResult ?? EditorStartupWorldResult.Empty);
    }
}

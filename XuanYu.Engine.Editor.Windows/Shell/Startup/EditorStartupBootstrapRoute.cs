using XuanYu.Engine.Editor.Windows.Viewport.Project;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.World;
using XuanYu.Engine.Editor.Windows.Viewport.World.Bootstrap;
using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Project.Metadata;
using XuanYu.Engine.Project.World.SaveLoad;
using XuanYu.Engine.Project.World.Validation;

namespace XuanYu.Engine.Editor.Windows.Shell.Startup;

/// <summary>启动引导编排路由。协调项目加载、World 文件加载与内容实体播种。</summary>
public sealed class EditorStartupBootstrapRoute
{
    readonly ProjectBootstrapRoute _projectBootstrap;
    readonly WorldBootstrapRoute _worldBootstrap;
    readonly ViewportRenderSceneStore _renderSceneStore;
    readonly EditorSelectionRoute _selectionRoute;

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

        // Phase 1: 尝试从 World 文件加载
        var projectDir = _projectBootstrap.ProjectDirectory;
        if (projectDir is not null)
        {
            worldResult = LoadWorldFromFile(projectDir);
            if (worldResult is not null && worldResult.HasEntities)
            {
                _renderSceneStore.Initialize(worldResult.RenderScene);
                _selectionRoute.State.SetFirstEntityId(worldResult.FirstEntityId);
                return EditorStartupBootstrapResult.Succeeded(project, worldResult);
            }
        }

        // Phase 2: 无 World 文件时从项目内容生成
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

    EditorStartupWorldResult? LoadWorldFromFile(string projectDir)
    {
        var worldPath = Path.Combine(projectDir, "Content", "Worlds", "main.world.json");
        var readResult = WorldDocumentReader.Read(worldPath);
        if (!readResult.IsSuccess || readResult.Document is null)
            return null;

        var report = WorldDocumentValidator.Validate(readResult.Document);
        if (!report.IsValid)
            return null;

        var world = WorldStateDocumentConvert.ToWorldState(readResult.Document);
        var renderScene = WorldBootstrapRenderSeed.BuildRenderScene(world);
        var entities = world.ListEntities();
        var firstId = entities.Count > 0 ? entities[0].EntityId : default;

        return new EditorStartupWorldResult(
            world, renderScene, firstId, [],
            [$"已加载 World 文件：{worldPath}，实体数量：{entities.Count}。"],
            []);
    }
}

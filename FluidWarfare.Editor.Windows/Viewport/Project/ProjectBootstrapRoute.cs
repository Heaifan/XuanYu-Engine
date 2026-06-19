using FluidWarfare.Project.Loading;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Paths;
using FluidWarfare.Project.Validation;

namespace FluidWarfare.Editor.Windows.Viewport.Project;

/// <summary>项目加载路由。处理项目发现、加载、校验逻辑。不持有 UI 控件引用。</summary>
public sealed class ProjectBootstrapRoute
{
    public GameProjectInfo? Project { get; private set; }

    public ProjectBootstrapResult LoadSampleProject()
    {
        var pathResult = SampleProjectPath.TryFindFrom(
            Environment.CurrentDirectory, out var projectDir);

        if (!pathResult.IsSuccess)
            return ProjectBootstrapResult.Failed(
                pathResult.Error?.Message ?? "未知错误。");

        var loadResult = GameProjectLoader.LoadFromDirectory(projectDir);

        if (loadResult.Result.IsSuccess && loadResult.Project is not null)
        {
            Project = loadResult.Project;
            return new ProjectBootstrapResult(true, "info",
                $"已加载示例项目：{loadResult.Project.DisplayName}。",
                loadResult.Project.DisplayName);
        }

        return ProjectBootstrapResult.Failed(
            loadResult.Result.Error?.Message ?? "未知错误。");
    }
}

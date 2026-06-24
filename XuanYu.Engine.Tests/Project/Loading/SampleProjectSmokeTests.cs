using XuanYu.Engine.Project.Loading;

namespace FluidWarfare.Tests.Project.Loading;

public sealed class SampleProjectSmokeTests
{
    [Fact]
    public void RepositorySampleProject_ShouldLoadSuccessfully()
    {
        var sampleProjectPath = FindRepositoryPath("GameProjects", "SampleProject");

        var result = GameProjectLoader.LoadFromDirectory(sampleProjectPath);

        Assert.True(result.Result.IsSuccess);
        Assert.NotNull(result.Project);
        Assert.Equal(1, result.Project.SchemaVersion);
        Assert.Equal("sample_project", result.Project.ProjectId);
        Assert.False(result.ValidationReport.HasIssues);
    }

    [Fact]
    public void RepositorySampleProject_ShouldExposeContentFoldersAndFiles()
    {
        var sampleProjectPath = FindRepositoryPath("GameProjects", "SampleProject");

        var result = GameProjectLoader.LoadFromDirectory(sampleProjectPath);

        Assert.NotNull(result.Project);
        Assert.Contains(result.Project.ContentFolders, f => f.FolderName == "units" && f.ContentKind == "unitTemplate");
        Assert.Contains(result.Project.ContentFolders, f => f.FolderName == "icons" && f.ContentKind == "icon");
        Assert.Contains(result.Project.ContentFiles, f => f.RelativePath == "units/sample_unit.json");
        Assert.Contains(result.Project.ContentFiles, f => f.RelativePath == "icons/sample_icon.svg");
        Assert.False(result.ValidationReport.HasIssues);
    }

    private static string FindRepositoryPath(params string[] segments)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "XuanYu.Engine.sln");
            if (File.Exists(solutionPath))
            {
                return Path.Combine([current.FullName, .. segments]);
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("未找到 XuanYu Engine 仓库根目录。");
    }
}

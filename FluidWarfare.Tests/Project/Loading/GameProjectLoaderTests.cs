using FluidWarfare.Project.Loading;

namespace FluidWarfare.Tests.Project.Loading;

public sealed class GameProjectLoaderTests
{
    [Fact]
    public void LoadFromDirectory_WithValidProject_ShouldSucceed()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "projectId": "test_project",
              "displayName": "TestProject",
              "description": "测试项目。",
              "contentFolders": [ "units", "weapons" ]
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsSuccess);
        Assert.NotNull(result.Project);
        Assert.Equal("test_project", result.Project.ProjectId);
        Assert.Equal("TestProject", result.Project.DisplayName);
        Assert.Equal(["units", "weapons"], result.Project.ContentFolders);
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDirectory_ShouldFail()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var result = GameProjectLoader.LoadFromDirectory(missingPath);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal("Project.DirectoryMissing", result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithMissingManifest_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal("Project.ManifestMissing", result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithInvalidJson_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("{ invalid json");

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal("Project.ManifestInvalid", result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithMissingProjectId_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "displayName": "TestProject",
              "contentFolders": [ "units" ]
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal("Project.ProjectIdMissing", result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDisplayName_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "projectId": "test_project",
              "contentFolders": [ "units" ]
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal("Project.DisplayNameMissing", result.Result.Error?.Code);
    }

    private sealed class ProjectTestDirectory : IDisposable
    {
        private ProjectTestDirectory(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static ProjectTestDirectory Create()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"FluidWarfareProjectTests_{Guid.NewGuid():N}");

            Directory.CreateDirectory(path);
            return new ProjectTestDirectory(path);
        }

        public void WriteManifest(string json)
        {
            File.WriteAllText(System.IO.Path.Combine(Path, "game.project.json"), json);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}

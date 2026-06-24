using XuanYu.Engine.Project.Paths;

namespace FluidWarfare.Tests.Project.Paths;

public sealed class SampleProjectPathTests
{
    [Fact]
    public void TryFindFrom_WithDirectRepositoryRoot_ShouldSucceed()
    {
        using var scope = SampleProjectTree.Create(withManifest: true);

        var result = SampleProjectPath.TryFindFrom(scope.RootPath, out var projectDirectory);

        Assert.True(result.IsSuccess);
        Assert.Equal(scope.SampleProjectPath, projectDirectory);
    }

    [Fact]
    public void TryFindFrom_WithNestedDirectory_ShouldWalkUpAndSucceed()
    {
        using var scope = SampleProjectTree.Create(withManifest: true);
        var nestedPath = Path.Combine(scope.RootPath, "FluidWarfare.Editor.Windows", "bin", "Debug");
        Directory.CreateDirectory(nestedPath);

        var result = SampleProjectPath.TryFindFrom(nestedPath, out var projectDirectory);

        Assert.True(result.IsSuccess);
        Assert.Equal(scope.SampleProjectPath, projectDirectory);
    }

    [Fact]
    public void TryFindFrom_WithMissingSampleProject_ShouldFail()
    {
        using var scope = SampleProjectTree.Create(withManifest: false);

        var result = SampleProjectPath.TryFindFrom(scope.RootPath, out var projectDirectory);

        Assert.True(result.IsFailure);
        Assert.Equal(string.Empty, projectDirectory);
        Assert.Equal("Project.SampleProjectMissing", result.Error?.Code);
    }

    [Fact]
    public void TryFindFrom_WithEmptyStartDirectory_ShouldFail()
    {
        var result = SampleProjectPath.TryFindFrom(" ", out var projectDirectory);

        Assert.True(result.IsFailure);
        Assert.Equal(string.Empty, projectDirectory);
        Assert.Equal("Project.StartDirectoryMissing", result.Error?.Code);
    }

    private sealed class SampleProjectTree : IDisposable
    {
        private SampleProjectTree(string rootPath, string sampleProjectPath)
        {
            RootPath = rootPath;
            SampleProjectPath = sampleProjectPath;
        }

        public string RootPath { get; }

        public string SampleProjectPath { get; }

        public static SampleProjectTree Create(bool withManifest)
        {
            var rootPath = Path.Combine(
                Path.GetTempPath(),
                $"FluidWarfareSampleProjectPathTests_{Guid.NewGuid():N}");

            var sampleProjectPath = Path.Combine(rootPath, "GameProjects", "SampleProject");
            Directory.CreateDirectory(sampleProjectPath);

            if (withManifest)
            {
                File.WriteAllText(Path.Combine(sampleProjectPath, "game.project.json"), "{}");
            }

            return new SampleProjectTree(rootPath, sampleProjectPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }
    }
}

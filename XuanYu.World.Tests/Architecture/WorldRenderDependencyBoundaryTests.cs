namespace XuanYu.World.Tests.Architecture;

public sealed class WorldRenderDependencyBoundaryTests
{
    [Fact]
    public void World_project_does_not_reference_render_abstractions()
    {
        var root = FindRoot();
        var project = File.ReadAllText(Path.Combine(root, "XuanYu.World", "XuanYu.World.csproj"));

        Assert.DoesNotContain("XuanYu.Render.Abstractions", project,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void World_terrain_sources_do_not_own_render_projection_types()
    {
        var root = FindRoot();
        var directory = Path.Combine(root, "XuanYu.World", "Terrain");
        var forbidden = new[]
        {
            "XuanYu.Render.Abstractions", "TerrainRenderResource",
            "TerrainRenderMetadata", "TerrainRenderExtent", "TerrainHeightfield"
        };

        foreach (var file in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
        foreach (var token in forbidden)
            Assert.DoesNotContain(token, File.ReadAllText(file), StringComparison.Ordinal);
    }

    static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}

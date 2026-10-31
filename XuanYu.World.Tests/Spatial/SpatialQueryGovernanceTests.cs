namespace XuanYu.World.Tests.World;

public sealed class SpatialQueryGovernanceTests
{
    [Fact]
    public void Production_world_query_does_not_scan_global_entities()
    {
        var root = FindRepoRoot();
        var files = new[]
        {
            Path.Combine(root, "XuanYu.World", "WorldState", "World", "GlobalWorld.Query.cs"),
            Path.Combine(root, "XuanYu.World", "Spatial", "Query", "WorldQuery.cs")
        };

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain(".Entities", text);
            Assert.DoesNotContain("_registry", text);
        }
    }

    static string FindRepoRoot()
        => TestSourceRoot.Root;
}

namespace XuanYu.Core.Tests.World;

public sealed class WorldSpatialQueryGovernanceTests
{
    [Fact]
    public void Production_world_query_does_not_scan_global_entities()
    {
        var root = FindRepoRoot();
        var files = new[]
        {
            Path.Combine(root, "XuanYu.Core", "World", "GlobalWorld.Query.cs"),
            Path.Combine(root, "XuanYu.Core", "World", "WorldQuery.cs")
        };

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain(".Entities", text);
            Assert.DoesNotContain("foreach (var entity", text);
        }
    }

    static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "XuanYu.Engine.slnx"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("无法定位仓库根目录。");
    }
}

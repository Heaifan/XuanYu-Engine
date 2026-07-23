using System.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.World;

namespace XuanYu.Core.Tests.World;

public sealed class WorldPartitionR2Tests
{
    [Fact]
    public void Partition_invariant_holds_after_every_random_migration()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var createWatch = Stopwatch.StartNew();
        var entities = Enumerable.Range(0, 1000).Select(i => world.Create(
            $"Entity {i}",
            transform: new CommittedTransform(new Vector3d(i % 100 * 10, i / 100 * 10, 0)))).ToArray();
        createWatch.Stop();
        var random = new Random(20260723);
        var migrateWatch = Stopwatch.StartNew();

        for (var i = 0; i < 10000; i++)
        {
            var entity = entities[random.Next(entities.Length)];
            var p = new Vector3d(random.Next(100) * 10 + 1, random.Next(100) * 10 + 1, 0);
            Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(p)));
            AssertPartitionInvariant(world);
        }

        migrateWatch.Stop();
        Debug.WriteLine(
            $"WORLD-A-R2-R2 1000实体迁移：CreateTicks={createWatch.ElapsedTicks}; " +
            $"MigrationTicks={migrateWatch.ElapsedTicks}; LookupCount={world.PartitionSnapshot.Count}");
    }

    [Fact]
    public void Dormant_entity_remains_queryable_and_keeps_region()
    {
        var world = new GlobalWorld();
        var entity = world.Create("Dormant");
        Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(new Vector3d(1001, 0, 0))));

        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Dormant));

        Assert.True(world.Exists(entity.EntityKey));
        Assert.True(world.TryGet(entity.EntityKey, out var snapshot));
        Assert.Equal(entity.EntityKey, snapshot.EntityKey);
        Assert.Equal(RegionKey.FromGrid(1, 0), snapshot.RegionKey);
        Assert.Contains(entity.EntityKey, world.EntitiesIn(snapshot.RegionKey));
        AssertPartitionInvariant(world);
    }

    [Fact]
    public void Region_grid_geometry_is_only_used_by_partition_strategy()
    {
        var root = FindRepoRoot();
        var files = Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains(".git", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains("Tests", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.EndsWith("RegionKey.cs", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.EndsWith("GridWorldPartitionStrategy.cs", StringComparison.OrdinalIgnoreCase));

        foreach (var file in files)
            Assert.DoesNotContain("RegionKey.FromGrid", File.ReadAllText(file));
    }

    static void AssertPartitionInvariant(GlobalWorld world)
    {
        var memberships = world.PartitionSnapshot.GroupBy(item => item.EntityKey).ToDictionary(g => g.Key, g => g.ToArray());
        Assert.Equal(world.EntityCount, memberships.Count);
        foreach (var entity in world.Entities)
        {
            Assert.True(memberships.TryGetValue(entity.EntityKey, out var entries));
            Assert.Single(entries);
            Assert.Equal(world.ResolveRegion(entity.Transform), entries[0].RegionKey);
            Assert.Equal(entity.RegionKey, entries[0].RegionKey);
            Assert.Equal(entity.Activity, entries[0].Activity);
        }
    }

    static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "XuanYu.Engine.slnx"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("无法定位仓库根目录。");
    }
}

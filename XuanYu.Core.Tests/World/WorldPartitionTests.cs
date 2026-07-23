using System.Diagnostics;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.World;

namespace XuanYu.Core.Tests.World;

public sealed class WorldPartitionTests
{
    [Fact]
    public void Region_membership_does_not_own_entity_lifecycle()
    {
        var world = new GlobalWorld();
        var b = RegionKey.FromGrid(1, 0);
        var entity = world.Create("Soldier");

        Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(new Vector3d(1001, 0, 0))));

        Assert.True(world.Exists(entity.EntityKey));
        Assert.DoesNotContain(entity.EntityKey, world.EntitiesIn(RegionKey.Origin));
        Assert.Contains(entity.EntityKey, world.EntitiesIn(b));
        Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
    }

    [Fact]
    public void Global_position_updates_region_without_changing_identity()
    {
        var world = new GlobalWorld();
        var entity = world.Create("Scout");

        Assert.True(world.UpdateTransform(
            entity.EntityKey,
            new CommittedTransform(new Vector3d(1200, -1, 0))));

        var next = world.Get(entity.EntityKey);
        Assert.Equal(entity.EntityKey, next.EntityKey);
        Assert.Equal(new Vector3d(1200, -1, 0), next.GlobalPosition);
        Assert.Equal(RegionKey.FromGrid(1, -1, 0), next.RegionKey);
        Assert.Contains(entity.EntityKey, world.EntitiesIn(next.RegionKey));
    }

    [Fact]
    public void Partition_strategy_can_be_replaced_without_changing_entity_owner()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("Scout");

        Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(new Vector3d(11, 0, 0))));

        Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
        Assert.Equal(RegionKey.FromGrid(1, 0), world.GetRegion(entity.EntityKey));
    }

    [Fact]
    public void Activity_changes_are_not_destroy_or_recreate()
    {
        var world = new GlobalWorld();
        var entity = world.Create("Reserve");

        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Dormant));
        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Active));
        Assert.False(world.SetActivity(entity.EntityKey, WorldEntityActivity.Externalized));

        var next = world.Get(entity.EntityKey);
        Assert.Equal(entity.EntityKey, next.EntityKey);
        Assert.Equal(WorldEntityActivity.Active, next.Activity);
        Assert.True(world.Exists(entity.EntityKey));
    }

    [Fact]
    public void Thousand_entities_can_migrate_regions_without_identity_mix()
    {
        var world = new GlobalWorld();
        var entities = Enumerable.Range(0, 1000)
            .Select(i => world.Create($"Entity {i}"))
            .ToArray();
        var watch = Stopwatch.StartNew();

        foreach (var entity in entities)
        {
            var p = new Vector3d(entity.EntityKey.Value % 13 * 1000, entity.EntityKey.Value % 7 * 1000, 0);
            Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(p)));
        }

        watch.Stop();
        foreach (var entity in entities)
        {
            var expected = RegionKey.FromGrid(entity.EntityKey.Value % 13, entity.EntityKey.Value % 7);
            Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
            Assert.Equal(expected, world.GetRegion(entity.EntityKey));
        }
        var regions = entities.Select(item => world.GetRegion(item.EntityKey)).Distinct().ToArray();
        foreach (var entity in entities)
        {
            var ownedCount = regions.Sum(region => world.EntitiesIn(region).Count(item => item == entity.EntityKey));
            Assert.Equal(1, ownedCount);
        }
        Debug.WriteLine($"WORLD-A-R2 1000实体分区迁移Ticks={watch.ElapsedTicks}");
    }
}

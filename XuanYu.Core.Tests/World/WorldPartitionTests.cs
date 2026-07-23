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
        var a = RegionKey.FromGrid(0, 0);
        var b = RegionKey.FromGrid(1, 0);
        var entity = world.Create("Soldier", region: a);

        Assert.True(world.MoveToRegion(entity.EntityKey, b));

        Assert.True(world.Exists(entity.EntityKey));
        Assert.DoesNotContain(entity.EntityKey, world.EntitiesIn(a));
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
    public void Activity_changes_are_not_destroy_or_recreate()
    {
        var world = new GlobalWorld();
        var entity = world.Create("Reserve");

        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Dormant));
        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Externalized));
        Assert.True(world.SetActivity(entity.EntityKey, WorldEntityActivity.Active));

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
            .Select(i => world.Create($"Entity {i}", region: RegionKey.FromGrid(i % 10, 0)))
            .ToArray();
        var watch = Stopwatch.StartNew();

        foreach (var entity in entities)
        {
            var region = RegionKey.FromGrid(entity.EntityKey.Value % 13, entity.EntityKey.Value % 7);
            Assert.True(world.MoveToRegion(entity.EntityKey, region));
        }

        watch.Stop();
        foreach (var entity in entities)
        {
            var expected = RegionKey.FromGrid(entity.EntityKey.Value % 13, entity.EntityKey.Value % 7);
            Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
            Assert.Equal(expected, world.GetRegion(entity.EntityKey));
        }
        Debug.WriteLine($"WORLD-A-R2 1000实体分区迁移Ticks={watch.ElapsedTicks}");
    }
}

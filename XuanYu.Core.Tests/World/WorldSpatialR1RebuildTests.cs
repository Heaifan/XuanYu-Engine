using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.Core.World;

namespace XuanYu.Core.Tests.World;

public sealed class WorldSpatialR1RebuildTests
{
    [Fact]
    public void Rebuild_from_world_keeps_radius_and_bounds_results()
    {
        var world = CreateWorld(1000);
        var center = new Vector3d(120, 80, 0);
        var radius = 55;
        var bounds = new SpatialAabb(new Vector3d(40, 30, -1), new Vector3d(180, 160, 1));
        var beforeRadius = world.QueryRadius(center, radius).OrderBy(id => id.Value).ToArray();
        var beforeBounds = world.QueryBounds(bounds).OrderBy(id => id.Value).ToArray();

        world.RebuildSpatialIndexFromWorld();

        WorldSpatialR1Oracle.AssertSame(beforeRadius, world.QueryRadius(center, radius));
        WorldSpatialR1Oracle.AssertSame(beforeBounds, world.QueryBounds(bounds));
        Assert.Equal(world.EntityCount, world.SpatialEntityCount);
    }

    [Fact]
    public void Deterministic_random_queries_match_oracle_after_random_moves()
    {
        var world = CreateWorld(1000);
        var random = new Random(20260723);
        for (var i = 0; i < 200; i++)
        {
            var entity = world.Entities[random.Next(world.EntityCount)];
            var next = new Vector3d(random.Next(500), random.Next(500), 0);
            Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(next)));
            var center = new Vector3d(random.Next(500), random.Next(500), 0);
            var radius = random.Next(5, 80);
            var bounds = WorldSpatialR1Oracle.BoundsAround(center, radius);
            WorldSpatialR1Oracle.AssertSame(WorldSpatialR1Oracle.Radius(world, center, radius), world.QueryRadius(center, radius));
            WorldSpatialR1Oracle.AssertSame(WorldSpatialR1Oracle.Bounds(world, bounds), world.QueryBounds(bounds));
        }
    }

    static GlobalWorld CreateWorld(int count)
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 25));
        for (var i = 0; i < count; i++)
        {
            var p = new Vector3d((i % 100) * 5.0, (i / 100) * 5.0, 0);
            world.Create($"Entity {i}", transform: new CommittedTransform(p));
        }

        return world;
    }
}

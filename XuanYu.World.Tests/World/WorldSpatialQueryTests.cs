using System.Diagnostics;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.World;

public sealed class WorldSpatialQueryTests
{
    readonly ITestOutputHelper _output;

    public WorldSpatialQueryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Query_radius_and_bounds_match_brute_force_oracle()
    {
        var watch = Stopwatch.StartNew();
        var world = CreateWorld(1000);
        watch.Stop();
        var center = new Vector3d(125, 80, 0);
        var radius = 45;
        var bounds = new SpatialAabb(new Vector3d(40, 40, -1), new Vector3d(170, 140, 1));
        var queryWatch = Stopwatch.StartNew();

        AssertSame(BruteRadius(world, center, radius), world.QueryRadius(center, radius));
        AssertSame(BruteBounds(world, bounds), world.QueryBounds(bounds));
        queryWatch.Stop();
        _output.WriteLine($"WORLD-A-R3 1000实体查询：CreateTicks={watch.ElapsedTicks}; QueryTicks={queryWatch.ElapsedTicks}; Visited={world.LastSpatialQueryStats.VisitedNodeCount}; Candidates={world.LastSpatialQueryStats.CandidateCount}");
    }

    [Fact]
    public void Query_updates_after_move_cross_region_and_destroy()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("Mover", transform: new CommittedTransform(new Vector3d(1, 1, 0)));
        Assert.Contains(entity.EntityKey, world.QueryRadius(new Vector3d(1, 1, 0), 1));

        Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(new Vector3d(25, 1, 0))));
        Assert.DoesNotContain(entity.EntityKey, world.QueryRadius(new Vector3d(1, 1, 0), 1));
        Assert.Contains(entity.EntityKey, world.QueryRadius(new Vector3d(25, 1, 0), 1));
        Assert.Equal(RegionKey.FromGrid(2, 0), world.Get(entity.EntityKey).RegionKey);

        Assert.True(world.Destroy(entity.EntityKey));
        Assert.DoesNotContain(entity.EntityKey, world.QueryRadius(new Vector3d(25, 1, 0), 1));
    }

    [Fact]
    public void Ten_thousand_entity_queries_match_oracle_and_record_stats()
    {
        var watch = Stopwatch.StartNew();
        var world = CreateWorld(10000);
        watch.Stop();
        var center = new Vector3d(280, 320, 0);
        var radius = 60;

        var queryWatch = Stopwatch.StartNew();
        var actual = world.QueryRadius(center, radius);
        queryWatch.Stop();

        AssertSame(BruteRadius(world, center, radius), actual);
        Assert.True(world.LastSpatialQueryStats.VisitedNodeCount < world.EntityCount * 2);
        _output.WriteLine($"WORLD-A-R3 10000实体查询：CreateTicks={watch.ElapsedTicks}; QueryTicks={queryWatch.ElapsedTicks}; Visited={world.LastSpatialQueryStats.VisitedNodeCount}; Candidates={world.LastSpatialQueryStats.CandidateCount}");
    }

    static GlobalWorld CreateWorld(int count)
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 50));
        for (var i = 0; i < count; i++)
        {
            var x = (i % 100) * 7.0;
            var y = (i / 100) * 7.0;
            world.Create($"Entity {i}", transform: new CommittedTransform(new Vector3d(x, y, 0)));
        }

        return world;
    }

    static IReadOnlyList<EntityId> BruteBounds(GlobalWorld world, SpatialAabb bounds) =>
        world.Entities.Where(e => bounds.Intersects(EntityBox(e.GlobalPosition))).Select(e => e.EntityKey).ToArray();

    static IReadOnlyList<EntityId> BruteRadius(GlobalWorld world, Vector3d center, double radius) =>
        world.Entities.Where(e => DistanceSquared(e.GlobalPosition, center) <= radius * radius).Select(e => e.EntityKey).ToArray();

    static double DistanceSquared(Vector3d a, Vector3d b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }

    // Entity half-extent matches WorldQuery.PointBounds (R2 single-authority box).
    static SpatialAabb EntityBox(Vector3d p) =>
        new(new Vector3d(p.X - 0.5, p.Y - 0.5, p.Z - 0.5), new Vector3d(p.X + 0.5, p.Y + 0.5, p.Z + 0.5));

    static void AssertSame(IReadOnlyList<EntityId> expected, IReadOnlyList<EntityId> actual) =>
        Assert.Equal(expected.OrderBy(id => id.Value), actual.OrderBy(id => id.Value));
}

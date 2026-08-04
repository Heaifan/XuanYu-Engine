using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

public sealed class SpatialIndexEditLifecycleTests
{
    [Fact]
    public void Create_move_cross_region_and_destroy_keep_index_current()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("Tracked", transform: At(1));
        AssertIndexedAt(world, entity.EntityKey, new Vector3d(1, 0, 0));

        Assert.True(world.UpdateTransform(entity.EntityKey, At(25)));
        AssertNotIndexedAt(world, entity.EntityKey, new Vector3d(1, 0, 0));
        AssertIndexedAt(world, entity.EntityKey, new Vector3d(25, 0, 0));
        Assert.Equal(RegionKey.FromGrid(2, 0), world.Get(entity.EntityKey).RegionKey);

        Assert.True(world.Destroy(entity.EntityKey));
        Assert.False(world.Exists(entity.EntityKey));
        AssertNotIndexedAt(world, entity.EntityKey, new Vector3d(25, 0, 0));
        Assert.Empty(world.QueryBounds(SpatialQueryOracle.BoundsAround(new Vector3d(25, 0, 0), 1)));
    }

    [Fact]
    public void Preview_cancel_does_not_pollute_formal_world_index()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("Preview", transform: At(1));
        var previewOnly = new Vector3d(25, 0, 0);

        AssertIndexedAt(world, entity.EntityKey, new Vector3d(1, 0, 0));
        AssertNotIndexedAt(world, entity.EntityKey, previewOnly);
        Assert.Equal(new Vector3d(1, 0, 0), world.Get(entity.EntityKey).GlobalPosition);
    }

    [Fact]
    public void Undo_and_redo_restore_index_to_committed_position()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("History", transform: At(1));
        var before = entity.Transform;
        var after = At(25);
        Assert.True(world.UpdateTransform(entity.EntityKey, after));

        Assert.True(world.UpdateTransform(entity.EntityKey, before));
        AssertIndexedAt(world, entity.EntityKey, before.Position);
        AssertNotIndexedAt(world, entity.EntityKey, after.Position);

        Assert.True(world.UpdateTransform(entity.EntityKey, after));
        AssertNotIndexedAt(world, entity.EntityKey, before.Position);
        AssertIndexedAt(world, entity.EntityKey, after.Position);
    }

    static CommittedTransform At(double x) => new(new Vector3d(x, 0, 0));

    static void AssertIndexedAt(GlobalWorld world, XuanYu.Core.Identity.EntityId id, Vector3d p) =>
        Assert.Contains(id, world.QueryRadius(p, 0.1));

    static void AssertNotIndexedAt(GlobalWorld world, XuanYu.Core.Identity.EntityId id, Vector3d p) =>
        Assert.DoesNotContain(id, world.QueryRadius(p, 0.1));
}

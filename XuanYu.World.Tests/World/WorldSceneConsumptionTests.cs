using XuanYu.Core.Identity;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

using XuanYu.World.Scene;
namespace XuanYu.World.Tests.World;

public sealed class WorldSceneConsumptionTests
{
    [Fact]
    public void Scene_owner_projects_default_entity_from_global_world()
    {
        var scene = new SceneStateOwner();

        var entity = scene.RenderSnapshot.Entity;

        Assert.True(scene.TryGetEntity(entity.EntityKey, out var worldEntity));
        Assert.Equal(worldEntity.EntityKey, entity.EntityKey);
        Assert.Equal(worldEntity.Name, entity.Name);
        Assert.Equal(worldEntity.Transform, entity.Transform);
    }

    [Fact]
    public void Move_commit_updates_same_world_entity_identity()
    {
        var scene = new SceneStateOwner();
        var id = scene.RenderSnapshot.Entity.EntityKey;

        var commit = scene.CommitPositionWithResult(new Vector3d(4, 0, 0));

        Assert.True(commit.Changed);
        Assert.Equal(id, commit.EntityKey);
        Assert.True(scene.TryGetEntity(id, out var worldEntity));
        Assert.Equal(new Vector3d(4, 0, 0), worldEntity.Transform.Position);
        Assert.Equal(id, scene.RenderSnapshot.Entity.EntityKey);
    }

    [Fact]
    public void Undo_redo_restore_same_world_entity()
    {
        var scene = new SceneStateOwner();
        var commit = scene.CommitPositionWithResult(new Vector3d(2, 0, 0));
        var entry = new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After);

        Assert.True(scene.RestoreTransform(entry.EntityKey, entry.Before));
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.True(scene.RestoreTransform(entry.EntityKey, entry.After));
        Assert.Equal(new Vector3d(2, 0, 0), scene.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(entry.EntityKey, scene.RenderSnapshot.Entity.EntityKey);
    }

    [Fact]
    public void Destroy_active_entity_clears_render_projection()
    {
        var scene = new SceneStateOwner();
        var id = scene.RenderSnapshot.Entity.EntityKey;

        Assert.True(scene.DestroyEntity(id));

        Assert.False(scene.TryGetEntity(id, out _));
        Assert.False(scene.RenderSnapshot.HasEntity);
        Assert.False(scene.RaycastSpatial(SpatialRay(), SpatialQueryCategory.All).HasHit);
    }

    static SpatialRayQuery SpatialRay() =>
        new(new WorldRay(new Vector3d(-5, 0, 0), Vector3d.UnitX), 20);
}

using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.Transform;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Transform;

public sealed partial class TransformSessionTests
{
    [Fact]
    public void Commit_rejects_recreated_entity_with_same_key_as_stale_session()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        var original = scene.RenderSnapshot.Entity;
        Assert.True(scene.TryGetEntity(original.EntityKey, out var worldOriginal));
        scene.ReplaceEntities([new WorldEntitySnapshot(original.EntityKey,
            "替换实体", original.Type, original.Transform, worldOriginal.GlobalPosition,
            worldOriginal.RegionKey, worldOriginal.Activity, worldOriginal.Extent)]);
        Assert.True(session.TryPreview(17, Vector3d.UnitX));
        Assert.False(session.TryCommit(17, scene, out var commit));
        Assert.False(commit.Changed);
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.False(session.IsActive);
    }

    [Fact]
    public void Commit_allows_unrelated_entity_mutation()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        var unrelated = scene.CreateEntity("实体B", WorldEntityTypes.Cube);
        var spatialRevision = scene.SpatialRevision;
        Assert.True(scene.CommitPositionWithResult(unrelated.EntityKey, Vector3d.UnitY).Changed);
        Assert.True(scene.SpatialRevision > spatialRevision);
        Assert.True(session.TryPreview(17, Vector3d.UnitX));
        Assert.True(session.TryCommit(17, scene));
    }

    [Fact]
    public void Commit_allows_unrelated_world_entity_rename()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        var unrelated = scene.CreateEntity("实体B", WorldEntityTypes.Cube);
        Assert.True(scene.RenameEntity(unrelated.EntityKey, "实体B-改名", out _));
        Assert.True(session.TryPreview(17, Vector3d.UnitX));
        Assert.True(session.TryCommit(17, scene));
    }

    [Fact]
    public void Commit_rejects_target_entity_conflicting_mutation()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        Assert.True(scene.CommitPositionWithResult(
            scene.RenderSnapshot.Entity.EntityKey, Vector3d.UnitY).Changed);
        Assert.True(session.TryPreview(17, Vector3d.UnitX));
        Assert.False(session.TryCommit(17, scene, out var commit));
        Assert.False(commit.Changed);
    }

    [Fact]
    public void Commit_rejects_remove_and_recreate_with_same_key()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        var original = scene.RenderSnapshot.Entity;
        Assert.True(scene.TryGetEntity(original.EntityKey, out var snapshot));
        Assert.True(scene.DestroyEntity(original.EntityKey));
        scene.ReplaceEntities([snapshot]);
        Assert.True(session.TryPreview(17, Vector3d.UnitX));
        Assert.False(session.TryCommit(17, scene, out var commit));
        Assert.False(commit.Changed);
    }

    static TransformSession Begin(SceneStateOwner scene, MoveGizmoAxis axis)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, axis,
            ((IWorldTransformMutationContract)scene).WorldRevision));
        return session;
    }
}

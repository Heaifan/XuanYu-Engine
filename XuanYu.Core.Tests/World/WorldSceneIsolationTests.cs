using XuanYu.Core.Identity;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.World;

public sealed class WorldSceneIsolationTests
{
    [Fact]
    public void Moving_b_keeps_a_and_c_unchanged()
    {
        var scene = new SceneStateOwner();
        var a = scene.RenderSnapshot.Entity.EntityKey;
        var b = scene.CreateEntity("B", "MinimalSceneEntity").EntityKey;
        var c = scene.CreateEntity("C", "MinimalSceneEntity").EntityKey;

        var commit = scene.CommitPositionWithResult(b, new Vector3d(9, 0, 0));

        Assert.True(commit.Changed);
        Assert.Equal(Vector3d.Zero, Get(scene, a).Transform.Position);
        Assert.Equal(new Vector3d(9, 0, 0), Get(scene, b).Transform.Position);
        Assert.Equal(Vector3d.Zero, Get(scene, c).Transform.Position);
    }

    [Fact]
    public void Undo_for_b_restores_only_b()
    {
        var scene = new SceneStateOwner();
        var a = scene.RenderSnapshot.Entity.EntityKey;
        var b = scene.CreateEntity("B", "MinimalSceneEntity").EntityKey;
        var c = scene.CreateEntity("C", "MinimalSceneEntity").EntityKey;
        var commit = scene.CommitPositionWithResult(b, new Vector3d(9, 0, 0));
        var entry = new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After);

        Assert.True(scene.RestoreTransform(entry.EntityKey, entry.Before));

        Assert.Equal(Vector3d.Zero, Get(scene, a).Transform.Position);
        Assert.Equal(Vector3d.Zero, Get(scene, b).Transform.Position);
        Assert.Equal(Vector3d.Zero, Get(scene, c).Transform.Position);
    }

    [Fact]
    public void Destroy_selected_entity_falls_back_without_reusing_identity()
    {
        var scene = new SceneStateOwner();
        var a = scene.RenderSnapshot.Entity.EntityKey;
        var b = scene.CreateEntity("B", "MinimalSceneEntity").EntityKey;
        scene.SetActiveEntity(b);

        Assert.True(scene.DestroyEntity(b));

        Assert.False(scene.TryGetEntity(b, out _));
        Assert.True(scene.RenderSnapshot.HasEntity);
        Assert.Equal(a, scene.RenderSnapshot.Entity.EntityKey);
    }

    static XuanYu.Core.World.WorldEntitySnapshot Get(SceneStateOwner scene, EntityId id)
    {
        Assert.True(scene.TryGetEntity(id, out var entity));
        return entity;
    }
}

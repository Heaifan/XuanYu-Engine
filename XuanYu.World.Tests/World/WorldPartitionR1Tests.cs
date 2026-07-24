using System.Diagnostics;
using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.World;

using XuanYu.World.Scene;
using XuanYu.World.Transform;
namespace XuanYu.World.Tests.World;

public sealed class WorldPartitionR1Tests
{
    [Fact]
    public void Preview_into_next_region_then_cancel_keeps_membership()
    {
        var scene = new SceneStateOwner();
        var id = scene.RenderSnapshot.Entity.EntityKey;
        var session = Begin(scene);

        Assert.True(session.TryPreview(17, new Vector3d(1001, 0, 0)));
        Assert.Equal(RegionKey.Origin, scene.GetRegion(id));

        Assert.True(session.TryCancel(17));
        Assert.Equal(RegionKey.Origin, scene.GetRegion(id));
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
    }

    [Fact]
    public void Preview_into_next_region_then_commit_migrates_once()
    {
        var scene = new SceneStateOwner();
        var id = scene.RenderSnapshot.Entity.EntityKey;
        var session = Begin(scene);
        session.TryPreview(17, new Vector3d(1001, 0, 0));

        Assert.True(session.TryCommit(17, scene, out var commit));

        Assert.True(commit.Changed);
        Assert.Equal(id, scene.RenderSnapshot.Entity.EntityKey);
        Assert.Equal(RegionKey.FromGrid(1, 0), scene.GetRegion(id));
        Assert.Equal(new Vector3d(1001, 0, 0), scene.RenderSnapshot.Entity.Transform.Position);
    }

    [Fact]
    public void Undo_and_redo_recompute_region_from_position()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();
        var id = scene.RenderSnapshot.Entity.EntityKey;
        Commit(scene, history, new Vector3d(1001, 0, 0));

        Assert.True(history.TryUndo(out var undo));
        Assert.True(scene.RestoreTransform(undo.EntityKey, undo.Before));
        Assert.Equal(RegionKey.Origin, scene.GetRegion(id));

        Assert.True(history.TryRedo(out var redo));
        Assert.True(scene.RestoreTransform(redo.EntityKey, redo.After));
        Assert.Equal(RegionKey.FromGrid(1, 0), scene.GetRegion(id));
    }

    [Fact]
    public void Migrating_one_entity_does_not_move_neighbors()
    {
        var scene = new SceneStateOwner();
        scene.EnsureEntityCount(3);
        var first = scene.Entities[0];
        var second = scene.Entities[1];
        var third = scene.Entities[2];

        scene.CommitPositionWithResult(first.EntityKey, new Vector3d(1001, 0, 0));

        Assert.Equal(RegionKey.FromGrid(1, 0), scene.GetRegion(first.EntityKey));
        Assert.Equal(RegionKey.Origin, scene.GetRegion(second.EntityKey));
        Assert.Equal(RegionKey.Origin, scene.GetRegion(third.EntityKey));
    }

    [Fact]
    public void Active_dormant_active_keeps_identity_region_and_position()
    {
        var scene = new SceneStateOwner();
        var before = scene.RenderSnapshot.Entity;

        Assert.True(scene.SetEntityActivity(before.EntityKey, WorldEntityActivity.Dormant));
        Assert.True(scene.SetEntityActivity(before.EntityKey, WorldEntityActivity.Active));

        var after = scene.RenderSnapshot.Entity;
        Assert.Equal(before.EntityKey, after.EntityKey);
        Assert.Equal(before.Transform.Position, after.Transform.Position);
        Assert.Equal(RegionKey.Origin, scene.GetRegion(after.EntityKey));
    }

    static TransformSession Begin(SceneStateOwner scene)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        return session;
    }

    static void Commit(SceneStateOwner scene, EditorHistoryOwner history, Vector3d position)
    {
        var session = Begin(scene);
        session.TryPreview(17, position);
        Assert.True(session.TryCommit(17, scene, out var commit));
        if (commit.Changed) history.Push(new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After));
    }
}

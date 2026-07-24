using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Core.Transform;

using XuanYu.World.Scene;
using XuanYu.World.Transform;
namespace XuanYu.World.Tests.World;

public sealed class WorldR1FinalSceneTests
{
    [Fact]
    public void Move_undo_redo_changes_only_selected_entity_five()
    {
        var scene = new SceneStateOwner();
        scene.EnsureEntityCount(10);
        var ids = scene.Entities.Select(item => item.EntityKey).ToArray();
        var target = ids[4];
        scene.SetActiveEntity(target);
        var before = scene.RenderSnapshot.Entities.ToDictionary(
            item => item.EntityKey, item => item.Transform.Position);

        var commit = CommitMove(scene, new Vector3d(8, 0, 0));
        var history = new EditorHistoryOwner();
        history.Push(new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After));

        AssertOnlyTargetMoved(scene, before, target, new Vector3d(8, 0, 0));
        Assert.True(history.TryUndo(out var undo));
        Assert.True(scene.RestoreTransform(undo.EntityKey, undo.Before));
        AssertOnlyTargetMoved(scene, before, target, before[target]);
        Assert.True(history.TryRedo(out var redo));
        Assert.True(scene.RestoreTransform(redo.EntityKey, redo.After));
        AssertOnlyTargetMoved(scene, before, target, new Vector3d(8, 0, 0));
    }

    [Fact]
    public void Destroy_removes_entity_from_world_snapshot_spatial_and_render()
    {
        var scene = new SceneStateOwner();
        scene.EnsureEntityCount(10);
        var doomed = scene.CreateEntity(
            "WORLD-A-R1 FINAL Doomed",
            "MinimalSceneEntity",
            new CommittedTransform(new Vector3d(100, 100, 0)));
        scene.SetActiveEntity(doomed.EntityKey);

        Assert.True(scene.DestroyEntity(doomed.EntityKey));

        Assert.False(scene.TryGetEntity(doomed.EntityKey, out _));
        Assert.DoesNotContain(scene.RenderSnapshot.Entities, item => item.EntityKey == doomed.EntityKey);
        Assert.DoesNotContain(scene.QuerySpatial(Area(100, 100), SpatialQueryCategory.All).Candidates,
            item => item.EntityKey == doomed.EntityKey);
        Assert.False(scene.RaycastSpatial(Ray(95, 100), SpatialQueryCategory.All).HasHit);
        Assert.NotEqual(doomed.EntityKey, scene.RenderSnapshot.Entity.EntityKey);
    }

    static SceneTransformCommitResult CommitMove(SceneStateOwner scene, Vector3d position)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        session.TryPreview(17, position);
        Assert.True(session.TryCommit(17, scene, out var commit));
        return commit;
    }

    static void AssertOnlyTargetMoved(
        SceneStateOwner scene,
        Dictionary<XuanYu.Core.Identity.EntityId, Vector3d> before,
        XuanYu.Core.Identity.EntityId target,
        Vector3d expected)
    {
        foreach (var entity in scene.RenderSnapshot.Entities)
        {
            var expectedPosition = entity.EntityKey == target ? expected : before[entity.EntityKey];
            Assert.Equal(expectedPosition, entity.Transform.Position);
        }
    }

    static SpatialAabb Area(double x, double y) =>
        new(new Vector3d(x - 0.25, y - 0.25, -0.25), new Vector3d(x + 0.25, y + 0.25, 0.25));

    static SpatialRayQuery Ray(double x, double y) =>
        new(new WorldRay(new Vector3d(x, y, 0), Vector3d.UnitX), 10);
}

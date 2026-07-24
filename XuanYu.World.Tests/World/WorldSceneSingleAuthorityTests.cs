using System.Linq;
using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Core.Transform;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class WorldSceneSingleAuthorityTests
{
    [Fact]
    public void Case1_create_is_queryable_through_unique_authority()
    {
        var scene = new SceneStateOwner();
        var e = scene.CreateEntity("E1", "T1", new CommittedTransform(new Vector3d(5, 0, 0)));

        Assert.Contains(e.EntityKey, scene.QuerySpatial(Area(5, 0), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey));
        Assert.True(scene.RaycastSpatial(Ray(5, 0), SpatialQueryCategory.All).HasHit);
    }

    [Fact]
    public void Case2_move_must_not_leave_ghost_at_old_position()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;

        Assert.True(scene.CommitPosition(new Vector3d(10, 0, 0)));

        Assert.Empty(scene.QuerySpatial(Area(0, 0), SpatialQueryCategory.All).Candidates);
        Assert.Contains(key, scene.QuerySpatial(Area(10, 0), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey));
        Assert.True(scene.RaycastSpatial(Ray(10, 0), SpatialQueryCategory.All).HasHit);
        Assert.False(scene.RaycastSpatial(Ray(0, 5), SpatialQueryCategory.All).HasHit);
    }

    [Fact]
    public void Case3_undo_redo_keeps_spatial_answer_consistent()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;

        scene.CommitPosition(new Vector3d(10, 0, 0));
        Assert.True(scene.RaycastSpatial(Ray(10, 0), SpatialQueryCategory.All).HasHit);
        Assert.Empty(scene.QuerySpatial(Area(0, 0), SpatialQueryCategory.All).Candidates);

        scene.RestoreTransform(key, new CommittedTransform(new Vector3d(0, 0, 0)));
        Assert.True(scene.RaycastSpatial(Ray(0, 0), SpatialQueryCategory.All).HasHit);
        Assert.Empty(scene.QuerySpatial(Area(10, 0), SpatialQueryCategory.All).Candidates);

        scene.RestoreTransform(key, new CommittedTransform(new Vector3d(10, 0, 0)));
        Assert.True(scene.RaycastSpatial(Ray(10, 0), SpatialQueryCategory.All).HasHit);
        Assert.Empty(scene.QuerySpatial(Area(0, 0), SpatialQueryCategory.All).Candidates);
    }

    [Fact]
    public void Case4_destroy_removes_entity_from_spatial_query()
    {
        var scene = new SceneStateOwner();
        var doomed = scene.CreateEntity("Doomed", "T1", new CommittedTransform(new Vector3d(4, 0, 0)));

        Assert.True(scene.DestroyEntity(doomed.EntityKey));

        Assert.False(scene.RaycastSpatial(Ray(4, -5, Vector3d.UnitY), SpatialQueryCategory.All).HasHit);
        Assert.Empty(scene.QuerySpatial(Area(4, 0), SpatialQueryCategory.All).Candidates);
    }

    [Fact]
    public void Case5_cross_region_move_keeps_unique_query_correct()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;
        var r0 = scene.GetRegion(key);

        scene.CommitPosition(new Vector3d(1_000_000, 0, 0));
        var r1 = scene.GetRegion(key);

        Assert.NotEqual(r0, r1);
        Assert.Contains(key, scene.QuerySpatial(Area(1_000_000, 0), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey));
        Assert.Empty(scene.QuerySpatial(Area(0, 0), SpatialQueryCategory.All).Candidates);
        Assert.True(scene.RaycastSpatial(Ray(1_000_000, 0), SpatialQueryCategory.All).HasHit);
    }

    [Fact]
    public void Case6_scene_has_no_second_spatial_index()
    {
        var field = typeof(SceneStateOwner).GetField("_spatialIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Null(field);
    }

    static SpatialAabb Area(double x, double y) =>
        new(new Vector3d(x - 0.5, y - 0.5, -0.5), new Vector3d(x + 0.5, y + 0.5, 0.5));

    static SpatialRayQuery Ray(double x, double y, Vector3d? direction = null) =>
        new(new WorldRay(new Vector3d(x, y, 0), direction ?? Vector3d.UnitX), 20);
}

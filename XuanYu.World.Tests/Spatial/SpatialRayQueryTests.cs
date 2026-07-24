using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

using XuanYu.World.Spatial;
namespace XuanYu.World.Tests.Spatial;

public sealed class SpatialRayQueryTests
{
    [Fact]
    public void Ray_query_returns_candidates_and_honors_mask()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 4));
        owner.Insert(SpatialTestData.Bounds(2, 6, SpatialQueryCategory.Gizmo));

        var scene = owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 10), SpatialQueryCategory.SceneEntity);
        var gizmo = owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 10), SpatialQueryCategory.Gizmo);

        Assert.Equal(EntityId.FromInt(1), scene.Candidates.Single().EntityKey);
        Assert.Equal(EntityId.FromInt(2), gizmo.Candidates.Single().EntityKey);
    }

    [Fact]
    public void Ray_query_rejects_miss_back_facing_and_max_distance()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 4));

        Assert.Empty(owner.Query(Ray(new Vector3d(0, 3, 0), Vector3d.UnitX, 10), SpatialQueryCategory.All).Candidates);
        Assert.Empty(owner.Query(Ray(Vector3d.Zero, -Vector3d.UnitX, 10), SpatialQueryCategory.All).Candidates);
        Assert.Empty(owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 3), SpatialQueryCategory.All).Candidates);
    }

    [Fact]
    public void Ray_query_handles_inside_origin_and_parallel_axis()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 0));

        var inside = owner.Query(Ray(Vector3d.Zero, Vector3d.UnitY, 10), SpatialQueryCategory.All);
        var parallel = owner.Query(Ray(new Vector3d(0, -4, 0), Vector3d.UnitY, 10), SpatialQueryCategory.All);

        Assert.Single(inside.Candidates);
        Assert.Single(parallel.Candidates);
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }
}

using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialRaycastNearestTests
{
    [Fact]
    public void Picks_nearest_hit_from_broad_phase_candidates()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 10));
        owner.Insert(SpatialTestData.Bounds(2, 3));
        owner.Insert(SpatialTestData.Bounds(3, 7));

        var result = owner.Raycast(Ray(Vector3d.Zero, Vector3d.UnitX, 20), SpatialQueryCategory.All);

        Assert.Equal(EntityId.FromInt(2), result.Hit!.Value.EntityKey);
        Assert.Equal(result.Stats.CandidateCount, result.Stats.NarrowPhaseTestCount);
        Assert.Equal(3, result.Stats.HitCount);
    }

    [Fact]
    public void Equal_distance_uses_stable_entity_key()
    {
        var first = Owner(5, 8);
        var second = Owner(8, 5);

        Assert.Equal(EntityId.FromInt(5), first.Raycast(Ray(Vector3d.Zero, Vector3d.UnitY, 5), SpatialQueryCategory.All).Hit!.Value.EntityKey);
        Assert.Equal(EntityId.FromInt(5), second.Raycast(Ray(Vector3d.Zero, Vector3d.UnitY, 5), SpatialQueryCategory.All).Hit!.Value.EntityKey);
    }

    static SpatialIndexOwner Owner(int a, int b)
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(new SpatialBounds(EntityId.FromInt(a), SpatialTestData.Box(0, 4), SpatialQueryCategory.SceneEntity));
        owner.Insert(new SpatialBounds(EntityId.FromInt(b), SpatialTestData.Box(0, 4), SpatialQueryCategory.SceneEntity));
        return owner;
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }
}

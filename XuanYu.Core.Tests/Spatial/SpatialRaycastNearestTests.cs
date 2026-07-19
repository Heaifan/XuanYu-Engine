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

    [Fact]
    public void Resolver_does_not_publish_broad_candidate_without_narrow_hit()
    {
        var index = new OneCandidateIndex(SpatialTestData.Bounds(7, 10));
        var owner = new SpatialIndexOwner(index);

        var result = owner.Raycast(Ray(Vector3d.Zero, Vector3d.UnitY, 5), SpatialQueryCategory.All);

        Assert.False(result.HasHit);
        Assert.Equal(1, result.Stats.CandidateCount);
        Assert.Equal(1, result.Stats.NarrowPhaseTestCount);
        Assert.Equal(0, result.Stats.HitCount);
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

    sealed class OneCandidateIndex(SpatialBounds candidate) : ISpatialIndex
    {
        public int Count => 1;

        public void Insert(SpatialBounds bounds)
        {
        }

        public bool Remove(EntityId entityKey)
        {
            return false;
        }

        public bool Update(SpatialBounds bounds)
        {
            return false;
        }

        public SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask)
        {
            return Query(mask);
        }

        public SpatialQueryResult Query(SpatialRayQuery ray, SpatialQueryCategory mask)
        {
            return Query(mask);
        }

        SpatialQueryResult Query(SpatialQueryCategory mask)
        {
            IReadOnlyList<SpatialBounds> candidates = (candidate.Category & mask) == 0 ? [] : [candidate];
            var stats = new SpatialQueryStats(0, Count, 1, candidates.Count);
            return new SpatialQueryResult(candidates, stats);
        }
    }
}

using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialRayQueryLifecycleTests
{
    [Fact]
    public void Update_changes_ray_candidates_and_remove_clears_ghosts()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 4));

        Assert.True(owner.Update(SpatialTestData.Bounds(1, 8)));

        Assert.Empty(owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 5), SpatialQueryCategory.All).Candidates);
        Assert.Equal(EntityId.FromInt(1), owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 10), SpatialQueryCategory.All).Candidates.Single().EntityKey);
        Assert.True(owner.Remove(EntityId.FromInt(1)));
        Assert.Empty(owner.Query(Ray(Vector3d.Zero, Vector3d.UnitX, 10), SpatialQueryCategory.All).Candidates);
    }

    [Fact]
    public void Ray_query_scale_stats_do_not_degrade_to_full_scan()
    {
        var thousand = SpatialTestData.Grid(1_000).Query(Ray(new Vector3d(20, -4, 0), Vector3d.UnitY, 20), SpatialQueryCategory.SceneEntity);
        var tenThousand = SpatialTestData.Grid(10_000).Query(Ray(new Vector3d(120, 156, 0), Vector3d.UnitY, 20), SpatialQueryCategory.SceneEntity);

        Assert.Equal(1_000, thousand.Stats.TotalEntityCount);
        Assert.Equal(10_000, tenThousand.Stats.TotalEntityCount);
        Assert.True(thousand.Stats.VisitedNodeCount < 1_000);
        Assert.True(tenThousand.Stats.VisitedNodeCount < 2_000);
        Assert.True(thousand.Stats.CandidateCount <= 6);
        Assert.True(tenThousand.Stats.CandidateCount <= 6);
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }
}

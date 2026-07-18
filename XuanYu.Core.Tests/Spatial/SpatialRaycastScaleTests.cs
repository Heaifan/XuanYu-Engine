using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialRaycastScaleTests
{
    [Fact]
    public void One_thousand_entities_raycast_checks_only_candidates()
    {
        var owner = SpatialTestData.Grid(1_000);

        var result = owner.Raycast(Ray(new Vector3d(20, -4, 0), Vector3d.UnitY, 20), SpatialQueryCategory.SceneEntity);

        Assert.True(result.HasHit);
        Assert.Equal(1_000, result.Stats.TotalEntityCount);
        Assert.True(result.Stats.VisitedNodeCount < 1_000);
        Assert.Equal(result.Stats.CandidateCount, result.Stats.NarrowPhaseTestCount);
        Assert.True(result.Stats.NarrowPhaseTestCount <= 6);
    }

    [Fact]
    public void Ten_thousand_entities_raycast_checks_only_candidates()
    {
        var owner = SpatialTestData.Grid(10_000);

        var result = owner.Raycast(Ray(new Vector3d(120, 156, 0), Vector3d.UnitY, 20), SpatialQueryCategory.SceneEntity);

        Assert.True(result.HasHit);
        Assert.Equal(10_000, result.Stats.TotalEntityCount);
        Assert.True(result.Stats.VisitedNodeCount < 2_000);
        Assert.Equal(result.Stats.CandidateCount, result.Stats.NarrowPhaseTestCount);
        Assert.True(result.Stats.NarrowPhaseTestCount <= 6);
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }
}

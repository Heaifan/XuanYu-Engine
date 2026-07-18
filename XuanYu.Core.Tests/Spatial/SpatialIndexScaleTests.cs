using XuanYu.Core.Identity;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialIndexScaleTests
{
    [Fact]
    public void One_thousand_entities_query_uses_broad_phase_stats()
    {
        var owner = SpatialTestData.Grid(1_000);

        var result = owner.Query(SpatialTestData.PointQuery(20, 20), SpatialQueryCategory.SceneEntity);

        Assert.Equal(1_000, result.Stats.TotalEntityCount);
        Assert.Single(result.Candidates);
        Assert.True(result.Stats.VisitedNodeCount < 1_000);
    }

    [Fact]
    public void Ten_thousand_entities_query_does_not_visit_every_leaf()
    {
        var owner = SpatialTestData.Grid(10_000);

        var result = owner.Query(SpatialTestData.PointQuery(120, 160), SpatialQueryCategory.SceneEntity);

        Assert.Equal(10_000, result.Stats.TotalEntityCount);
        Assert.Single(result.Candidates);
        Assert.True(result.Stats.VisitedNodeCount < 2_000);
    }

    [Fact]
    public void Many_moves_and_removes_keep_index_consistent()
    {
        var owner = SpatialTestData.Grid(1_000);
        for (var i = 1; i <= 100; i++) owner.Update(SpatialTestData.Bounds(i, 800 + i));
        for (var i = 101; i <= 200; i++) owner.Remove(EntityId.FromInt(i));

        Assert.Equal(900, owner.EntityCount);
        Assert.Empty(owner.Query(SpatialTestData.PointQuery(0, 4), SpatialQueryCategory.All).Candidates);
        Assert.Single(owner.Query(SpatialTestData.PointQuery(801, 0), SpatialQueryCategory.All).Candidates);
    }
}

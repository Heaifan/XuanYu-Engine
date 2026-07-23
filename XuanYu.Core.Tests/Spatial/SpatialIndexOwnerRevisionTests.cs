using XuanYu.Core.Identity;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialIndexOwnerRevisionTests
{
    [Fact]
    public void Revision_changes_only_when_index_fact_changes()
    {
        var owner = new SpatialIndexOwner();
        var first = SpatialTestData.Bounds(1, 0);

        owner.Insert(first);
        Assert.Equal(1, owner.SpatialRevision);
        Assert.False(owner.Update(first));
        Assert.Equal(1, owner.SpatialRevision);
        Assert.True(owner.Update(SpatialTestData.Bounds(1, 10)));
        Assert.Equal(2, owner.SpatialRevision);
        Assert.False(owner.Remove(EntityId.FromInt(2)));
        Assert.Equal(2, owner.SpatialRevision);
        Assert.True(owner.Remove(EntityId.FromInt(1)));
        Assert.Equal(3, owner.SpatialRevision);
    }

    [Fact]
    public void Query_stats_include_revision_count_and_probe_text()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 0));

        var result = owner.Query(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.All);

        Assert.Equal(1, result.Stats.SpatialRevision);
        Assert.Equal(1, result.Stats.TotalEntityCount);
        Assert.Contains("空间查询完成", result.Stats.ToChineseProbe());
    }
}

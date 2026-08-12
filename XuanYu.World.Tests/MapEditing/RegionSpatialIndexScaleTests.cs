using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSpatialIndexScaleTests
{
    [Fact]
    public void Ten_thousand_axis_crossing_regions_keep_tree_and_query_bounded()
    {
        const int count = 10_000;
        var layerId = MapLayerId.New();
        var index = new RegionSpatialIndex();
        MapRegionId expected = default;
        for (var item = 0; item < count; item++)
        {
            var region = Region(layerId, 0, -249950 + (item * 50));
            if (item == 4999) expected = region.RegionId;
            index.Upsert(region);
        }

        var result = index.QueryWithStats(new(-20, -20, 20, 20));

        Assert.Equal(count, index.Count);
        Assert.Equal((2 * count) - 1, index.NodeCount);
        Assert.True(index.Height <= (2 * Math.Ceiling(Math.Log2(count))) + 2);
        Assert.Equal([expected], result.RegionIds.ToArray());
        Assert.True(result.Stats.VisitedNodeCount < 256);
        Assert.True(result.Stats.TestedLeafCount <= 4);
    }

    [Fact]
    public void Repeated_upsert_and_full_remove_do_not_leave_orphan_nodes()
    {
        var layerId = MapLayerId.New();
        var index = new RegionSpatialIndex();
        for (var item = 0; item < 128; item++) index.Upsert(Region(layerId, item * 100, 5000));
        var moving = Region(layerId, -1000, 0);
        index.Upsert(moving);
        var expectedNodes = index.NodeCount;

        for (var item = 0; item < 1000; item++)
        {
            var x = (item & 1) == 0 ? 1000 : -1000;
            index.Upsert(moving with { Vertices = Points(x, 0) });
            Assert.Equal(expectedNodes, index.NodeCount);
        }

        Assert.Equal([moving.RegionId], index.Query(new(-1020, -20, -980, 20)).ToArray());
        var fullMap = index.QueryWithStats(new(-500000, -500000, 500000, 500000));
        Assert.Equal(expectedNodes, fullMap.Stats.VisitedNodeCount);
        foreach (var regionId in fullMap.RegionIds)
            Assert.True(index.Remove(regionId));
        Assert.Equal(0, index.NodeCount);
        Assert.Equal(0, index.Height);
    }

    static MapRegion Region(MapLayerId layerId, double x, double y) => new(
        MapRegionId.New(), layerId, "区域", MapRegionKind.Generic, Points(x, y));

    static System.Collections.Immutable.ImmutableArray<MapPoint> Points(double x, double y) =>
        [new(x - 10, y - 10), new(x + 10, y - 10), new(x, y + 10)];
}

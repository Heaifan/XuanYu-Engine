using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class GenericGeometryCapabilityTests
{
    [Fact]
    public void Region_and_road_map_to_distinct_geometry_kinds_and_capabilities()
    {
        var session = new MapEditSession();
        var region = new MapRegion(MapRegionId.New(), session.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [new(0, 0), new(10, 0), new(10, 10)]);
        var road = new MapRoad(MapRoadId.New(), session.ActiveRegionLayerId, "道路", "generic",
            [new(20, 0), new(30, 0), new(40, 0)]);
        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.True(session.CreateRoad(road).IsSuccess);
        Assert.True(GeometryFeatureAdapters.TryGet(session.CurrentMap,
            new(GeometryFeatureKind.Region, region.RegionId.ToString()), out var regionAdapter));
        Assert.True(GeometryFeatureAdapters.TryGet(session.CurrentMap,
            new(GeometryFeatureKind.Road, road.RoadId.ToString()), out var roadAdapter));
        Assert.Equal(GeometryKind.Polygon, regionAdapter.Kind);
        Assert.Equal(GeometryKind.Polyline, roadAdapter.Kind);
        Assert.True(regionAdapter.IsClosed);
        Assert.Equal(2, roadAdapter.SegmentCount);
        Assert.False(GeometrySnapPolicy.CanTarget(
            new(GeometryFeatureKind.Road, road.RoadId.ToString()),
            new(GeometryFeatureKind.Road, road.RoadId.ToString())));
    }

    [Fact]
    public void Local_geometry_query_returns_features_without_full_map_enumeration_api()
    {
        var session = new MapEditSession();
        var road = new MapRoad(MapRoadId.New(), session.ActiveRegionLayerId, "道路", "generic",
            [new(1000, 1000), new(1100, 1000)]);
        Assert.True(session.CreateRoad(road).IsSuccess);
        var local = session.QueryLocalGeometry(new(999, 999, 1001, 1001));
        Assert.Contains(new GeometryFeatureKey(GeometryFeatureKind.Road, road.RoadId.ToString()), local);
        Assert.Empty(session.QueryLocalGeometry(new(-1000, -1000, -900, -900)));
    }
}

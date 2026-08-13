using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionEdgeSnapResolverTests
{
    [Fact]
    public void Interior_edge_returns_closest_world_point()
    {
        var region = Region("00000000000000000000000000000002", [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        var result = Resolve(new(50, 3), [region]);
        Assert.Equal(RegionSnapKind.Edge, result.Kind); Assert.Equal(new MapPoint(50, 0), result.ResolvedPoint);
    }

    [Fact]
    public void Vertex_wins_when_raw_point_is_near_edge_endpoint()
    {
        var region = Region("00000000000000000000000000000002", [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        var result = Resolve(new(1, 1), [region]);
        Assert.Equal(RegionSnapKind.Vertex, result.Kind); Assert.Equal(0, result.TargetIndex);
    }

    [Fact]
    public void Source_region_is_excluded()
    {
        var source = Region("00000000000000000000000000000001", [new(0, 0), new(10, 0), new(10, 10)]);
        var result = RegionEdgeSnapResolver.Resolve(source.RegionId, new(5, 3), new(5, 3), [source],
            RegionEdgeSnapSettings.Default);
        Assert.False(result.IsSnapped);
    }

    [Fact]
    public void Equal_edge_distance_uses_region_id_then_edge_index()
    {
        var first = Region("00000000000000000000000000000001", [new(0, 0), new(10, 0), new(10, 10)]);
        var second = Region("00000000000000000000000000000002", [new(0, 0), new(10, 0), new(10, -10)]);
        var result = Resolve(new(5, 3), [second, first]);
        Assert.Equal(first.RegionId, result.TargetRegionId); Assert.Equal(0, result.TargetIndex);
    }

    [Fact]
    public void Zero_length_edge_is_safe()
    {
        var region = Region("00000000000000000000000000000002", [new(100, 100), new(100, 100), new(110, 100)]);
        var result = Resolve(new(0, 0), [region]);
        Assert.False(result.IsSnapped);
    }

    static RegionEdgeSnapResult Resolve(ScreenPoint point, ImmutableArray<RegionEdgeSnapRegion> regions) =>
        RegionEdgeSnapResolver.Resolve(MapRegionId.New(), new(point.X, point.Y), point, regions,
            RegionEdgeSnapSettings.Default);

    static RegionEdgeSnapRegion Region(string id, ImmutableArray<MapPoint> points)
    {
        MapRegionId.TryParse(id, out var regionId);
        return new(regionId, points.Select((point, index) => new RegionEdgeSnapVertex(regionId, index, point,
            new ScreenPoint(point.X, point.Y))).ToImmutableArray());
    }
}

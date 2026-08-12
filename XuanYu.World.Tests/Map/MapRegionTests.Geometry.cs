using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

public sealed partial class MapRegionTests
{
    [Fact]
    public void Irregular_quadrilateral_passes()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, Points((0, 0), (240, 30), (180, 210), (-40, 130)))), layers, Map10km);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Irregular_pentagon_passes()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, Points((0, 0), (180, -30), (260, 120), (90, 240), (-80, 120)))), layers, Map10km);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Simple_concave_polygon_passes()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, Points((0, 0), (240, 0), (240, 220), (130, 100), (0, 220)))), layers, Map10km);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Bow_tie_region_rejected()
    {
        var layers = Layers();
        var vertices = Points((0, 0), (100, 100), (0, 100), (100, 0));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, vertices)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("SelfIntersectingRegion", result.ErrorCode);
    }

    [Fact]
    public void Non_adjacent_touch_rejected()
    {
        var layers = Layers();
        var vertices = Points((0, 0), (100, 0), (100, 100), (50, 0), (0, 100));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, vertices)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("SelfIntersectingRegion", result.ErrorCode);
    }

    [Fact]
    public void Non_adjacent_overlap_rejected()
    {
        var layers = Layers();
        var vertices = Points((0, 0), (100, 0), (100, 100), (0, 100), (25, 0), (75, 0));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, vertices)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("SelfIntersectingRegion", result.ErrorCode);
    }

    static ImmutableArray<MapPoint> Points(params (double X, double Y)[] points) =>
        points.Select(point => new MapPoint(point.X, point.Y)).ToImmutableArray();
}

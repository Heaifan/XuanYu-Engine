using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1-F1：区域严格性（相邻重复点/首尾规则/三不同顶点/非零面积）。
public sealed partial class MapRegionTests
{
    [Fact]
    public void Adjacent_duplicate_vertex_rejected()
    {
        var layers = Layers();
        var duplicated = ImmutableArray.Create(
            new MapPoint(-100, -100), new MapPoint(100, -100),
            new MapPoint(100, 100), new MapPoint(100, 100), new MapPoint(-100, 100));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, duplicated)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("AdjacentDuplicateVertex", result.ErrorCode);
    }

    [Fact]
    public void First_and_last_duplicate_rejected()
    {
        var layers = Layers();
        var duplicated = ImmutableArray.Create(
            new MapPoint(-100, -100), new MapPoint(100, -100),
            new MapPoint(100, 100), new MapPoint(-100, 100), new MapPoint(-100, -100));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, duplicated)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("AdjacentDuplicateVertex", result.ErrorCode);
    }

    [Fact]
    public void Collinear_zero_area_rejected()
    {
        var layers = Layers();
        var collinear = ImmutableArray.Create(
            new MapPoint(0, 0), new MapPoint(10, 0), new MapPoint(20, 0));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, collinear)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("ZeroAreaRegion", result.ErrorCode);
    }

    [Fact]
    public void Too_few_distinct_vertices_rejected()
    {
        var layers = Layers();
        var alternating = ImmutableArray.Create(
            new MapPoint(0, 0), new MapPoint(10, 0),
            new MapPoint(0, 0), new MapPoint(10, 0));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, alternating)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("TooFewDistinctVertices", result.ErrorCode);
    }

    [Fact]
    public void Too_many_vertices_rejected()
    {
        var layers = Layers();
        var many = Enumerable.Range(0, MapRegionValidator.MaxVerticesPerRegion + 1)
            .Select(i => new MapPoint(i - 512.0, 0.0)).ToImmutableArray();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, many)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("TooManyRegionVertices", result.ErrorCode);
    }

    [Theory]
    [InlineData(double.NaN, 0.0), InlineData(0.0, double.PositiveInfinity)]
    public void Non_finite_vertex_rejected(double x, double y)
    {
        var layers = Layers();
        var bad = ImmutableArray.Create(new MapPoint(x, y), new MapPoint(0, 0), new MapPoint(1, 1));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, bad)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("NonFiniteRegionVertex", result.ErrorCode);
    }

    [Fact]
    public void Out_of_bounds_vertex_rejected()
    {
        var layers = Layers();
        var escaped = ImmutableArray.Create(
            new MapPoint(-100, -100), new MapPoint(100, -100),
            new MapPoint(6000, 100), new MapPoint(-100, 100));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, escaped)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("RegionVertexOutOfBounds", result.ErrorCode);
    }
}

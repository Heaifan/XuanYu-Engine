using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1：区域验证（闭合/顶点数/引用图层/边界/有限数值）。
public sealed partial class MapRegionTests
{
    [Fact]
    public void Valid_closed_region_passes()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId)), layers, Map10km);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Too_few_vertices_rejected()
    {
        var layers = Layers();
        var twoPoints = ImmutableArray.Create(new MapPoint(0, 0), new MapPoint(10, 10));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, twoPoints)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("TooFewRegionVertices", result.ErrorCode);
    }

    [Fact]
    public void Open_region_rejected()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[1].LayerId, closed: false)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("OpenRegion", result.ErrorCode);
    }

    [Fact]
    public void Unknown_layer_rejected()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(MapLayerId.New())), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownRegionLayer", result.ErrorCode);
    }

    [Fact]
    public void Duplicate_region_id_rejected()
    {
        var layers = Layers();
        var region = Region(layers[1].LayerId);
        var duplicated = region with { DisplayName = "另一个区域" };
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(region, duplicated), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateRegionId", result.ErrorCode);
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
    [InlineData(double.NaN, 0.0)]
    [InlineData(0.0, double.PositiveInfinity)]
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

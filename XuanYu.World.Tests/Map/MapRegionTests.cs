using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1：区域验证（闭合/顶点数/引用图层/边界/有限数值）。
public sealed partial class MapRegionTests
{
    [Fact]
    public void Valid_closed_region_passes()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId)), layers, Map10km);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Too_few_vertices_rejected()
    {
        var layers = Layers();
        var twoPoints = ImmutableArray.Create(new MapPoint(0, 0), new MapPoint(10, 10));
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[2].LayerId, twoPoints)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("TooFewRegionVertices", result.ErrorCode);
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
    public void Region_on_base_layer_rejected()
    {
        var layers = Layers();
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(Region(layers[0].LayerId)), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("RegionOnBaseLayer", result.ErrorCode);
    }

    [Fact]
    public void Duplicate_region_id_rejected()
    {
        var layers = Layers();
        var region = Region(layers[2].LayerId);
        var duplicated = region with { DisplayName = "另一个区域" };
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(region, duplicated), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateRegionId", result.ErrorCode);
    }

    [Fact]
    public void Invalid_region_id_rejected()
    {
        var layers = Layers();
        var region = Region(layers[2].LayerId) with { RegionId = default };
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(region), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidRegionId", result.ErrorCode);
    }

    [Fact]
    public void Unknown_region_kind_rejected()
    {
        var layers = Layers();
        var unknown = Region(layers[2].LayerId) with { Kind = (MapRegionKind)99 };
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(unknown), layers, Map10km);
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownRegionKind", result.ErrorCode);
    }
}

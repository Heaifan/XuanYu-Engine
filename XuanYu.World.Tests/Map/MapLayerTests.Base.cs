using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D4：系统图层合同（地面层恰好一个 Order 0、边界层恰好一个 Order 1、区域层 Order ≥ 2）。
public sealed partial class MapLayerTests
{
    [Fact]
    public void Multiple_ground_layers_rejected()
    {
        var layers = Default();
        var twoGround = layers.SetItem(2, layers[2] with { Kind = MapLayerKind.Ground });
        var result = MapLayerValidator.Validate(twoGround);
        Assert.False(result.Succeeded);
        Assert.Equal("GroundLayerCount", result.ErrorCode);
    }
    [Fact]
    public void Missing_ground_layer_rejected()
    {
        var layers = ImmutableArray.Create(
            new MapLayer(MapLayerId.New(), "边界", 1, MapLayerKind.Boundary),
            new MapLayer(MapLayerId.New(), "区域 1", 2, MapLayerKind.Region),
            new MapLayer(MapLayerId.New(), "区域 2", 3, MapLayerKind.Region));
        var result = MapLayerValidator.Validate(layers);
        Assert.False(result.Succeeded);
        Assert.Equal("GroundLayerCount", result.ErrorCode);
    }

    [Fact]
    public void Multiple_boundary_layers_rejected()
    {
        var layers = Default();
        var twoBoundary = layers.SetItem(2, layers[2] with { Kind = MapLayerKind.Boundary });
        var result = MapLayerValidator.Validate(twoBoundary);
        Assert.False(result.Succeeded);
        Assert.Equal("BoundaryLayerCount", result.ErrorCode);
    }
    [Fact]
    public void Missing_boundary_layer_rejected()
    {
        var layers = ImmutableArray.Create(
            new MapLayer(MapLayerId.New(), "地面", 0, MapLayerKind.Ground),
            new MapLayer(MapLayerId.New(), "区域 1", 2, MapLayerKind.Region),
            new MapLayer(MapLayerId.New(), "区域 2", 3, MapLayerKind.Region));
        var result = MapLayerValidator.Validate(layers);
        Assert.False(result.Succeeded);
        Assert.Equal("BoundaryLayerCount", result.ErrorCode);
    }

    [Fact]
    public void Unknown_layer_kind_rejected()
    {
        var layers = Default();
        var unknown = layers.SetItem(2, layers[2] with { Kind = (MapLayerKind)99 });
        var result = MapLayerValidator.Validate(unknown);
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownLayerKind", result.ErrorCode);
    }

    [Fact]
    public void Ground_layer_must_be_order_zero()
    {
        var layers = Default();
        var wrongOrder = layers.SetItem(0, layers[0] with { Order = 1 })
            .SetItem(1, layers[1] with { Order = 0 });
        var result = MapLayerValidator.Validate(wrongOrder);
        Assert.False(result.Succeeded);
        Assert.Equal("GroundLayerOrder", result.ErrorCode);
    }

    [Fact]
    public void Boundary_layer_must_be_order_one()
    {
        var layers = Default();
        var wrongOrder = layers.SetItem(1, layers[1] with { Order = 2 });
        var result = MapLayerValidator.Validate(wrongOrder);
        Assert.False(result.Succeeded);
        Assert.Equal("BoundaryLayerOrder", result.ErrorCode);
    }

    [Fact]
    public void Region_layer_order_below_two_rejected()
    {
        var layers = Default();
        var wrongOrder = layers.SetItem(2, layers[2] with { Order = 1 });
        var result = MapLayerValidator.Validate(wrongOrder);
        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateLayerOrder", result.ErrorCode);
    }
    [Fact]
    public void Zero_region_layers_rejected()
    {
        var layers = Default();
        var noRegion = layers.RemoveAt(2);
        var result = MapLayerValidator.Validate(noRegion);
        Assert.False(result.Succeeded);
        Assert.Equal("RegionLayerCount", result.ErrorCode);
    }
}

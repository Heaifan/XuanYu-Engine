using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D4：图层顺序与状态操作（T03/T09 区域层内排序、T11/T12 显隐锁定保身份）。
public sealed partial class MapLayerStackTests
{
    static ImmutableArray<MapLayer> Default() => MapDefaultDefinition.CreateDefault().Layers;

    static ImmutableArray<MapLayer> ThreeRegions()
    {
        var layers = Default();
        layers = layers.Add(new MapLayer(MapLayerId.New(), "区域 2", 3, MapLayerKind.Region));
        return layers.Add(new MapLayer(MapLayerId.New(), "区域 3", 4, MapLayerKind.Region));
    }

    [Fact]
    public void Create_region_layer_gets_unique_id_and_top_order()
    {
        var layers = Default();
        var layer = MapLayerStack.CreateRegionLayer(layers, "区域 2");
        Assert.True(layer.LayerId.IsValid);
        Assert.Equal(3, layer.Order);
        Assert.True(layers.All(l => l.LayerId != layer.LayerId));
        Assert.True(MapLayerValidator.Validate(layers.Add(layer)).Succeeded);
    }

    [Fact]
    public void Region_layers_ordered_top_first()
    {
        var layers = ThreeRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        Assert.Equal("区域 3", regions[0].DisplayName);
        Assert.Equal("区域 1", regions[2].DisplayName);
    }

    [Fact]
    public void Move_up_swaps_orders_within_region_range_only()
    {
        var layers = ThreeRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveUp(layers, regions[1].LayerId);
        var after = MapLayerStack.RegionLayers(moved);
        Assert.Equal(regions[1].LayerId, after[0].LayerId);
        Assert.Equal(regions[0].LayerId, after[1].LayerId);
        Assert.True(MapLayerValidator.Validate(moved).Succeeded);
        Assert.Equal(0, moved.First(l => l.Kind == MapLayerKind.Ground).Order);
        Assert.Equal(1, moved.First(l => l.Kind == MapLayerKind.Boundary).Order);
    }

    [Fact]
    public void Move_down_swaps_orders_within_region_range_only()
    {
        var layers = ThreeRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveDown(layers, regions[0].LayerId);
        var after = MapLayerStack.RegionLayers(moved);
        Assert.Equal(regions[0].LayerId, after[1].LayerId);
        Assert.Equal(regions[1].LayerId, after[0].LayerId);
        Assert.True(MapLayerValidator.Validate(moved).Succeeded);
    }
}

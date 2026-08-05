using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D4-F3：区域图层拖动排序领域合同（T01～T08）。
public sealed partial class MapLayerStackTests
{
    static ImmutableArray<MapLayer> DragRegions()
    {
        var layers = MapDefaultDefinition.CreateDefault().Layers;
        layers = layers.Add(new MapLayer(MapLayerId.New(), "区域 2", 3, MapLayerKind.Region));
        return layers.Add(new MapLayer(MapLayerId.New(), "区域 3", 4, MapLayerKind.Region));
    }

    [Fact]
    public void T01_region_layer_moves_to_any_valid_index()
    {
        var layers = DragRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveRegionToIndex(layers, regions[0].LayerId, 2);
        var after = MapLayerStack.RegionLayers(moved);
        Assert.Equal(regions[0].LayerId, after[2].LayerId);
        Assert.Equal(regions[1].LayerId, after[0].LayerId);
        Assert.Equal(regions[2].LayerId, after[1].LayerId);
    }

    [Fact]
    public void T02_system_layers_keep_orders_zero_one()
    {
        var layers = DragRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveRegionToIndex(layers, regions[0].LayerId, 2);
        Assert.Equal(0, moved.First(l => l.Kind == MapLayerKind.Ground).Order);
        Assert.Equal(1, moved.First(l => l.Kind == MapLayerKind.Boundary).Order);
    }

    [Fact]
    public void T03_region_orders_continuous_and_unique()
    {
        var layers = DragRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveRegionToIndex(layers, regions[0].LayerId, 2);
        var orders = moved.Where(l => l.Kind == MapLayerKind.Region).Select(l => l.Order).OrderBy(o => o).ToArray();
        Assert.Equal(new[] { 2, 3, 4 }, orders);
        Assert.True(MapLayerValidator.Validate(moved).Succeeded);
    }

    [Fact]
    public void T04_identity_visibility_lock_unchanged()
    {
        var layers = DragRegions().SetItem(2, DragRegions()[2] with { IsVisible = false, IsLocked = true });
        var regions = MapLayerStack.RegionLayers(layers);
        var moved = MapLayerStack.MoveRegionToIndex(layers, regions[2].LayerId, 0);
        var after = moved.First(l => l.LayerId == regions[2].LayerId);
        Assert.Equal(regions[2].DisplayName, after.DisplayName);
        Assert.Equal(regions[2].LayerId, after.LayerId);
        Assert.False(after.IsVisible);
        Assert.True(after.IsLocked);
    }

    [Fact]
    public void T05_same_position_is_noop()
    {
        var layers = DragRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        Assert.Equal(layers, MapLayerStack.MoveRegionToIndex(layers, regions[1].LayerId, 1));
    }

    [Fact]
    public void T06_system_layer_move_returns_original()
    {
        var layers = DragRegions();
        Assert.Equal(layers, MapLayerStack.MoveRegionToIndex(layers, layers[0].LayerId, 0));
    }

    [Fact]
    public void T07_unknown_layer_returns_original()
    {
        var layers = DragRegions();
        Assert.Equal(layers, MapLayerStack.MoveRegionToIndex(layers, MapLayerId.New(), 0));
    }

    [Fact]
    public void T08_out_of_range_target_returns_original()
    {
        var layers = DragRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        Assert.Equal(layers, MapLayerStack.MoveRegionToIndex(layers, regions[0].LayerId, -1));
        Assert.Equal(layers, MapLayerStack.MoveRegionToIndex(layers, regions[0].LayerId, 9));
    }
}

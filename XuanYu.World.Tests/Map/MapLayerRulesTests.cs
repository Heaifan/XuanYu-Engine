using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D4：图层操作规则（T05 名称校验、T06 系统层保护、T07/T08 删除保护、T04 自动命名）。
public sealed class MapLayerRulesTests
{
    static ImmutableArray<MapLayer> Default() => MapDefaultDefinition.CreateDefault().Layers;

    static ImmutableArray<MapLayer> WithRegion(string name, int order) =>
        Default().Add(new MapLayer(MapLayerId.New(), name, order, MapLayerKind.Region));

    [Fact]
    public void Blank_and_whitespace_names_rejected()
    {
        Assert.NotNull(MapLayerRules.ValidateName(""));
        Assert.NotNull(MapLayerRules.ValidateName("   "));
        Assert.NotNull(MapLayerRules.ValidateName(null));
    }

    [Fact]
    public void Overlong_name_rejected()
    {
        Assert.NotNull(MapLayerRules.ValidateName(new string('长', 33)));
        Assert.Null(MapLayerRules.ValidateName(new string('长', 32)));
    }

    [Fact]
    public void Control_characters_rejected()
    {
        Assert.NotNull(MapLayerRules.ValidateName("a\nb"));
        Assert.NotNull(MapLayerRules.ValidateName("a\tb"));
        Assert.Null(MapLayerRules.ValidateName("主战区"));
    }

    [Fact]
    public void Next_region_name_uses_smallest_available_number()
    {
        Assert.Equal("区域 2", MapLayerRules.NextRegionName(Default()));
        var renamed = Default().SetItem(2, Default()[2] with { DisplayName = "主战区" });
        Assert.Equal("区域 1", MapLayerRules.NextRegionName(renamed));
        var withTwo = WithRegion("区域 2", 3);
        Assert.Equal("区域 3", MapLayerRules.NextRegionName(withTwo));
        var reused = Default().SetItem(2, Default()[2] with { DisplayName = "区域 5" });
        Assert.Equal("区域 1", MapLayerRules.NextRegionName(reused));
    }

    [Fact]
    public void System_layers_cannot_be_removed()
    {
        var layers = Default();
        Assert.NotNull(MapLayerRules.CanRemove(layers, layers[0].LayerId));
        Assert.NotNull(MapLayerRules.CanRemove(layers, layers[1].LayerId));
        var two = WithRegion("区域 2", 3);
        Assert.Null(MapLayerRules.CanRemove(two, two[2].LayerId));
    }

    [Fact]
    public void Last_region_layer_cannot_be_removed()
    {
        var layers = Default();
        Assert.NotNull(MapLayerRules.CanRemove(layers, layers[2].LayerId));
        var two = WithRegion("区域 2", 3);
        Assert.Null(MapLayerRules.CanRemove(two, two[2].LayerId));
    }

    [Fact]
    public void Unknown_layer_cannot_be_removed()
    {
        Assert.NotNull(MapLayerRules.CanRemove(Default(), MapLayerId.New()));
    }

    [Fact]
    public void System_layers_cannot_be_moved()
    {
        var layers = Default();
        Assert.NotNull(MapLayerRules.CanMove(layers, layers[0].LayerId, up: true));
        Assert.NotNull(MapLayerRules.CanMove(layers, layers[1].LayerId, up: false));
    }

    [Fact]
    public void Top_region_cannot_move_up_and_bottom_cannot_move_down()
    {
        var layers = WithRegion("区域 2", 3);
        var regions = MapLayerStack.RegionLayers(layers);
        Assert.NotNull(MapLayerRules.CanMove(layers, regions[0].LayerId, up: true));
        Assert.Null(MapLayerRules.CanMove(layers, regions[0].LayerId, up: false));
        Assert.NotNull(MapLayerRules.CanMove(layers, regions[^1].LayerId, up: false));
        Assert.Null(MapLayerRules.CanMove(layers, regions[^1].LayerId, up: true));
    }
}

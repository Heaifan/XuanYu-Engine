using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1：图层领域模型与验证（默认图层/稳定 ID/唯一性）。
public sealed partial class MapLayerTests
{
    static ImmutableArray<MapLayer> Default() => MapDefaultDefinition.CreateDefault().Layers;

    [Fact]
    public void Default_layers_contain_base_and_region()
    {
        var layers = Default();
        Assert.Equal(2, layers.Length);
        Assert.Equal("基础地图", layers[0].DisplayName);
        Assert.Equal(0, layers[0].Order);
        Assert.Equal(MapLayerKind.Base, layers[0].Kind);
        Assert.Equal("区域", layers[1].DisplayName);
        Assert.Equal(1, layers[1].Order);
        Assert.Equal(MapLayerKind.Region, layers[1].Kind);
    }

    [Fact]
    public void Default_layers_have_unique_ids_and_pass_validation()
    {
        var layers = Default();
        Assert.NotEqual(layers[0].LayerId, layers[1].LayerId);
        Assert.True(MapLayerValidator.Validate(layers).Succeeded);
    }

    [Fact]
    public void Duplicate_layer_id_rejected()
    {
        var layers = Default();
        var duplicated = layers.SetItem(1, layers[1] with { LayerId = layers[0].LayerId });
        var result = MapLayerValidator.Validate(duplicated);
        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateLayerId", result.ErrorCode);
    }

    [Fact]
    public void Blank_layer_name_rejected()
    {
        var layers = Default();
        var renamed = layers.SetItem(1, layers[1] with { DisplayName = "  " });
        var result = MapLayerValidator.Validate(renamed);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidLayerName", result.ErrorCode);
    }

    [Fact]
    public void Negative_order_rejected()
    {
        var layers = Default();
        var reordered = layers.SetItem(0, layers[0] with { Order = -1 });
        var result = MapLayerValidator.Validate(reordered);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidLayerOrder", result.ErrorCode);
    }

    [Fact]
    public void Duplicate_layer_order_rejected()
    {
        var layers = Default();
        var reordered = layers.SetItem(1, layers[1] with { Order = 0 });
        var result = MapLayerValidator.Validate(reordered);
        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateLayerOrder", result.ErrorCode);
    }

    [Fact]
    public void Empty_layer_list_rejected()
    {
        var result = MapLayerValidator.Validate(ImmutableArray<MapLayer>.Empty);
        Assert.False(result.Succeeded);
        Assert.Equal("EmptyLayerList", result.ErrorCode);
    }
}

using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1-F1：基础层合同（必须且仅有一个、位于第 0 位、稳定角色标识）。
public sealed partial class MapLayerTests
{
    [Fact]
    public void Multiple_base_layers_rejected()
    {
        var layers = Default();
        var twoBase = layers.SetItem(1, layers[1] with { Kind = MapLayerKind.Base });
        var result = MapLayerValidator.Validate(twoBase);
        Assert.False(result.Succeeded);
        Assert.Equal("BaseLayerCount", result.ErrorCode);
    }

    [Fact]
    public void Missing_base_layer_rejected()
    {
        var layers = Default();
        var noBase = layers.SetItem(0, layers[0] with { Kind = MapLayerKind.Custom });
        var result = MapLayerValidator.Validate(noBase);
        Assert.False(result.Succeeded);
        Assert.Equal("BaseLayerCount", result.ErrorCode);
    }

    [Fact]
    public void Unknown_layer_kind_rejected()
    {
        var layers = Default();
        var unknown = layers.SetItem(1, layers[1] with { Kind = (MapLayerKind)99 });
        var result = MapLayerValidator.Validate(unknown);
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownLayerKind", result.ErrorCode);
    }

    [Fact]
    public void Base_layer_must_be_order_zero()
    {
        var layers = Default();
        var wrongOrder = layers.SetItem(0, layers[0] with { Order = 1 })
            .SetItem(1, layers[1] with { Order = 0, Kind = MapLayerKind.Custom });
        var result = MapLayerValidator.Validate(wrongOrder);
        Assert.False(result.Succeeded);
        Assert.Equal("BaseLayerOrder", result.ErrorCode);
    }
}

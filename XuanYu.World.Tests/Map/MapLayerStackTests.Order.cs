using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D4：图层顺序边界与状态操作（T10 系统层顺序固定、显隐/锁定/改名保身份）。
public sealed partial class MapLayerStackTests
{
    [Fact]
    public void Move_at_boundary_is_noop()
    {
        var layers = ThreeRegions();
        var regions = MapLayerStack.RegionLayers(layers);
        Assert.Equal(layers, MapLayerStack.MoveUp(layers, regions[0].LayerId));
        Assert.Equal(layers, MapLayerStack.MoveDown(layers, regions[^1].LayerId));
    }

    [Fact]
    public void Remove_keeps_system_layers_untouched()
    {
        var layers = ThreeRegions();
        var removed = MapLayerStack.Remove(layers, MapLayerStack.RegionLayers(layers)[0].LayerId);
        Assert.Equal(4, removed.Length);
        Assert.Equal("地面", removed[0].DisplayName);
        Assert.Equal("边界", removed[1].DisplayName);
        Assert.True(MapLayerValidator.Validate(removed).Succeeded);
    }

    [Fact]
    public void Visibility_change_keeps_layer_identity()
    {
        var layers = MapDefaultDefinition.CreateDefault().Layers;
        var layer = layers[2];
        var updated = MapLayerStack.SetVisibility(layer, false);
        Assert.False(updated.IsVisible);
        Assert.Equal(layer.LayerId, updated.LayerId);
        Assert.Equal(layer.DisplayName, updated.DisplayName);
        Assert.Equal(layer.Order, updated.Order);
    }

    [Fact]
    public void Lock_change_keeps_layer_identity()
    {
        var layer = MapDefaultDefinition.CreateDefault().Layers[2];
        var updated = MapLayerStack.SetLocked(layer, true);
        Assert.True(updated.IsLocked);
        Assert.Equal(layer.LayerId, updated.LayerId);
        Assert.Equal(layer.DisplayName, updated.DisplayName);
    }

    [Fact]
    public void Rename_keeps_layer_identity()
    {
        var layer = MapDefaultDefinition.CreateDefault().Layers[2];
        var renamed = MapLayerStack.Rename(layer, "主战区");
        Assert.Equal("主战区", renamed.DisplayName);
        Assert.Equal(layer.LayerId, renamed.LayerId);
        Assert.Equal(layer.Order, renamed.Order);
    }
}

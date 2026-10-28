using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D4：图层顺序与领域操作（纯函数，返回新不可变集合）。
// Order 语义：值越大越靠前（显示在上方）；地面=0、边界=1、区域层 ≥ 2。
// 区域层排序只在区域层之间交换 Order，绝不创建/销毁图层 ID。
public static class MapLayerStack
{
    // 区域图层按 Order 降序（UI 从上到下）。
    public static ImmutableArray<MapLayer> RegionLayers(ImmutableArray<MapLayer> layers) =>
        layers.Where(l => l.Kind == MapLayerKind.Region)
            .OrderByDescending(l => l.Order)
            .ToImmutableArray();

    // 新建区域图层（放在区域层最上方 = 全局最大 Order）。
    public static MapLayer CreateRegionLayer(ImmutableArray<MapLayer> layers, string name) =>
        new(MapLayerId.New(), name, layers.Max(l => l.Order) + 1, MapLayerKind.Region);

    public static ImmutableArray<MapLayer> Remove(
        ImmutableArray<MapLayer> layers, MapLayerId layerId) =>
        layers.RemoveAll(l => l.LayerId == layerId);

    // 区域图层上移一步：与上方相邻区域层交换 Order。
    public static ImmutableArray<MapLayer> MoveUp(
        ImmutableArray<MapLayer> layers, MapLayerId layerId) =>
        Move(layers, layerId, up: true);

    public static ImmutableArray<MapLayer> MoveDown(
        ImmutableArray<MapLayer> layers, MapLayerId layerId) =>
        Move(layers, layerId, up: false);

    static ImmutableArray<MapLayer> Move(
        ImmutableArray<MapLayer> layers, MapLayerId layerId, bool up)
    {
        var regions = RegionLayers(layers);
        var index = MapLayerRules.IndexOfId(regions, layerId);
        if (index < 0) return layers;
        var targetIndex = up ? index - 1 : index + 1;
        if (targetIndex < 0 || targetIndex >= regions.Length) return layers;
        var current = regions[index];
        var target = regions[targetIndex];
        var currentOrder = current.Order;
        return layers
            .SetItem(layers.IndexOf(current), current with { Order = target.Order })
            .SetItem(layers.IndexOf(target), target with { Order = currentOrder });
    }

    public static MapLayer Rename(MapLayer layer, string name) =>
        layer with { DisplayName = name };

    public static MapLayer SetVisibility(MapLayer layer, bool visible) =>
        layer with { IsVisible = visible };

    public static MapLayer SetLocked(MapLayer layer, bool locked) =>
        layer with { IsLocked = locked };

    // F3：区域图层拖动排序——targetIndex 0=最上方区域图层；仅区域层间重排，
    // 重新分配连续 Order（地面=0、边界=1、最下方区域=2、越靠上越大）；
    // 同位置返回原集合；LayerId/名称/显隐/锁定保持不变。
    public static ImmutableArray<MapLayer> MoveRegionToIndex(
        ImmutableArray<MapLayer> layers, MapLayerId layerId, int targetIndex)
    {
        var regions = RegionLayers(layers);
        var sourceIndex = MapLayerRules.IndexOfId(regions, layerId);
        if (sourceIndex < 0) return layers;
        if (sourceIndex == targetIndex) return layers;
        if (targetIndex < 0 || targetIndex >= regions.Length) return layers; // 越界安全返回原集合
        var list = regions.ToList();
        var moving = list[sourceIndex];
        list.RemoveAt(sourceIndex);
        list.Insert(targetIndex, moving);
        var order = 2 + list.Count - 1; // 最上方区域层 Order
        var byId = new Dictionary<MapLayerId, MapLayer>();
        foreach (var item in list)
            byId[item.LayerId] = item with { Order = order-- };
        return layers.Select(l => byId.TryGetValue(l.LayerId, out var updated) ? updated : l)
            .ToImmutableArray();
    }
}

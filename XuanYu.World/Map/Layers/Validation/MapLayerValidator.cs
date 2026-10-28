using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D4：图层集合严格校验（领域权威层）。
// 检查：ID 合法且唯一、名称非空、顺序非负且唯一、
// 地面层（Ground）必须且仅有一个且 Order 0、边界层（Boundary）必须且仅有一个且 Order 1、
// 区域层（Region）至少一个且 Order ≥ 2。
public static class MapLayerValidator
{
    public static MapValidationResult Validate(ImmutableArray<MapLayer> layers)
    {
        if (layers.IsDefault)
            return MapValidationResult.Fail("InvalidLayerList", "图层集合缺失。");
        if (layers.Length == 0)
            return MapValidationResult.Fail("EmptyLayerList", "图层集合不能为空。");

        var ids = new HashSet<MapLayerId>();
        var orders = new HashSet<int>();
        var groundCount = 0;
        var boundaryCount = 0;
        var regionCount = 0;
        foreach (var layer in layers)
        {
            if (!layer.LayerId.IsValid)
                return MapValidationResult.Fail("InvalidLayerId", $"图层ID非法：{layer.LayerId}。");
            if (!Enum.IsDefined(layer.Kind))
                return MapValidationResult.Fail("UnknownLayerKind", $"图层角色未知：{layer.DisplayName}。");
            if (!ids.Add(layer.LayerId))
                return MapValidationResult.Fail("DuplicateLayerId", $"图层ID重复：{layer.LayerId}。");
            if (string.IsNullOrWhiteSpace(layer.DisplayName))
                return MapValidationResult.Fail("InvalidLayerName", $"图层名称不能为空：{layer.LayerId}。");
            if (layer.Order < 0)
                return MapValidationResult.Fail("InvalidLayerOrder", $"图层顺序不得为负：{layer.DisplayName}。");
            if (!orders.Add(layer.Order))
                return MapValidationResult.Fail("DuplicateLayerOrder", $"图层顺序重复：{layer.Order}。");
            if (layer.Kind == MapLayerKind.Ground)
            {
                groundCount++;
                if (groundCount > 1)
                    return MapValidationResult.Fail("GroundLayerCount", "地面层必须且仅有一个。");
                if (layer.Order != 0)
                    return MapValidationResult.Fail("GroundLayerOrder", "地面层顺序必须为 0。");
            }
            else if (layer.Kind == MapLayerKind.Boundary)
            {
                boundaryCount++;
                if (boundaryCount > 1)
                    return MapValidationResult.Fail("BoundaryLayerCount", "边界层必须且仅有一个。");
                if (layer.Order != 1)
                    return MapValidationResult.Fail("BoundaryLayerOrder", "边界层顺序必须为 1。");
            }
            else
            {
                regionCount++;
                if (layer.Order < 2)
                    return MapValidationResult.Fail("RegionLayerOrder", "区域图层顺序必须大于等于 2。");
            }
        }

        if (groundCount != 1)
            return MapValidationResult.Fail("GroundLayerCount", "地面层必须且仅有一个。");
        if (boundaryCount != 1)
            return MapValidationResult.Fail("BoundaryLayerCount", "边界层必须且仅有一个。");
        if (regionCount < 1)
            return MapValidationResult.Fail("RegionLayerCount", "区域图层至少需要一个。");
        return MapValidationResult.Ok();
    }
}

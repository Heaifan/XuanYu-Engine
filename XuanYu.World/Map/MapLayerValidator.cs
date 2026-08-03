using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1：图层集合严格校验（领域权威层）。
// 检查：ID 合法且唯一、名称非空、顺序非负且唯一、基础层（Base）必须且仅有一个且位于第 0 位。
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
        var baseCount = 0;
        foreach (var layer in layers)
        {
            if (!layer.LayerId.IsValid)
                return MapValidationResult.Fail("InvalidLayerId", $"图层ID非法：{layer.LayerId}。");
            if (!ids.Add(layer.LayerId))
                return MapValidationResult.Fail("DuplicateLayerId", $"图层ID重复：{layer.LayerId}。");
            if (string.IsNullOrWhiteSpace(layer.DisplayName))
                return MapValidationResult.Fail("InvalidLayerName", $"图层名称不能为空：{layer.LayerId}。");
            if (layer.Order < 0)
                return MapValidationResult.Fail("InvalidLayerOrder", $"图层顺序不得为负：{layer.DisplayName}。");
            if (!orders.Add(layer.Order))
                return MapValidationResult.Fail("DuplicateLayerOrder", $"图层顺序重复：{layer.Order}。");
            if (layer.Kind == MapLayerKind.Base)
            {
                baseCount++;
                if (baseCount > 1)
                    return MapValidationResult.Fail("BaseLayerCount", "基础层必须且仅有一个。");
                if (layer.Order != 0)
                    return MapValidationResult.Fail("BaseLayerOrder", "基础层顺序必须为 0。");
            }
        }

        if (baseCount != 1)
            return MapValidationResult.Fail("BaseLayerCount", "基础层必须且仅有一个。");
        return MapValidationResult.Ok();
    }
}

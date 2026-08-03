using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：图层集合严格校验。返回结构化结果，不抛出来源不明的异常。
// 检查：ID 唯一、名称非空、顺序非负、固定层至多一个。
public static class MapLayerValidator
{
    public static MapDocumentResult<bool> Validate(
        ImmutableArray<MapLayer> layers)
    {
        if (layers.IsDefault)
            return Fail("InvalidLayerList", "图层集合缺失。");
        if (layers.Length == 0)
            return Fail("EmptyLayerList", "图层集合不能为空。");

        var ids = new HashSet<MapLayerId>();
        var fixedCount = 0;
        foreach (var layer in layers)
        {
            if (!ids.Add(layer.LayerId))
                return Fail("DuplicateLayerId", $"图层ID重复：{layer.LayerId}。");
            if (string.IsNullOrWhiteSpace(layer.DisplayName))
                return Fail("InvalidLayerName", $"图层名称不能为空：{layer.LayerId}。");
            if (layer.Order < 0)
                return Fail("InvalidLayerOrder", $"图层顺序不得为负：{layer.DisplayName}。");
            if (layer.IsFixed) fixedCount++;
        }

        if (fixedCount > 1)
            return Fail("MultipleFixedLayers", "固定层（不可删除）至多一个。");
        return MapDocumentResult<bool>.Ok(true);
    }

    static MapDocumentResult<bool> Fail(string code, string message) =>
        MapDocumentResult<bool>.Fail(code, message, "ValidateLayers");
}

using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D4：图层操作规则（名称校验、系统层保护、最后区域层保护、自动命名）。
// 纯静态规则：不修改状态，只回答"能否"与"叫什么"；修改由 MapLayerStack/会话命令执行。
public static class MapLayerRules
{
    public const int MinNameLength = 1;
    public const int MaxNameLength = 32;

    public static bool IsSystemLayer(MapLayerKind kind) =>
        kind is MapLayerKind.Ground or MapLayerKind.Boundary;

    // 返回错误消息；合法返回 null。规则：去首尾空格后非空、1～32 字符、无换行/控制字符。
    public static string? ValidateName(string? name)
    {
        var trimmed = name?.Trim() ?? "";
        if (trimmed.Length < MinNameLength)
            return "图层名称不能为空。";
        if (trimmed.Length > MaxNameLength)
            return $"图层名称长度不能超过 {MaxNameLength} 个字符。";
        if (trimmed.Any(char.IsControl))
            return "图层名称不能包含换行或控制字符。";
        return null;
    }

    // 自动名称：按可用序号生成"区域 N"（N = 最小未使用正整数，名称允许重复）。
    public static string NextRegionName(ImmutableArray<MapLayer> layers)
    {
        var used = layers
            .Where(l => l.Kind == MapLayerKind.Region)
            .Select(l => ParseRegionNumber(l.DisplayName))
            .Where(n => n > 0)
            .ToHashSet();
        var n = 1;
        while (used.Contains(n)) n++;
        return $"区域 {n}";
    }

    // 删除保护：系统图层禁删、最后一个区域图层禁删。返回错误消息或 null。
    public static string? CanRemove(ImmutableArray<MapLayer> layers, MapLayerId layerId)
    {
        var layer = Find(layers, layerId);
        if (layer is null) return "图层不存在。";
        if (IsSystemLayer(layer.Kind)) return "系统图层不能删除。";
        if (layers.Count(l => l.Kind == MapLayerKind.Region) <= 1)
            return "至少保留一个区域图层。";
        return null;
    }

    // 顺序保护：仅区域图层可移动；最上方禁上移、最下方禁下移。返回错误消息或 null。
    public static string? CanMove(ImmutableArray<MapLayer> layers, MapLayerId layerId, bool up)
    {
        var layer = Find(layers, layerId);
        if (layer is null) return "图层不存在。";
        if (layer.Kind != MapLayerKind.Region) return "系统图层不能调整顺序。";
        var regions = MapLayerStack.RegionLayers(layers);
        var index = IndexOfId(regions, layerId);
        if (up && index == 0) return "该图层已位于区域图层最上方。";
        if (!up && index == regions.Length - 1) return "该图层已位于区域图层最下方。";
        return null;
    }

    public static MapLayer? Find(ImmutableArray<MapLayer> layers, MapLayerId layerId) =>
        layers.FirstOrDefault(l => l.LayerId == layerId);

    public static int IndexOfId(ImmutableArray<MapLayer> layers, MapLayerId layerId)
    {
        for (var i = 0; i < layers.Length; i++)
            if (layers[i].LayerId == layerId) return i;
        return -1;
    }

    static int ParseRegionNumber(string name)
    {
        if (!name.StartsWith("区域 ", StringComparison.Ordinal)) return 0;
        return int.TryParse(name.AsSpan(3), out var n) ? n : 0;
    }
}

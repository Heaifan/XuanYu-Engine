using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class MapObjectNameAllocator
{
    public static string Region(MapDefinition map, string requested) =>
        Allocate(map.Regions.Select(item => item.DisplayName), requested, "区域", "区域");

    public static string Road(MapDefinition map, string requested) =>
        Allocate(map.Roads.Select(item => item.DisplayName), requested, "道路", "道路");

    public static string Marker(MapDefinition map, string requested) =>
        Allocate(map.Markers.IsDefault ? [] : map.Markers.Select(item => item.DisplayName), requested, "地图标记", "标记");

    static string Allocate(IEnumerable<string> existing, string requested, string placeholder, string prefix)
    {
        if (!string.IsNullOrWhiteSpace(requested) && requested.Trim() != placeholder) return requested.Trim();
        var names = existing.ToHashSet(StringComparer.Ordinal);
        for (var index = 1; ; index++)
        {
            var candidate = $"{prefix}{index}";
            if (!names.Contains(candidate)) return candidate;
        }
    }
}

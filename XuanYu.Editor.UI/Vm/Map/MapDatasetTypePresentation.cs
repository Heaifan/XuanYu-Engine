using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed record MapDatasetTypeOption(string Value, string Display);

public static class MapDatasetTypePresentation
{
    public static IReadOnlyList<MapDatasetTypeOption> Options { get; } =
        MapDatasetTypes.All.Select(type => new MapDatasetTypeOption(type, Display(type))).ToArray();

    public static string Display(string type) => type switch
    {
        MapDatasetTypes.Region => "区域",
        MapDatasetTypes.Road => "道路",
        MapDatasetTypes.Settlement => "城镇",
        MapDatasetTypes.Resource => "资源",
        MapDatasetTypes.River => "河流",
        MapDatasetTypes.TerrainArea => "地形区域",
        _ => type
    };
}

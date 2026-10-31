using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed record MapDatasetTypeOption(string Value, string Display);

public static class MapDatasetTypePresentation
{
    public static IReadOnlyList<MapDatasetTypeOption> Options { get; } =
        MapDatasetTypes.All.Select(type => new MapDatasetTypeOption(type, Display(type))).ToArray();

    public static string Display(string type) => MapDatasetTypes.DisplayName(type);
}

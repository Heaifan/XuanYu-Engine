using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetTypes
{
    public const string Region = "region";
    public const string Road = "road";
    public const string Settlement = "settlement";
    public const string Resource = "resource";
    public const string River = "river";
    public const string TerrainArea = "terrain_area";

    public static ImmutableArray<string> All { get; } =
        ImmutableArray.Create(Region, Road, Settlement, Resource, River, TerrainArea);

    public static bool IsKnown(string? type) =>
        type is not null && All.Contains(type, StringComparer.Ordinal);

    public static string DisplayName(string type) => type switch
    {
        Region => "区域面", Road => "道路", Settlement => "城镇",
        Resource => "资源", River => "河流", TerrainArea => "地形区域", _ => type
    };
}

public sealed record MapDatasetDescriptor(string Id, string Type, string Source, string? Name = null);

using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：地图 Manifest 只描述文档身份、坐标入口和两个后续容器。
public sealed record MapManifest(
    string Format,
    string Version,
    string Id,
    string Name,
    MapManifestCoordinateSystem CoordinateSystem,
    ImmutableArray<JsonElement> Datasets,
    ImmutableArray<JsonElement> Assets)
{
    public const string CurrentFormat = "xuanyu-map";
    public const string CurrentVersion = "0.1.0";

    public static MapManifest CreateNew(string id, string name) => new(
        CurrentFormat,
        CurrentVersion,
        id,
        name,
        MapManifestCoordinateSystem.LocalCartesianMeter,
        ImmutableArray<JsonElement>.Empty,
        ImmutableArray<JsonElement>.Empty);

    public static MapManifest FromMap(MapDefinition map) => CreateNew(
        map.MapId.Value,
        map.DisplayName);
}

public sealed record MapManifestCoordinateSystem(string Type, string Unit)
{
    public static MapManifestCoordinateSystem LocalCartesianMeter { get; } =
        new("local_cartesian", "meter");
}

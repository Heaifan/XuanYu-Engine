using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R2-C1：地图 Manifest 携带强类型 Dataset Descriptor。
public sealed record MapManifest(
    string Format,
    string Version,
    string Id,
    string Name,
    MapManifestCoordinateSystem CoordinateSystem,
    ImmutableArray<MapDatasetDescriptor> Datasets,
    ImmutableArray<DatasetLayerState> DatasetLayerStates,
    ImmutableArray<System.Text.Json.JsonElement> Assets)
{
    public const string CurrentFormat = "xuanyu-map";
    public const string CurrentVersion = "0.1.0";

    public static MapManifest CreateNew(string id, string name) => new(
        CurrentFormat,
        CurrentVersion,
        id,
        name,
        MapManifestCoordinateSystem.LocalCartesianMeter,
        ImmutableArray<MapDatasetDescriptor>.Empty,
        ImmutableArray<DatasetLayerState>.Empty,
        ImmutableArray<System.Text.Json.JsonElement>.Empty);

    public static MapManifest FromMap(MapDefinition map) => CreateNew(
        map.MapId.Value,
        map.DisplayName);
}

public sealed record MapManifestCoordinateSystem(string Type, string Unit)
{
    public static MapManifestCoordinateSystem LocalCartesianMeter { get; } =
        new("local_cartesian", "meter");
}

using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：领域 Manifest 与 JSON DTO 的唯一映射点。
internal static class MapManifestMapper
{
    public static MapManifestJson ToJson(MapManifest manifest) => new(
        manifest.Format,
        manifest.Version,
        manifest.Id,
        manifest.Name,
        new MapManifestCoordinateSystemJson(
            manifest.CoordinateSystem.Type,
            manifest.CoordinateSystem.Unit),
        manifest.Datasets.Select(dataset => new MapDatasetDescriptorJson(
            dataset.Id, dataset.Type, dataset.Source)).ToArray(),
        manifest.Assets);

    public static MapManifest ToManifest(MapManifestJson json) => new(
        json.Format ?? "",
        json.Version ?? "",
        json.Id ?? "",
        json.Name ?? "",
        new MapManifestCoordinateSystem(
            json.CoordinateSystem?.Type ?? "",
            json.CoordinateSystem?.Unit ?? ""),
        json.Datasets?.Select(dataset => new MapDatasetDescriptor(
            dataset.Id ?? "", dataset.Type ?? "", dataset.Source ?? "")).ToImmutableArray() ?? default,
        json.Assets?.ToImmutableArray() ?? default);
}

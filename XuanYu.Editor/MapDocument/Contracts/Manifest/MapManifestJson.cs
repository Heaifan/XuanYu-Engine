using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：Manifest JSON 键名固定为批准的 snake_case 合同。
internal sealed record MapManifestJson(
    [property: JsonPropertyName("format"), JsonPropertyOrder(0)] string? Format,
    [property: JsonPropertyName("version"), JsonPropertyOrder(1)] string? Version,
    [property: JsonPropertyName("id"), JsonPropertyOrder(2)] string? Id,
    [property: JsonPropertyName("name"), JsonPropertyOrder(3)] string? Name,
    [property: JsonPropertyName("coordinate_system"), JsonPropertyOrder(4)] MapManifestCoordinateSystemJson? CoordinateSystem,
    [property: JsonPropertyName("datasets"), JsonPropertyOrder(5)]
    IReadOnlyList<MapDatasetDescriptorJson>? Datasets,
    [property: JsonPropertyName("dataset_layer_state"), JsonPropertyOrder(6)] IReadOnlyList<DatasetLayerStateJson>? DatasetLayerStates,
    [property: JsonPropertyName("assets"), JsonPropertyOrder(7)]
    IReadOnlyList<System.Text.Json.JsonElement>? Assets);

internal sealed record DatasetLayerStateJson(string? DatasetId, bool IsVisible, bool IsLocked, int Order);

internal sealed record MapDatasetDescriptorJson(
    [property: JsonPropertyName("id"), JsonPropertyOrder(0)] string? Id,
    [property: JsonPropertyName("name"), JsonPropertyOrder(1)] string? Name,
    [property: JsonPropertyName("type"), JsonPropertyOrder(2)] string? Type,
    [property: JsonPropertyName("source"), JsonPropertyOrder(3)] string? Source);

internal sealed record MapManifestCoordinateSystemJson(
    [property: JsonPropertyName("type"), JsonPropertyOrder(0)] string? Type,
    [property: JsonPropertyName("unit"), JsonPropertyOrder(1)] string? Unit);

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

internal sealed record MapDatasetDocumentJson(
    [property: JsonPropertyName("format"), JsonPropertyOrder(0)] string? Format,
    [property: JsonPropertyName("version"), JsonPropertyOrder(1)] string? Version,
    [property: JsonPropertyName("id"), JsonPropertyOrder(2)] string? Id,
    [property: JsonPropertyName("type"), JsonPropertyOrder(3)] string? Type,
    [property: JsonPropertyName("features"), JsonPropertyOrder(4)] IReadOnlyList<JsonElement>? Features);

internal static class MapDatasetDocumentMapper
{
    public static MapDatasetDocumentJson ToJson(MapDatasetDocument document) => new(
        document.Format, document.Version, document.Id, document.Type, document.Features);

    public static MapDatasetDocument ToDocument(MapDatasetDocumentJson json) => new(
        json.Format ?? "",
        json.Version ?? "",
        json.Id ?? "",
        json.Type ?? "",
        json.Features?.ToImmutableArray() ?? default);
}

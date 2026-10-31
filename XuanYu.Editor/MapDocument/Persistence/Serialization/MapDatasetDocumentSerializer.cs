using System.Text.Json;
using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetDocumentSerializer
{
    static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static MapDocumentResult<MapDatasetDocument> Deserialize(string json)
    {
        try
        {
            var parsed = JsonSerializer.Deserialize<MapDatasetDocumentJson>(json, ReadOptions);
            return parsed is null
                ? MapDocumentResult<MapDatasetDocument>.Fail("BrokenJson", "Dataset JSON 为空。", "Parse")
                : MapDocumentResult<MapDatasetDocument>.Ok(MapDatasetDocumentMapper.ToDocument(parsed));
        }
        catch (JsonException ex)
        {
            return MapDocumentResult<MapDatasetDocument>.Fail(
                "BrokenJson", "Dataset JSON 损坏或格式不严格。", "Parse", ex.Message);
        }
    }

    public static string Serialize(MapDatasetDocument document) =>
        JsonSerializer.Serialize(MapDatasetDocumentMapper.ToJson(document), WriteOptions);
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：Manifest 严格 JSON 序列化，拒绝未知字段并保持确定性输出。
public static class MapManifestSerializer
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

    public static MapDocumentResult<MapManifest> Deserialize(string json)
    {
        try
        {
            var parsed = JsonSerializer.Deserialize<MapManifestJson>(json, ReadOptions);
            return parsed is null
                ? MapDocumentResult<MapManifest>.Fail("BrokenJson", "地图 Manifest JSON 为空。", "Parse")
                : MapDocumentResult<MapManifest>.Ok(MapManifestMapper.ToManifest(parsed));
        }
        catch (JsonException ex)
        {
            return MapDocumentResult<MapManifest>.Fail(
                "BrokenJson", "地图 Manifest JSON 损坏或格式不严格。", "Parse", ex.Message);
        }
    }

    public static string Serialize(MapManifest manifest) =>
        JsonSerializer.Serialize(MapManifestMapper.ToJson(manifest), WriteOptions);
}

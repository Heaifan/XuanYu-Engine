using System.Text.Json;
using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：.xymap 严格 JSON 读写。字段大小写敏感、未知字段拒绝、确定性输出、UTF-8。
public static class MapJsonSerializer
{
    static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static MapDocumentResult<MapDocument> Deserialize(string json)
    {
        try
        {
            var parsed = JsonSerializer.Deserialize<MapDocumentJson>(json, ReadOptions);
            if (parsed is null)
                return MapDocumentResult<MapDocument>.Fail("BrokenJson", "地图JSON结构损坏。", "Parse");
            return MapDocumentResult<MapDocument>.Ok(MapJsonMapper.ToDocument(parsed));
        }
        catch (JsonException ex)
        {
            return MapDocumentResult<MapDocument>.Fail("BrokenJson", "地图JSON损坏或格式不严格。", "Parse", ex.Message);
        }
    }

    public static string Serialize(MapDocument document) =>
        JsonSerializer.Serialize(MapJsonMapper.ToJson(document), WriteOptions);
}

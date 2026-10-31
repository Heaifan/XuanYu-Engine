using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapMarkerDatasetCodec
{
    public static JsonElement Write(MapMarker marker) => JsonSerializer.SerializeToElement(new
    {
        id = marker.MarkerId.Value,
        geometry = new { type = "point", position = new { x = marker.Position.X, y = marker.Position.Y } },
        properties = new { name = marker.DisplayName }
    });

    public static MapDocumentResult<MapMarkerDatasetFeature> Read(JsonElement feature)
    {
        if (feature.ValueKind != JsonValueKind.Object || !Exact(feature, "id", "geometry", "properties")) return Fail("InvalidFeature", "Marker Feature 字段不合法。");
        if (!feature.TryGetProperty("id", out var id) || !MapMarkerId.TryParse(id.GetString(), out var markerId)) return Fail("InvalidMarkerId", "Marker Feature ID 非法。");
        var geometry = feature.GetProperty("geometry");
        if (geometry.ValueKind != JsonValueKind.Object || !Exact(geometry, "type", "position") || geometry.GetProperty("type").GetString() != "point") return Fail("InvalidGeometry", "Marker geometry 必须是 point。");
        var point = geometry.GetProperty("position");
        if (point.ValueKind != JsonValueKind.Object || !Exact(point, "x", "y") || !point.GetProperty("x").TryGetDouble(out var x) || !point.GetProperty("y").TryGetDouble(out var y) || !Finite(x) || !Finite(y)) return Fail("InvalidPoint", "Marker position 必须是有限 x/y 数值。");
        var properties = feature.GetProperty("properties");
        if (properties.ValueKind != JsonValueKind.Object || !Exact(properties, "name") || properties.GetProperty("name").ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(properties.GetProperty("name").GetString())) return Fail("InvalidProperties", "Marker properties 不合法。");
        return MapDocumentResult<MapMarkerDatasetFeature>.Ok(new(markerId, new(x, y), properties.GetProperty("name").GetString()!));
    }

    public static MapDocumentResult<MapDatasetDocument> Validate(string type, ImmutableArray<JsonElement> features)
    {
        if (type != MapDatasetTypes.Marker && !features.IsEmpty) return FailDocument("InvalidFeatures", "非 Marker Dataset 的 features 必须为空数组。");
        var ids = new HashSet<MapMarkerId>();
        foreach (var feature in features) { var read = Read(feature); if (!read.Succeeded || read.Value is null) return FailDocument(read.ErrorCode, read.Message); if (!ids.Add(read.Value.MarkerId)) return FailDocument("DuplicateFeatureId", "Marker Feature ID 不得重复。"); }
        return MapDocumentResult<MapDatasetDocument>.Ok(null!);
    }
    static bool Exact(JsonElement item, params string[] names) => item.EnumerateObject().Select(p => p.Name).Order().SequenceEqual(names.Order());
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    static MapDocumentResult<MapMarkerDatasetFeature> Fail(string code, string message) => MapDocumentResult<MapMarkerDatasetFeature>.Fail(code, message, "Validate");
    static MapDocumentResult<MapDatasetDocument> FailDocument(string code, string message) => MapDocumentResult<MapDatasetDocument>.Fail(code, message, "Validate");
}

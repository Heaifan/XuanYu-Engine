using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapRoadDatasetCodec
{
    public static JsonElement Write(MapRoad road) => JsonSerializer.SerializeToElement(new
    {
        id = road.RoadId.Value,
        geometry = new { type = "polyline", points = road.Points.Select(point => new { x = point.X, y = point.Y }) },
        properties = new { name = road.DisplayName, kind = road.Kind }
    });

    public static MapDocumentResult<MapDatasetDocument> Validate(string type, ImmutableArray<JsonElement> features)
    {
        if (type != MapDatasetTypes.Road && !features.IsEmpty)
            return FailDocument("InvalidFeatures", "非 Road Dataset 的 features 必须为空数组。");
        var ids = new HashSet<MapRoadId>();
        foreach (var raw in features)
        {
            var read = Read(raw);
            if (!read.Succeeded || read.Value is null) return FailDocument("InvalidFeatures", read.Message);
            if (!ids.Add(read.Value.RoadId)) return FailDocument("DuplicateFeatureId", "Road Feature ID 不得重复。");
        }
        return MapDocumentResult<MapDatasetDocument>.Ok(null!);
    }

    public static MapDocumentResult<MapRoadDatasetFeature> Read(JsonElement feature)
    {
        if (feature.ValueKind != JsonValueKind.Object || !Exact(feature, "id", "geometry", "properties")) return Fail("InvalidFeature", "Road Feature 字段不合法。");
        if (!feature.TryGetProperty("id", out var id) || !MapRoadId.TryParse(id.GetString(), out var roadId)) return Fail("InvalidRoadId", "Road Feature ID 非法。");
        if (!feature.TryGetProperty("geometry", out var geometry) || geometry.ValueKind != JsonValueKind.Object ||
            !Exact(geometry, "type", "points") || geometry.GetProperty("type").GetString() != "polyline") return Fail("InvalidGeometry", "Road geometry 必须是 polyline。");
        if (!geometry.TryGetProperty("points", out var points) || points.ValueKind != JsonValueKind.Array) return Fail("InvalidPoints", "Road points 必须是数组。");
        var result = ImmutableArray.CreateBuilder<MapPoint>();
        foreach (var point in points.EnumerateArray())
        {
            if (point.ValueKind != JsonValueKind.Object || !Exact(point, "x", "y") || !point.TryGetProperty("x", out var x) || !point.TryGetProperty("y", out var y) || !x.TryGetDouble(out var px) || !y.TryGetDouble(out var py) || !Finite(px) || !Finite(py)) return Fail("InvalidPoint", "Road point 必须是有限 x/y 数值。");
            if (result.Count > 0 && result[^1] == new MapPoint(px, py)) return Fail("AdjacentDuplicatePoint", "Road 相邻节点不得重复。");
            result.Add(new MapPoint(px, py));
        }
        if (result.Count < 2 || result.Count > MapRoadValidator.MaxPoints) return Fail("InvalidPoints", "Road points 数量必须为 2～1024。");
        if (!feature.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Object || !Exact(properties, "name", "kind") || !properties.TryGetProperty("name", out var name) || name.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(name.GetString()) || !properties.TryGetProperty("kind", out var kind) || kind.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(kind.GetString())) return Fail("InvalidProperties", "Road properties 不合法。");
        return MapDocumentResult<MapRoadDatasetFeature>.Ok(new(roadId, result.ToImmutable(), name.GetString()!, kind.GetString()!));
    }
    static bool Exact(JsonElement item, params string[] names) => item.EnumerateObject().Select(property => property.Name).Order().SequenceEqual(names.Order());
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    static MapDocumentResult<MapDatasetDocument> FailDocument(string code, string message) => MapDocumentResult<MapDatasetDocument>.Fail(code, message, "Validate");
    static MapDocumentResult<MapRoadDatasetFeature> Fail(string code, string message) => MapDocumentResult<MapRoadDatasetFeature>.Fail(code, message, "Validate");
}

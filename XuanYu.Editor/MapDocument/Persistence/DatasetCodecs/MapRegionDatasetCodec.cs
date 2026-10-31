using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapRegionDatasetCodec
{
    public static JsonElement Write(MapRegion region) => JsonSerializer.SerializeToElement(new
    {
        id = region.RegionId.Value,
        geometry = new { type = "polygon", points = region.Vertices.Select(point => new { x = point.X, y = point.Y }) },
        properties = new { name = region.DisplayName, kind = region.Kind.ToString().ToLowerInvariant() }
    });

    public static MapDocumentResult<MapDatasetDocument> Validate(string type, ImmutableArray<JsonElement> features)
    {
        if (type != MapDatasetTypes.Region && !features.IsEmpty)
            return FailDocument("InvalidFeatures", "非 Region Dataset 的 features 必须为空数组。");
        var ids = new HashSet<MapRegionId>();
        foreach (var feature in features)
        {
            var read = Read(feature);
            if (!read.Succeeded || read.Value is null) return FailDocument(read.ErrorCode, read.Message);
            if (!ids.Add(read.Value.RegionId)) return FailDocument("DuplicateFeatureId", "Region Feature ID 不得重复。");
        }
        return MapDocumentResult<MapDatasetDocument>.Ok(null!);
    }

    public static MapDocumentResult<MapRegionDatasetFeature> Read(JsonElement feature)
    {
        if (feature.ValueKind != JsonValueKind.Object || !Exact(feature, "id", "geometry", "properties"))
            return Fail<MapRegionDatasetFeature>("InvalidFeature", "Region Feature 字段不合法。");
        if (!feature.TryGetProperty("id", out var id) || id.ValueKind != JsonValueKind.String ||
            !MapRegionId.TryParse(id.GetString(), out var regionId))
            return Fail<MapRegionDatasetFeature>("InvalidRegionId", "Region Feature ID 非法。");
        if (!feature.TryGetProperty("geometry", out var geometry) || geometry.ValueKind != JsonValueKind.Object ||
            !Exact(geometry, "type", "points") || geometry.GetProperty("type").GetString() != "polygon")
            return Fail<MapRegionDatasetFeature>("InvalidGeometry", "Region geometry 必须是 polygon。");
        if (!geometry.TryGetProperty("points", out var points) || points.ValueKind != JsonValueKind.Array)
            return Fail<MapRegionDatasetFeature>("InvalidPoints", "Region points 必须是数组。");
        var result = ImmutableArray.CreateBuilder<MapPoint>();
        foreach (var point in points.EnumerateArray())
        {
            if (point.ValueKind != JsonValueKind.Object || !Exact(point, "x", "y") ||
                !point.TryGetProperty("x", out var x) || !point.TryGetProperty("y", out var y) ||
                !x.TryGetDouble(out var px) || !y.TryGetDouble(out var py) || !Finite(px) || !Finite(py))
                return Fail<MapRegionDatasetFeature>("InvalidPoint", "Region point 必须是有限 x/y 数值。");
            result.Add(new MapPoint(px, py));
        }
        if (result.Count < 3 || result[0] == result[^1])
            return Fail<MapRegionDatasetFeature>("InvalidPoints", "Region points 至少三个且不得重复首尾点。");
        if (!feature.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Object ||
            !Exact(properties, "name", "kind") || !properties.TryGetProperty("name", out var name) ||
            name.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(name.GetString()) ||
            !properties.TryGetProperty("kind", out var kind) || kind.ValueKind != JsonValueKind.String ||
            !Enum.TryParse<MapRegionKind>(kind.GetString(), true, out var regionKind))
            return Fail<MapRegionDatasetFeature>("InvalidProperties", "Region properties 不合法。");
        return MapDocumentResult<MapRegionDatasetFeature>.Ok(new(regionId, result.ToImmutable(), name.GetString()!, regionKind));
    }

    static bool Exact(JsonElement item, params string[] names) =>
        item.EnumerateObject().Select(property => property.Name).Order().SequenceEqual(names.Order());
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    static MapDocumentResult<MapDatasetDocument> FailDocument(string code, string message) =>
        MapDocumentResult<MapDatasetDocument>.Fail(code, message, "Validate");
    static MapDocumentResult<T> Fail<T>(string code, string message) => MapDocumentResult<T>.Fail(code, message, "Validate");
}

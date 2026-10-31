using System.Text.Json.Serialization;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：.xymap v1 严格 JSON 模型。
// JsonPropertyName 固定 camelCase 键名（优先于命名策略），JsonPropertyOrder 保证确定性输出。
// 读取严格大小写：键名与 JsonPropertyName 不一致即视为未知字段并拒绝。
internal sealed record MapDocumentJson(
    [property: JsonPropertyName("schemaVersion"), JsonPropertyOrder(0)] int SchemaVersion,
    [property: JsonPropertyName("mapId"), JsonPropertyOrder(1)] string MapId,
    [property: JsonPropertyName("name"), JsonPropertyOrder(2)] string Name,
    [property: JsonPropertyName("sizeMeters"), JsonPropertyOrder(3)] MapSizeJson SizeMeters,
    [property: JsonPropertyName("coordinateSystem"), JsonPropertyOrder(4)] MapCoordinateSystemJson CoordinateSystem,
    [property: JsonPropertyName("surface"), JsonPropertyOrder(5)] MapSurfaceJson Surface,
    [property: JsonPropertyName("environment"), JsonPropertyOrder(6)] MapEnvironmentJson Environment,
    [property: JsonPropertyName("layerReferences"), JsonPropertyOrder(7)] IReadOnlyList<string> LayerReferences);

internal sealed record MapSizeJson(
    [property: JsonPropertyName("width"), JsonPropertyOrder(0)] double Width,
    [property: JsonPropertyName("depth"), JsonPropertyOrder(1)] double Depth);

internal sealed record MapCoordinateSystemJson(
    [property: JsonPropertyName("unit"), JsonPropertyOrder(0)] string Unit,
    [property: JsonPropertyName("upAxis"), JsonPropertyOrder(1)] string UpAxis,
    [property: JsonPropertyName("origin"), JsonPropertyOrder(2)] MapVector3Json Origin);

internal sealed record MapVector3Json(
    [property: JsonPropertyName("x"), JsonPropertyOrder(0)] double X,
    [property: JsonPropertyName("y"), JsonPropertyOrder(1)] double Y,
    [property: JsonPropertyName("z"), JsonPropertyOrder(2)] double Z);

internal sealed record MapSurfaceJson(
    [property: JsonPropertyName("kind"), JsonPropertyOrder(0)] string Kind,
    [property: JsonPropertyName("baseHeightMeters"), JsonPropertyOrder(1)] double BaseHeightMeters,
    [property: JsonPropertyName("amplitudeMeters"), JsonPropertyOrder(2)] double AmplitudeMeters,
    [property: JsonPropertyName("wavelengthMeters"), JsonPropertyOrder(3)] double WavelengthMeters,
    [property: JsonPropertyName("seed"), JsonPropertyOrder(4)] int Seed);

internal sealed record MapEnvironmentJson(
    [property: JsonPropertyName("skyPreset"), JsonPropertyOrder(0)] string SkyPreset,
    [property: JsonPropertyName("sunDirection"), JsonPropertyOrder(1)] MapVector3Json SunDirection,
    [property: JsonPropertyName("sunIntensity"), JsonPropertyOrder(2)] double SunIntensity,
    [property: JsonPropertyName("ambientIntensity"), JsonPropertyOrder(3)] double AmbientIntensity);

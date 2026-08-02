using System.Text.Json.Serialization;

namespace XuanYu.Editor.SceneDocument;

sealed record SceneDocumentJson(
    [property: JsonPropertyOrder(0)] string Format,
    [property: JsonPropertyOrder(1)] int SchemaVersion,
    [property: JsonPropertyOrder(2)] SceneInfoJson Scene,
    [property: JsonPropertyOrder(3)] IReadOnlyList<SceneEntityJson> Entities,
    [property: JsonPropertyOrder(4)] IReadOnlyList<SceneAssetJson>? Assets = null);

sealed record SceneInfoJson(
    [property: JsonPropertyOrder(0)] string Id,
    [property: JsonPropertyOrder(1)] string Name);

sealed record SceneEntityJson(
    [property: JsonPropertyOrder(0)] int Id,
    [property: JsonPropertyOrder(1)] string Name,
    [property: JsonPropertyOrder(2)] string? EntityType,
    [property: JsonPropertyOrder(3)] int? ParentId,
    [property: JsonPropertyOrder(4)] int SiblingOrder,
    [property: JsonPropertyOrder(5)] Vector3Json Position,
    [property: JsonPropertyOrder(6)] Vector3Json Rotation,
    [property: JsonPropertyOrder(7)] Vector3Json Scale,
    [property: JsonPropertyOrder(8)] string? ModelAssetId = null);

sealed record SceneAssetJson(
    [property: JsonPropertyOrder(0)] string AssetId,
    [property: JsonPropertyOrder(1)] string Kind,
    [property: JsonPropertyOrder(2)] string RelativePath,
    [property: JsonPropertyOrder(3)] string DisplayName,
    [property: JsonPropertyOrder(4)] int ImporterVersion);

sealed record Vector3Json(
    [property: JsonPropertyOrder(0)] double X,
    [property: JsonPropertyOrder(1)] double Y,
    [property: JsonPropertyOrder(2)] double Z);

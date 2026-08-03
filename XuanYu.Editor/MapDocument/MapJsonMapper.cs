using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：MapDocument ↔ MapDocumentJson 双向映射。
internal static class MapJsonMapper
{
    public static MapDocumentJson ToJson(MapDocument doc) =>
        new(
            doc.SchemaVersion,
            doc.MapId.Value,
            doc.Name,
            ToJson(doc.SizeMeters),
            ToJson(doc.CoordinateSystem),
            ToJson(doc.Surface),
            ToJson(doc.Environment),
            doc.LayerReferences);

    public static MapDocument ToDocument(MapDocumentJson json) =>
        new(
            json.SchemaVersion,
            MapId.TryParse(json.MapId, out var id) ? id : default,
            json.Name ?? "",
            new MapSize(json.SizeMeters?.Width ?? 0.0, json.SizeMeters?.Depth ?? 0.0),
            new MapCoordinateSystem(
                json.CoordinateSystem?.Unit ?? "",
                json.CoordinateSystem?.UpAxis ?? "",
                ToVector(json.CoordinateSystem?.Origin)),
            new MapSurfaceDefinition(
                json.Surface?.Kind ?? "",
                json.Surface?.BaseHeightMeters ?? 0.0,
                json.Surface?.AmplitudeMeters ?? 0.0,
                json.Surface?.WavelengthMeters ?? 0.0,
                json.Surface?.Seed ?? 0),
            new MapEnvironmentDefinition(
                json.Environment?.SkyPreset ?? "",
                ToVector(json.Environment?.SunDirection),
                json.Environment?.SunIntensity ?? 0.0,
                json.Environment?.AmbientIntensity ?? 0.0),
            json.LayerReferences?.ToImmutableArray() ?? default);

    static MapSizeJson ToJson(MapSize size) => new(size.Width, size.Depth);

    static MapCoordinateSystemJson ToJson(MapCoordinateSystem coord) =>
        new(coord.Unit, coord.UpAxis, ToJson(coord.Origin));

    static MapSurfaceJson ToJson(MapSurfaceDefinition surface) =>
        new(surface.Kind, surface.BaseHeightMeters, surface.AmplitudeMeters,
            surface.WavelengthMeters, surface.Seed);

    static MapEnvironmentJson ToJson(MapEnvironmentDefinition env) =>
        new(env.SkyPreset, ToJson(env.SunDirection), env.SunIntensity, env.AmbientIntensity);

    static MapVector3Json ToJson(MapVector3 v) => new(v.X, v.Y, v.Z);

    static MapVector3 ToVector(MapVector3Json? v) =>
        v is null ? new MapVector3(0, 0, 0) : new MapVector3(v.X, v.Y, v.Z);
}

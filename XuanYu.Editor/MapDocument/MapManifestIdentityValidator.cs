using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapManifestIdentityValidator
{
    public static MapDocumentResult<bool> Validate(MapId mapId, MapManifest? manifest)
    {
        if (!mapId.IsValid)
            return Fail("MissingMapIdentity", "当前地图 MapId 缺失或非法。", "mapId");
        if (manifest is null)
            return Fail("MissingManifestIdentity", "当前地图 Manifest 缺失。", "manifest");
        if (string.IsNullOrWhiteSpace(manifest.Id))
            return Fail("MissingManifestIdentity", "当前地图 Manifest.Id 缺失。", "manifest.id");
        if (!MapId.TryParse(manifest.Id, out var manifestId))
            return Fail("InvalidManifestIdentity", "当前地图 Manifest.Id 非法。", "manifest.id");
        return string.Equals(mapId.Value, manifestId.Value, StringComparison.OrdinalIgnoreCase)
            ? MapDocumentResult<bool>.Ok(true)
            : Fail("ManifestIdentityMismatch", "当前地图 MapId 与 Manifest.Id 不一致。", "identity");
    }

    static MapDocumentResult<bool> Fail(string code, string message, string detail) =>
        MapDocumentResult<bool>.Fail(code, message, "Identity", detail);
}

using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapManifestIdentityGuardTests
{
    [Fact]
    public void Null_manifest_id_is_blocked_without_throwing()
    {
        var manifest = MapManifest.CreateNew(MapId.New().Value, "地图") with { Id = null! };

        var result = MapManifestValidator.Validate(manifest);

        Assert.False(result.Succeeded);
        Assert.Equal("InvalidId", result.ErrorCode);
    }

    [Fact]
    public void Empty_manifest_id_is_blocked()
    {
        var mapId = MapId.New();
        var result = MapManifestIdentityValidator.Validate(mapId, MapManifest.CreateNew("", "地图"));

        Assert.False(result.Succeeded);
        Assert.Equal("MissingManifestIdentity", result.ErrorCode);
    }

    [Fact]
    public void Mismatched_manifest_id_is_blocked()
    {
        var result = MapManifestIdentityValidator.Validate(
            MapId.New(), MapManifest.CreateNew(MapId.New().Value, "地图"));

        Assert.False(result.Succeeded);
        Assert.Equal("ManifestIdentityMismatch", result.ErrorCode);
    }

    [Fact]
    public void Matching_manifest_id_is_accepted()
    {
        var mapId = MapId.New();

        var result = MapManifestIdentityValidator.Validate(
            mapId, MapManifest.CreateNew(mapId.Value, "地图"));

        Assert.True(result.Succeeded);
    }
}

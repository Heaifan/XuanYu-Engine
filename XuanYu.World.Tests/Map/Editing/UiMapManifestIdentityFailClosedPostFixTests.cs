using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapManifestIdentityFailClosedPostFixTests
{
    [Fact]
    public async Task Null_and_empty_manifest_ids_reject_without_world_commit()
    {
        foreach (var id in new string?[] { null, "" })
        {
            var (vm, root, _) = await RegionIdentityPostFixFixture.CreateAsync();
            try
            {
                var path = Path.Combine(root, $"bad-{id ?? "null"}.json");
                var manifest = MapManifest.CreateNew(vm.CurrentMapManifest.Id, "地图") with { Id = id! };
                await File.WriteAllTextAsync(path, MapManifestSerializer.Serialize(manifest));
                var before = vm.MapSession.CurrentMap;
                Assert.False(await vm.OpenMapManifestAsync(path));
                Assert.Equal(before, vm.MapSession.CurrentMap);
                Assert.False(string.IsNullOrWhiteSpace(vm.FooterMessage));
            }
            finally { RegionIdentityPostFixFixture.Cleanup(root); }
        }
    }

    [Fact]
    public void Mismatched_and_missing_identity_fail_closed_with_diagnostics()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var before = vm.MapSession.CurrentMap;
        var mapId = vm.MapSession.CurrentMap.MapId;
        var mismatch = MapManifestIdentityValidator.Validate(
            mapId, MapManifest.CreateNew(MapId.New().Value, "地图"));
        Assert.False(mismatch.Succeeded);
        Assert.Equal("ManifestIdentityMismatch", mismatch.ErrorCode);
        Assert.False(string.IsNullOrWhiteSpace(mismatch.Message));
        Assert.Equal(before, vm.MapSession.CurrentMap);

        var missing = MapManifestIdentityValidator.Validate(mapId, null);
        Assert.False(missing.Succeeded);
        Assert.Equal("MissingManifestIdentity", missing.ErrorCode);
        Assert.False(string.IsNullOrWhiteSpace(missing.Message));
        Assert.Equal(before, vm.MapSession.CurrentMap);
    }

    [Fact]
    public void Malformed_identity_returns_failure_instead_of_throwing()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var before = vm.MapSession.CurrentMap;
        var manifest = MapManifest.CreateNew(MapId.New().Value, "地图") with { Id = null! };
        var validation = MapManifestValidator.Validate(manifest);
        Assert.False(validation.Succeeded);
        Assert.Equal("InvalidId", validation.ErrorCode);
        Assert.False(string.IsNullOrWhiteSpace(validation.Message));
        Assert.Equal(before, vm.MapSession.CurrentMap);
    }
}

using XuanYu.Editor.MapDocument;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map;

// WP4 evidence candidate, grounded in MAP-A map-contract.md §.xyscene mapReference
// and MAP-DOC-A-R1-plan.md: map.json Manifest and .xymap are separate contracts.
public sealed class Wp4SceneMapPersistenceContractTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), "xye-wp4-scene-map-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task Scene_save_does_not_reference_a_manifest_path()
    {
        Directory.CreateDirectory(_root);
        var manifestPath = Path.Combine(_root, "map.json");
        var scenePath = Path.Combine(_root, "scene.xyscene");
        var source = new UiVm(null, () => true);

        Assert.True(await source.SaveMapManifestAsync(manifestPath));
        Assert.True(await source.SaveSceneAsync(scenePath));
        var savedScene = await new SceneStorageService().LoadAsync(scenePath);
        Assert.True(savedScene.Succeeded);
        Assert.Null(savedScene.Value!.MapReference);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.Null(reopened.MapReferenceError);
    }

    [Fact]
    public async Task Scene_reference_rejects_a_file_whose_map_id_changed()
    {
        Directory.CreateDirectory(_root);
        var mapPath = Path.Combine(_root, "map.xymap");
        var scenePath = Path.Combine(_root, "scene-id-mismatch.xyscene");
        var storage = new MapStorageService();
        var original = MapDocument.CreateNew("Original", 2000, 2000);
        Assert.True((await storage.SaveAsync(mapPath, original)).Succeeded);

        var source = new UiVm(null, () => true);
        var loaded = await storage.LoadAsync(mapPath);
        Assert.NotNull(loaded.Value);
        Assert.True(source.MapSession.ReplaceCurrentMap(
            MapDocumentAggregateBridge.ToAggregate(loaded.Value!), markSaved: true, mapPath).IsSuccess);
        Assert.True(await source.SaveSceneAsync(scenePath));

        var replacement = MapDocument.CreateNew("Replacement", 2000, 2000);
        Assert.NotEqual(original.MapId, replacement.MapId);
        Assert.True((await storage.SaveAsync(mapPath, replacement)).Succeeded);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.NotNull(reopened.MapReferenceError);
        Assert.NotEqual(replacement.MapId.ToString(), reopened.MapIdText);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

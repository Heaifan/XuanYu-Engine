using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetRegistryF1FailureTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-f1-fail-{Guid.NewGuid():N}");

    [Fact]
    public async Task F1_A08_dataset_write_failure_keeps_manifest_and_registry_unchanged()
    {
        Directory.CreateDirectory(_root);
        var mapPath = Path.Combine(_root, "map.json");
        var manifest = MapManifest.CreateNew("map", "地图");
        Assert.True((await new MapManifestStorageService().SaveAsync(mapPath, manifest)).Succeeded);
        await File.WriteAllTextAsync(Path.Combine(_root, "data"), "not-a-directory");
        var registry = new MapDatasetRegistry(mapPath, manifest);
        var result = await registry.CreateAsync("111", "road");
        var disk = await new MapManifestStorageService().LoadAsync(mapPath);
        Assert.False(result.Succeeded);
        Assert.Empty(registry.CurrentManifest.Datasets);
        Assert.Empty(disk.Value!.Datasets);
        Assert.False(File.Exists(Path.Combine(_root, "data", "111.json")));
    }

    [Fact]
    public async Task F1_A09_manifest_commit_failure_removes_orphan_dataset()
    {
        Directory.CreateDirectory(_root);
        var mapPath = Path.Combine(_root, "map.json");
        Directory.CreateDirectory(mapPath);
        var registry = new MapDatasetRegistry(mapPath, MapManifest.CreateNew("map", "地图"));
        var result = await registry.CreateAsync("111", "road");
        Assert.False(result.Succeeded);
        Assert.Empty(registry.CurrentManifest.Datasets);
        Assert.False(File.Exists(Path.Combine(_root, "data", "111.json")));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}

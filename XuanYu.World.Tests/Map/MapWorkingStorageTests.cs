using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapWorkingStorageTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-working-{Guid.NewGuid():N}");

    async Task<(MapWorkingStorage Storage, MapDatasetRegistry Registry)> WorkingRegistryAsync()
    {
        var storage = new MapWorkingStorage(_root);
        var manifest = MapManifest.CreateNew("map-a", "未命名场景");
        var working = await storage.EnsureAsync(manifest);
        Assert.True(working.Succeeded);
        return (storage, new MapDatasetRegistry(working.Value!, manifest));
    }

    [Fact]
    public async Task Ensure_is_lazy_and_keeps_working_path_separate_from_formal_path()
    {
        var storage = new MapWorkingStorage(_root);
        Assert.False(storage.HasWorkspace);
        var result = await storage.EnsureAsync(MapManifest.CreateNew("map-a", "未命名场景"));
        Assert.True(result.Succeeded);
        Assert.True(File.Exists(result.Value));
        Assert.StartsWith(Path.GetFullPath(_root), result.Value!, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(result.Value, storage.WorkingManifestPath);
    }

    [Fact]
    public async Task Promotion_copies_registered_datasets_and_discards_working_root()
    {
        var (storage, registry) = await WorkingRegistryAsync();
        Assert.True((await registry.CreateAsync("road-a", "road")).Succeeded);
        Assert.True((await registry.CreateAsync("river-c", "river")).Succeeded);
        var formal = Path.Combine(_root, "formal", "map.json");
        Assert.True((await storage.PromoteAsync(formal, registry.CurrentManifest)).Succeeded);
        var loaded = await new MapManifestStorageService().LoadAsync(formal);
        Assert.Equal(2, loaded.Value!.Datasets.Length);
        Assert.All(loaded.Value.Datasets, item => Assert.True(File.Exists(Path.Combine(
            Path.GetDirectoryName(formal)!, item.Source.Replace('/', Path.DirectorySeparatorChar)))));
        Assert.False(storage.HasWorkspace);
    }

    [Fact]
    public async Task Promotion_skips_unregistered_orphans_and_fails_on_target_collision()
    {
        var (storage, registry) = await WorkingRegistryAsync();
        Assert.True((await registry.CreateAsync("road-a", "road")).Succeeded);
        Assert.True((await registry.CreateAsync("region-b", "region")).Succeeded);
        Assert.True((await registry.UnregisterAsync("region-b")).Succeeded);
        var formal = Path.Combine(_root, "formal", "map.json");
        Assert.True((await storage.PromoteAsync(formal, registry.CurrentManifest)).Succeeded);
        Assert.True(File.Exists(Path.Combine(_root, "formal", "data", "road-a.json")));
        Assert.False(File.Exists(Path.Combine(_root, "formal", "data", "region-b.json")));

        var (secondStorage, secondRegistry) = await WorkingRegistryAsync();
        Assert.True((await secondRegistry.CreateAsync("road-a", "road")).Succeeded);
        Assert.False((await secondStorage.PromoteAsync(formal, secondRegistry.CurrentManifest)).Succeeded);
        Assert.True(secondStorage.HasWorkspace);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}

using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetRegistryLifecycleTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-registry-{Guid.NewGuid():N}");
    readonly string _mapPath;
    readonly MapDatasetRegistry _registry;

    public MapDatasetRegistryLifecycleTests()
    {
        Directory.CreateDirectory(_root);
        _mapPath = Path.Combine(_root, "map.json");
        _registry = new MapDatasetRegistry(_mapPath, MapManifest.CreateNew("south-china", "华南"));
    }

    [Fact]
    public async Task Create_resolve_enumerate_and_find_by_id_are_connected()
    {
        var created = await _registry.CreateAsync("roads", "road");
        var entries = await _registry.EnumerateAsync();
        var found = await _registry.FindByIdAsync("ROADS");
        Assert.True(created.Succeeded);
        Assert.Single(entries);
        Assert.Equal(MapDatasetStatus.Normal, entries[0].Status);
        Assert.Equal(MapDatasetStatus.Normal, found!.Status);
    }

    [Fact]
    public async Task Same_type_can_have_multiple_registered_datasets()
    {
        Assert.True((await _registry.CreateAsync("roads-a", "road")).Succeeded);
        Assert.True((await _registry.CreateAsync("roads-b", "road")).Succeeded);
        Assert.Equal(2, _registry.CurrentManifest.Datasets.Length);
    }

    [Fact]
    public async Task Register_accepts_an_existing_valid_dataset_file()
    {
        var descriptor = new MapDatasetDescriptor("rivers", "river", "data/rivers.json");
        var path = Path.Combine(_root, "data", "rivers.json");
        var saved = await new MapDatasetStorageService().SaveAsync(
            path, MapDatasetDocument.CreateNew(descriptor));
        var registered = await _registry.RegisterAsync(descriptor);
        Assert.True(saved.Succeeded);
        Assert.True(registered.Succeeded);
        Assert.Equal("rivers", _registry.CurrentManifest.Datasets[0].Id);
    }

    [Fact]
    public async Task Unregister_removes_only_the_manifest_registration()
    {
        await _registry.CreateAsync("roads", "road");
        var path = Path.Combine(_root, "data", "roads.json");
        var result = await _registry.UnregisterAsync("ROADS");
        Assert.True(result.Succeeded);
        Assert.Empty(_registry.CurrentManifest.Datasets);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Missing_and_invalid_files_are_isolated_per_entry()
    {
        await _registry.CreateAsync("roads", "road");
        var path = Path.Combine(_root, "data", "roads.json");
        File.Delete(path);
        Assert.Equal(MapDatasetStatus.Missing, (await _registry.EnumerateAsync())[0].Status);
        await File.WriteAllTextAsync(path, "broken");
        Assert.Equal(MapDatasetStatus.Invalid, (await _registry.EnumerateAsync())[0].Status);
    }

    [Fact]
    public async Task Duplicate_id_and_source_collision_leave_manifest_unchanged()
    {
        await _registry.CreateAsync("roads", "road");
        var duplicate = await _registry.CreateAsync("ROADS", "road");
        var collision = await _registry.CreateAsync("roads", "road");
        Assert.False(duplicate.Succeeded);
        Assert.False(collision.Succeeded);
        Assert.Single(_registry.CurrentManifest.Datasets);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}

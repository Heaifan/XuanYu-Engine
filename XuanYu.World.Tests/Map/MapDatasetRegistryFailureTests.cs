using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetRegistryFailureTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-registry-fail-{Guid.NewGuid():N}");

    [Fact]
    public async Task Invalid_register_does_not_write_a_manifest_entry()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var registry = new MapDatasetRegistry(path, MapManifest.CreateNew("map", "地图"));
        var descriptor = new MapDatasetDescriptor("roads", "road", "data/roads.json");
        var result = await registry.RegisterAsync(descriptor);
        Assert.False(result.Succeeded);
        Assert.Empty(registry.CurrentManifest.Datasets);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task Invalid_input_is_rejected_before_any_cross_file_write()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var registry = new MapDatasetRegistry(path, MapManifest.CreateNew("map", "地图"));
        var result = await registry.CreateAsync("../roads", "road");
        Assert.False(result.Succeeded);
        Assert.Empty(registry.CurrentManifest.Datasets);
        Assert.False(File.Exists(path));
        Assert.False(Directory.Exists(Path.Combine(_root, "data")));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}

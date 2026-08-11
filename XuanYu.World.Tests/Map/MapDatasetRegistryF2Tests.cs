using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetRegistryF2Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-f2-reg-{Guid.NewGuid():N}");

    async Task<MapDatasetRegistry> SavedRegistryAsync()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var manifest = MapManifest.CreateNew("map", "地图");
        Assert.True((await new MapManifestStorageService().SaveAsync(path, manifest)).Succeeded);
        return new MapDatasetRegistry(path, manifest);
    }

    [Fact]
    public async Task F2_auto_id_uses_internal_type_and_keeps_json_internal()
    {
        var registry = await SavedRegistryAsync();
        var result = await registry.CreateAutoAsync("road", () => "a31f82");
        Assert.True(result.Succeeded);
        Assert.Equal("road-a31f82", result.Value!.Id);
        var json = await File.ReadAllTextAsync(Path.Combine(_root, "data", "road-a31f82.json"));
        Assert.Contains("road", json);
        Assert.DoesNotContain("道路", json);
    }

    [Fact]
    public async Task F2_source_and_registry_collisions_retry_without_overwrite()
    {
        var registry = await SavedRegistryAsync();
        Assert.True((await registry.CreateAutoAsync("road", () => "aaaaaa")).Succeeded);
        var suffixes = new Queue<string>(["aaaaaa", "bbbbbb"]);
        var result = await registry.CreateAutoAsync("road", suffixes.Dequeue);
        Assert.True(result.Succeeded);
        Assert.Equal("road-bbbbbb", result.Value!.Id);
        Assert.Equal(2, registry.CurrentManifest.Datasets.Length);
    }

    [Fact]
    public async Task F2_source_collision_is_fail_closed()
    {
        var registry = await SavedRegistryAsync();
        var path = Path.Combine(_root, "data", "road-c0ffee.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, "keep");
        var result = await registry.CreateAutoAsync("road", () => "c0ffee");
        Assert.False(result.Succeeded);
        Assert.Equal("keep", await File.ReadAllTextAsync(path));
        Assert.Empty(registry.CurrentManifest.Datasets);
    }

    [Fact]
    public void F2_id_generation_has_a_finite_retry_bound()
    {
        var result = MapDatasetIdGenerator.Generate("road", _ => true, () => "deadbe");
        Assert.False(result.Succeeded);
        Assert.Equal(MapDatasetIdGenerator.MaxAttempts, 16);
        Assert.Equal("DatasetIdGenerationExhausted", result.ErrorCode);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}

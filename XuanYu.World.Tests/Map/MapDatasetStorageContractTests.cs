using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetStorageContractTests : IDisposable
{
    readonly string _directory = Path.Combine(Path.GetTempPath(), $"xuanyu-dataset-{Guid.NewGuid():N}");
    readonly MapDatasetStorageService _storage = new();
    readonly MapDatasetDescriptor _descriptor = new("roads", "road", "data/roads.json");

    public MapDatasetStorageContractTests() => Directory.CreateDirectory(_directory);

    [Fact]
    public async Task Missing_file_is_an_isolated_missing_status()
    {
        var result = await _storage.LoadAsync(Path.Combine(_directory, "missing.json"), _descriptor);
        Assert.Equal(MapDatasetStatus.Missing, result.Status);
    }

    [Fact]
    public async Task Save_then_load_returns_normal_and_matches_descriptor()
    {
        var path = Path.Combine(_directory, "roads.json");
        var saved = await _storage.SaveAsync(path, MapDatasetDocument.CreateNew(_descriptor));
        var loaded = await _storage.LoadAsync(path, _descriptor);
        Assert.True(saved.Succeeded);
        Assert.Equal(MapDatasetStatus.Normal, loaded.Status);
        Assert.Equal("roads", loaded.Document!.Id);
    }

    [Fact]
    public async Task Broken_json_is_an_invalid_status_without_throwing()
    {
        var path = Path.Combine(_directory, "roads.json");
        await File.WriteAllTextAsync(path, "not-json");
        var result = await _storage.LoadAsync(path, _descriptor);
        Assert.Equal(MapDatasetStatus.Invalid, result.Status);
        Assert.Equal("BrokenJson", result.ErrorCode);
    }

    [Fact]
    public async Task Descriptor_identity_mismatch_is_invalid()
    {
        var path = Path.Combine(_directory, "roads.json");
        await _storage.SaveAsync(path, MapDatasetDocument.CreateNew(_descriptor) with { Id = "rivers", Type = "river" });
        var result = await _storage.LoadAsync(path, _descriptor);
        Assert.Equal(MapDatasetStatus.Invalid, result.Status);
        Assert.Equal("DescriptorMismatch", result.ErrorCode);
    }

    [Fact]
    public async Task Invalid_document_does_not_replace_existing_file()
    {
        var path = Path.Combine(_directory, "roads.json");
        await _storage.SaveAsync(path, MapDatasetDocument.CreateNew(_descriptor));
        var failed = await _storage.SaveAsync(path, MapDatasetDocument.CreateNew(_descriptor) with
        {
            Features = default
        });
        var loaded = await _storage.LoadAsync(path, _descriptor);
        Assert.False(failed.Succeeded);
        Assert.Equal(MapDatasetStatus.Normal, loaded.Status);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
        catch (IOException) { }
    }
}

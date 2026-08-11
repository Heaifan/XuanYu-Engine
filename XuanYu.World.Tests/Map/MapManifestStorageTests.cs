using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-DOC-A-R1：文件 Create/Read/Save/Failure Safety 合同。
public sealed class MapManifestStorageTests : IDisposable
{
    readonly string _directory = Path.Combine(Path.GetTempPath(), "xy-map-manifest-r1-" + Guid.NewGuid().ToString("N"));
    readonly MapManifestStorageService _storage = new();

    public MapManifestStorageTests() => Directory.CreateDirectory(_directory);

    string PathFor(string name = "map") => Path.Combine(_directory, name, "map.json");
    static MapManifest Valid() => MapManifest.CreateNew("south-china", "华南");

    [Fact]
    public async Task Save_then_read_round_trips_and_creates_only_file()
    {
        var path = PathFor();
        var saved = await _storage.SaveAsync(path, Valid());
        var loaded = await _storage.LoadAsync(path);

        Assert.True(saved.Succeeded);
        Assert.True(loaded.Succeeded);
        Assert.Equal("south-china", loaded.Value!.Id);
        Assert.Equal("华南", loaded.Value.Name);
        Assert.True(File.Exists(path));
        Assert.False(Directory.Exists(Path.Combine(_directory, "map", "data")));
        Assert.False(Directory.Exists(Path.Combine(_directory, "map", "assets")));
    }

    [Fact]
    public async Task Broken_json_is_rejected_without_a_current_owner_change()
    {
        var path = PathFor();
        await _storage.SaveAsync(path, Valid());
        await File.WriteAllTextAsync(path, "{broken");
        var owner = new MapManifestOwner();
        owner.Load("old/map.json", Valid());

        var result = await _storage.LoadAsync(path);

        Assert.False(result.Succeeded);
        Assert.Equal("BrokenJson", result.ErrorCode);
        Assert.Equal("south-china", owner.CurrentManifest!.Id);
        Assert.Equal("old/map.json", owner.CurrentPath);
    }

    [Fact]
    public async Task Invalid_manifest_does_not_create_target_or_replace_existing_file()
    {
        var path = PathFor("invalid");
        var invalid = Valid() with { Id = "not valid" };

        var result = await _storage.SaveAsync(path, invalid);

        Assert.False(result.Succeeded);
        Assert.Equal("InvalidId", result.ErrorCode);
        Assert.False(File.Exists(path));
        Assert.False(Directory.Exists(Path.GetDirectoryName(path)!));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

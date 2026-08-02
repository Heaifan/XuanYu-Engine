using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：候选加载 / 原子保存（真实文件，临时目录）。
public sealed class MapStorageTests : IDisposable
{
    readonly string _dir = Path.Combine(Path.GetTempPath(), "xy-map-d2-" + Guid.NewGuid().ToString("N"));
    readonly MapStorageService _storage = new();

    public MapStorageTests() => Directory.CreateDirectory(_dir);

    string NewPath(string name = "TestBattlefield") =>
        Path.Combine(_dir, name, "map.xymap");

    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield", 2000, 2000);

    [Fact]
    public async Task First_save_then_load_round_trips()
    {
        var path = NewPath();
        var doc = Valid();
        var saved = await _storage.SaveAsync(path, doc);
        Assert.True(saved.Succeeded);

        var loaded = await _storage.LoadAsync(path);
        Assert.True(loaded.Succeeded);
        Assert.Equal(doc.MapId, loaded.Value!.MapId);
        Assert.Equal("TestBattlefield", loaded.Value.Name);
        Assert.Equal(2000.0, loaded.Value.SizeMeters.Width);
        Assert.Equal(2000.0, loaded.Value.SizeMeters.Depth);
    }

    [Fact]
    public async Task Overwrite_save_succeeds()
    {
        var path = NewPath();
        await _storage.SaveAsync(path, Valid());
        var second = Valid() with { Name = "Renamed" };
        var result = await _storage.SaveAsync(path, second);
        Assert.True(result.Succeeded);
        var loaded = await _storage.LoadAsync(path);
        Assert.Equal("Renamed", loaded.Value!.Name);
    }

    [Fact]
    public async Task Save_creates_missing_directories()
    {
        var path = NewPath("Nested/Folder/TestBattlefield");
        var result = await _storage.SaveAsync(path, Valid());
        Assert.True(result.Succeeded);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Load_missing_file_fails_with_code()
    {
        var result = await _storage.LoadAsync(Path.Combine(_dir, "nope", "map.xymap"));
        Assert.False(result.Succeeded);
        Assert.Equal("MissingFile", result.ErrorCode);
    }

    [Fact]
    public async Task Load_success_returns_clean_candidate()
    {
        var path = NewPath();
        await _storage.SaveAsync(path, Valid());
        var loaded = await _storage.LoadAsync(path);
        Assert.True(loaded.Succeeded);
        Assert.NotNull(loaded.Value);
    }

    [Fact]
    public async Task No_temp_files_remain_after_successful_save()
    {
        var path = NewPath();
        await _storage.SaveAsync(path, Valid());
        Assert.Empty(Directory.GetFiles(Path.GetDirectoryName(path)!,
            ".*.tmp", SearchOption.TopDirectoryOnly));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

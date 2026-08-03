using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：加载失败保护 / 非法合同拒绝 / 保存失败不写坏文件。
public sealed class MapStorageFailureTests : IDisposable
{
    readonly string _dir = Path.Combine(Path.GetTempPath(), "xy-map-d2f-" + Guid.NewGuid().ToString("N"));
    readonly MapStorageService _storage = new();

    public MapStorageFailureTests() => Directory.CreateDirectory(_dir);

    string NewPath() => Path.Combine(_dir, "TestBattlefield", "map.xymap");

    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield", 2000, 2000);

    [Fact]
    public async Task Corrupted_file_does_not_replace_candidate()
    {
        var path = NewPath();
        await _storage.SaveAsync(path, Valid());
        await File.WriteAllTextAsync(path, "{ not json");

        var loaded = await _storage.LoadAsync(path);
        Assert.False(loaded.Succeeded);
        Assert.Equal("BrokenJson", loaded.ErrorCode);
    }

    [Fact]
    public async Task Invalid_contract_file_fails_validation()
    {
        var path = NewPath();
        await _storage.SaveAsync(path, Valid());
        await File.WriteAllTextAsync(path,
            """{"schemaVersion":1,"mapId":"21e4a2d34d4a4a1eb2539eac76d412a8","name":"x","sizeMeters":{"width":2000,"depth":2000},"coordinateSystem":{"unit":"meter","upAxis":"Y","origin":{"x":0,"y":0,"z":0}},"surface":{"kind":"Flat","baseHeightMeters":0,"amplitudeMeters":12,"wavelengthMeters":400,"seed":1},"environment":{"skyPreset":"ClearDayV1","sunDirection":{"x":-0.35,"y":-0.55,"z":0.75},"sunIntensity":1,"ambientIntensity":0.35},"layerReferences":[]}""");

        var loaded = await _storage.LoadAsync(path);
        Assert.False(loaded.Succeeded);
        Assert.Equal("InvalidCoordinateSystem", loaded.ErrorCode);
    }

    [Fact]
    public async Task Save_rejects_invalid_document_without_writing()
    {
        var path = NewPath();
        var bad = Valid() with { SizeMeters = new MapSize(1, 2000) };
        var result = await _storage.SaveAsync(path, bad);
        Assert.False(result.Succeeded);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task No_temp_files_remain_after_failed_save()
    {
        var path = NewPath();
        var bad = Valid() with { SizeMeters = new MapSize(1, 2000) };
        await _storage.SaveAsync(path, bad);
        // 验证失败发生在写盘之前：目录不应被创建，更无临时文件残留。
        var dir = Path.GetDirectoryName(path)!;
        Assert.False(Directory.Exists(dir), "验证失败不得创建目标目录");
        if (Directory.Exists(dir))
            Assert.Empty(Directory.GetFiles(dir, ".*.tmp", SearchOption.TopDirectoryOnly));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

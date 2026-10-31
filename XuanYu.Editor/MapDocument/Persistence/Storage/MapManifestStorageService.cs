using System.Text;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：map.json 候选加载与同目录临时文件原子保存。
public sealed class MapManifestStorageService
{
    public async Task<MapDocumentResult<MapManifest>> LoadAsync(string path)
    {
        if (!File.Exists(path))
            return Fail<MapManifest>("MissingFile", "地图 Manifest 文件不存在。", "Read", path);
        try
        {
            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);
            var parsed = MapManifestSerializer.Deserialize(json);
            if (!parsed.Succeeded || parsed.Value is null)
                return Fail<MapManifest>(parsed.ErrorCode, parsed.Message, parsed.Stage, parsed.Detail);
            return MapManifestValidator.Validate(parsed.Value);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Fail<MapManifest>("ReadFailed", $"读取地图 Manifest 失败：{ex.Message}", "Read", ex.Message);
        }
    }

    public async Task<MapDocumentResult<string>> SaveAsync(string path, MapManifest manifest)
    {
        var valid = MapManifestValidator.Validate(manifest);
        if (!valid.Succeeded)
            return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory)) directory = Directory.GetCurrentDirectory();
        var temp = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            Directory.CreateDirectory(directory);
            var bytes = Encoding.UTF8.GetBytes(MapManifestSerializer.Serialize(manifest));
            await using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await stream.WriteAsync(bytes);
                await stream.FlushAsync();
                stream.Flush(true);
            }
            File.Move(temp, path, true);
            return MapDocumentResult<string>.Ok(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TryDeleteTemp(temp);
            return MapDocumentResult<string>.Fail("WriteFailed", $"保存地图 Manifest 失败：{ex.Message}", "Write", ex.Message);
        }
    }

    static MapDocumentResult<T> Fail<T>(string code, string message, string stage, string detail) =>
        MapDocumentResult<T>.Fail(code, message, stage, detail);

    static void TryDeleteTemp(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

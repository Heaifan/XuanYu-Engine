using System.Text;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图文件存储。候选加载 + 同目录临时文件原子保存，不直接替换任何状态。
public sealed class MapStorageService
{
    public async Task<MapDocumentResult<MapDocument>> LoadAsync(string path)
    {
        if (!File.Exists(path))
            return MapDocumentResult<MapDocument>.Fail("MissingFile", "地图文件不存在。", "Read", path);
        try
        {
            var text = await File.ReadAllTextAsync(path, Encoding.UTF8);
            var parsed = MapJsonSerializer.Deserialize(text);
            if (!parsed.Succeeded || parsed.Value is null)
                return MapDocumentResult<MapDocument>.Fail(parsed.ErrorCode, parsed.Message, "Parse", parsed.Detail);
            var validated = MapDocumentValidator.Validate(parsed.Value);
            if (!validated.Succeeded || validated.Value is null)
                return MapDocumentResult<MapDocument>.Fail(validated.ErrorCode, validated.Message, "Validate", validated.Detail);
            return MapDocumentResult<MapDocument>.Ok(validated.Value);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return MapDocumentResult<MapDocument>.Fail("ReadFailed", $"读取地图失败：{ex.Message}", "Read", ex.Message);
        }
    }

    public async Task<MapDocumentResult<string>> SaveAsync(string path, MapDocument document)
    {
        var validated = MapDocumentValidator.Validate(document);
        if (!validated.Succeeded || validated.Value is null)
            return MapDocumentResult<string>.Fail(validated.ErrorCode, validated.Message, "Validate", validated.Detail);

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory)) directory = Directory.GetCurrentDirectory();
        var temp = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            Directory.CreateDirectory(directory);
            await File.WriteAllTextAsync(temp, MapJsonSerializer.Serialize(document), Encoding.UTF8);
            File.Move(temp, path, true);
            return MapDocumentResult<string>.Ok(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TryDeleteTemp(temp);
            return MapDocumentResult<string>.Fail("WriteFailed", $"保存地图失败：{ex.Message}", "Write", ex.Message);
        }
    }

    static void TryDeleteTemp(string path)
    {
        // 清理辅助失败不影响已确定的失败结果（best-effort）；不得让清理异常覆盖原始保存异常。
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

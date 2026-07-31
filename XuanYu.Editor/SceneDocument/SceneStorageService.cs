using System.Text.Json;

namespace XuanYu.Editor.SceneDocument;

public sealed class SceneStorageService
{
    static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public async Task<SceneDocumentResult<SceneDocumentSnapshot>> LoadAsync(string path)
    {
        if (!File.Exists(path)) return SceneDocumentResult<SceneDocumentSnapshot>.Fail("MissingFile", "场景文件不存在。");
        try
        {
            var text = await File.ReadAllTextAsync(path);
            var doc = JsonSerializer.Deserialize<SceneDocumentJson>(text, Options);
            var checkedDoc = SceneDocumentValidator.Validate(doc);
            return checkedDoc.Succeeded
                ? SceneDocumentResult<SceneDocumentSnapshot>.Ok(SceneDocumentMapper.ToSnapshot(checkedDoc.Value!))
                : SceneDocumentResult<SceneDocumentSnapshot>.Fail(checkedDoc.ErrorCode, checkedDoc.Message);
        }
        catch (JsonException)
        {
            return SceneDocumentResult<SceneDocumentSnapshot>.Fail("BrokenJson", "场景JSON损坏或格式不严格。");
        }
        catch (IOException ex)
        {
            return SceneDocumentResult<SceneDocumentSnapshot>.Fail("ReadFailed", $"读取场景失败：{ex.Message}");
        }
    }

    public async Task<SceneDocumentResult<string>> SaveAsync(string path, SceneDocumentSnapshot snapshot)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory)) directory = Directory.GetCurrentDirectory();
        var temp = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            Directory.CreateDirectory(directory);
            var json = SceneDocumentMapper.ToJson(snapshot);
            await File.WriteAllTextAsync(temp, JsonSerializer.Serialize(json, Options));
            File.Move(temp, path, true);
            return SceneDocumentResult<string>.Ok(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TryDeleteTemp(temp);
            return SceneDocumentResult<string>.Fail("WriteFailed", $"保存场景失败：{ex.Message}");
        }
    }

    static void TryDeleteTemp(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

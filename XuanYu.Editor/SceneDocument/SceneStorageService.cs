using System.Text.Json;

namespace XuanYu.Editor.SceneDocument;

public sealed class SceneStorageService
{
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<SceneDocumentResult<SceneDocumentSnapshot>> LoadAsync(
        string path,
        Action<string>? stage = null)
    {
        stage?.Invoke("Read");
        if (!File.Exists(path)) return Fail("MissingFile", "场景文件不存在。", "Read", path);
        try
        {
            var text = await File.ReadAllTextAsync(path);
            stage?.Invoke("Parse");
            var doc = JsonSerializer.Deserialize<SceneDocumentJson>(text, Options);
            stage?.Invoke("Schema");
            var checkedDoc = SceneDocumentValidator.Validate(doc);
            stage?.Invoke("Validate");
            return checkedDoc.Succeeded
                ? SceneDocumentResult<SceneDocumentSnapshot>.Ok(SceneDocumentMapper.ToSnapshot(checkedDoc.Value!))
                : Fail(checkedDoc.ErrorCode, checkedDoc.Message, checkedDoc.Stage, checkedDoc.Detail);
        }
        catch (JsonException ex)
        {
            return Fail("BrokenJson", "场景JSON损坏或格式不严格。", "Parse", ex.Message);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Fail("ReadFailed", $"读取场景失败：{ex.Message}", "Read", ex.Message);
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
            return SceneDocumentResult<string>.Fail("WriteFailed", $"保存场景失败：{ex.Message}", "Write", ex.Message);
        }
    }

    static SceneDocumentResult<SceneDocumentSnapshot> Fail(
        string code,
        string message,
        string stage,
        string detail) =>
        SceneDocumentResult<SceneDocumentSnapshot>.Fail(code, message, stage, detail);

    static void TryDeleteTemp(string path)
    {
        // 清理辅助失败不影响已确定的失败结果（best-effort）；不得让清理异常覆盖原始保存异常。
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

using System.Text;

namespace XuanYu.Editor.MapDocument;

public sealed class MapDatasetStorageService
{
    public async Task<MapDatasetLoadResult> LoadAsync(
        string path, MapDatasetDescriptor? expected = null)
    {
        if (!File.Exists(path)) return MapDatasetLoadResult.Missing("Dataset 文件不存在。");
        try
        {
            var parsed = MapDatasetDocumentSerializer.Deserialize(
                await File.ReadAllTextAsync(path, Encoding.UTF8));
            if (!parsed.Succeeded || parsed.Value is null)
                return MapDatasetLoadResult.Invalid(parsed.ErrorCode, parsed.Message);
            var valid = MapDatasetDocumentValidator.Validate(parsed.Value);
            if (!valid.Succeeded || valid.Value is null)
                return MapDatasetLoadResult.Invalid(valid.ErrorCode, valid.Message);
            if (expected is not null && (expected.Id != valid.Value.Id || expected.Type != valid.Value.Type))
                return MapDatasetLoadResult.Invalid("DescriptorMismatch", "Dataset 文件身份与 Manifest Descriptor 不一致。");
            return MapDatasetLoadResult.Normal(valid.Value);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return MapDatasetLoadResult.Invalid("ReadFailed", $"读取 Dataset 失败：{ex.Message}");
        }
    }

    public async Task<MapDocumentResult<string>> SaveAsync(string path, MapDatasetDocument document)
    {
        var valid = MapDatasetDocumentValidator.Validate(document);
        if (!valid.Succeeded)
            return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
        var directory = Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();
        var temp = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            Directory.CreateDirectory(directory);
            var bytes = Encoding.UTF8.GetBytes(MapDatasetDocumentSerializer.Serialize(document));
            await File.WriteAllBytesAsync(temp, bytes);
            File.Move(temp, path, true);
            return MapDocumentResult<string>.Ok(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TryDelete(temp);
            return MapDocumentResult<string>.Fail("WriteFailed", $"保存 Dataset 失败：{ex.Message}", "Write", ex.Message);
        }
    }

    static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

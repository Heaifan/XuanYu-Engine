using System.Text;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    async Task<MapDocumentResult<string>> CommitCreateAsync(
        MapManifest candidate, MapDatasetDescriptor descriptor, string datasetPath)
    {
        var manifestTemp = Path.Combine(MapRoot, $".map.json.{Guid.NewGuid():N}.tmp");
        var datasetTemp = Path.Combine(Path.GetDirectoryName(datasetPath)!,
            $".{Path.GetFileName(datasetPath)}.{Guid.NewGuid():N}.tmp");
        byte[]? originalManifest = null;
        var datasetMoved = false;
        try
        {
            Directory.CreateDirectory(MapRoot);
            if (File.Exists(MapPath)) originalManifest = await File.ReadAllBytesAsync(MapPath);
            await WriteTempAsync(manifestTemp, MapManifestSerializer.Serialize(candidate));
            Directory.CreateDirectory(Path.GetDirectoryName(datasetPath)!);
            await WriteTempAsync(datasetTemp, MapDatasetDocumentSerializer.Serialize(
                MapDatasetDocument.CreateNew(descriptor)));
            File.Move(datasetTemp, datasetPath, false);
            datasetMoved = true;
            File.Move(manifestTemp, MapPath, true);
            return MapDocumentResult<string>.Ok(MapPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TryDelete(manifestTemp);
            TryDelete(datasetTemp);
            if (datasetMoved) TryDelete(datasetPath);
            await RestoreManifestAsync(originalManifest);
            return MapDocumentResult<string>.Fail("TransactionFailed", $"Dataset 注册事务失败：{ex.Message}", "Commit", ex.Message);
        }
    }

    async Task RestoreManifestAsync(byte[]? original)
    {
        if (original is null)
        {
            TryDelete(MapPath);
            return;
        }
        var restore = Path.Combine(MapRoot, $".map.restore.{Guid.NewGuid():N}.tmp");
        try
        {
            await WriteTempAsync(restore, Encoding.UTF8.GetString(original));
            File.Move(restore, MapPath, true);
        }
        catch (IOException) { TryDelete(restore); }
        catch (UnauthorizedAccessException) { TryDelete(restore); }
    }

    static async Task WriteTempAsync(string path, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write,
            FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.WriteThrough);
        await stream.WriteAsync(bytes);
        await stream.FlushAsync();
        stream.Flush(true);
    }

    static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

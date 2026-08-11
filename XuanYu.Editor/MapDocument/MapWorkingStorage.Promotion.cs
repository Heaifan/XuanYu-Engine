namespace XuanYu.Editor.MapDocument;

public sealed partial class MapWorkingStorage
{
    public async Task<MapDocumentResult<string>> PromoteAsync(string formalPath, MapManifest manifest)
    {
        if (WorkingRoot is null) return MapDocumentResult<string>.Fail("MissingWorkspace", "地图工作区不存在。", "Prepare");
        var formalRoot = Path.GetDirectoryName(Path.GetFullPath(formalPath));
        if (string.IsNullOrWhiteSpace(formalRoot)) return MapDocumentResult<string>.Fail("InvalidPath", "正式地图路径无效。", "Prepare");
        var prepared = PrepareCopies(formalRoot, manifest);
        if (!prepared.Succeeded || prepared.Value is null) return Fail(prepared);
        var committed = await CommitAsync(formalPath, prepared.Value, manifest);
        if (committed.Succeeded) Discard();
        return committed;
    }

    MapDocumentResult<List<(string Source, string Target)>> PrepareCopies(string formalRoot, MapManifest manifest)
    {
        var copies = new List<(string Source, string Target)>();
        foreach (var descriptor in manifest.Datasets)
        {
            if (!MapDatasetPathPolicy.TryResolve(WorkingRoot!, descriptor.Source, out var source) ||
                !MapDatasetPathPolicy.TryResolve(formalRoot, descriptor.Source, out var target) ||
                !File.Exists(source) || File.Exists(target))
                return MapDocumentResult<List<(string Source, string Target)>>.Fail(
                    "PromotionValidationFailed", "地图工作区数据集无法提升。", "Prepare", descriptor.Id);
            copies.Add((source, target));
        }
        return MapDocumentResult<List<(string Source, string Target)>>.Ok(copies);
    }

    async Task<MapDocumentResult<string>> CommitAsync(string formalPath,
        List<(string Source, string Target)> copies, MapManifest manifest)
    {
        var created = new List<string>();
        var temps = new List<string>();
        try
        {
            foreach (var (source, target) in copies)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                var temp = $"{target}.{Guid.NewGuid():N}.tmp";
                File.Copy(source, temp, false);
                temps.Add(temp);
            }
            for (var index = 0; index < copies.Count; index++)
            {
                File.Move(temps[index], copies[index].Target, false);
                created.Add(copies[index].Target);
            }
            var saved = await _manifestStorage.SaveAsync(formalPath, manifest);
            if (saved.Succeeded && saved.Value is not null) return saved;
            throw new IOException(saved.Message);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            foreach (var temp in temps) TryDelete(temp);
            foreach (var target in created) TryDelete(target);
            return MapDocumentResult<string>.Fail("PromotionFailed", $"提升地图工作区失败：{ex.Message}", "Commit", ex.Message);
        }
    }

    static MapDocumentResult<string> Fail(MapDocumentResult<List<(string Source, string Target)>> result) =>
        MapDocumentResult<string>.Fail(result.ErrorCode, result.Message, result.Stage, result.Detail);

    static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

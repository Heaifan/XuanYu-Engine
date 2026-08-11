namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R2-F4：未保存地图的内部可写工作区，不改变用户正式保存路径。
public sealed partial class MapWorkingStorage
{
    readonly MapManifestStorageService _manifestStorage = new();
    readonly string _parentRoot;

    public MapWorkingStorage(string? parentRoot = null)
    {
        _parentRoot = parentRoot ?? Path.Combine(Path.GetTempPath(), "XuanYuEngine", "map-working");
    }

    public bool HasWorkspace => WorkingRoot is not null;
    public string? WorkingRoot { get; private set; }
    public string? WorkingManifestPath { get; private set; }

    public async Task<MapDocumentResult<string>> EnsureAsync(MapManifest manifest)
    {
        if (WorkingManifestPath is not null) return MapDocumentResult<string>.Ok(WorkingManifestPath);
        var root = Path.Combine(_parentRoot, Guid.NewGuid().ToString("N"));
        var path = Path.Combine(root, "map.json");
        var saved = await _manifestStorage.SaveAsync(path, manifest);
        if (!saved.Succeeded || saved.Value is null)
        {
            TryDiscard(root);
            return MapDocumentResult<string>.Fail(saved.ErrorCode, saved.Message, saved.Stage, saved.Detail);
        }
        WorkingRoot = root;
        WorkingManifestPath = path;
        return MapDocumentResult<string>.Ok(path);
    }

    public void Discard()
    {
        if (WorkingRoot is not null) TryDiscard(WorkingRoot);
        WorkingRoot = null;
        WorkingManifestPath = null;
    }

    static void TryDiscard(string root)
    {
        try { if (Directory.Exists(root)) Directory.Delete(root, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

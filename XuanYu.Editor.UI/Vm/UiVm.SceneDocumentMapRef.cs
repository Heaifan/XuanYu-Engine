using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

// MAP-A-R1-D5-B：场景与地图引用的双向闭环。
// 保存场景时附加当前地图引用（mapId + 相对场景目录路径）；打开场景后解析并加载地图。
public sealed partial class UiVm
{
    string? _mapReferenceError;

    public string? MapReferenceError => _mapReferenceError;

    // 场景保存：把当前地图引用附加到快照（未加载地图则不附加）。
    SceneDocumentSnapshot WithMapReference(SceneDocumentSnapshot snapshot, string scenePath)
    {
        if (_mapDocument.CurrentMap is not { } map || _mapDocument.CurrentPath is not { } path)
            return snapshot;
        return snapshot with
        {
            MapReference = new MapReference(
                map.MapId.ToString(), ToSceneRelativePath(path, scenePath))
        };
    }

    // 场景打开：解析 mapReference 并加载地图；失败保持场景主体可用，仅标记引用失效。
    async Task ResolveMapReferenceAsync(SceneDocumentSnapshot snapshot, string scenePath)
    {
        _mapReferenceError = null;
        if (snapshot.MapReference is not { } mapRef) return;
        if (_mapDocument.CurrentMap is { } existing && existing.MapId.ToString() == mapRef.MapId)
            return;
        var fullPath = ResolveMapAssetPath(mapRef.AssetPath, scenePath);
        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
        {
            MarkMapReferenceInvalid(mapRef.AssetPath, "文件缺失");
            return;
        }

        var result = await _mapStorage.LoadAsync(fullPath);
        if (!result.Succeeded || result.Value is null)
        {
            MarkMapReferenceInvalid(mapRef.AssetPath, result.Message);
            return;
        }

        _mapDocument.Load(fullPath, result.Value);
        SyncMapToWorld(result.Value);
        FooterMessage = $"场景引用的地图已加载：{result.Value.Name}。";
        RaiseMapDocumentChanged();
    }

    void MarkMapReferenceInvalid(string assetPath, string reason)
    {
        _mapReferenceError = $"地图引用失效：{assetPath}（{reason}）";
        FooterMessage = _mapReferenceError;
        RaiseMapDocumentChanged();
    }

    static string? ResolveMapAssetPath(string relativePath, string scenePath)
    {
        if (!XuanYu.Editor.Assets.SceneAssetPathPolicy.IsSafeRelativePath(relativePath))
            return null;
        var dir = Path.GetDirectoryName(scenePath) ?? "";
        var candidate = Path.GetFullPath(Path.Combine(dir, relativePath));
        return File.Exists(candidate) ? candidate : null;
    }

    static string ToSceneRelativePath(string fullPath, string scenePath)
    {
        var dir = Path.GetDirectoryName(scenePath) ?? "";
        var full = Path.GetFullPath(fullPath);
        return full.StartsWith(Path.GetFullPath(dir), StringComparison.OrdinalIgnoreCase)
            ? Path.GetRelativePath(dir, full).Replace('\\', '/')
            : Path.GetFileName(fullPath);
    }
}

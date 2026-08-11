using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

// MAP-DOC-A-R1：Map Workspace 只投影 Manifest 身份与容器数量，不复制 Editor State。
public sealed partial class UiVm
{
    readonly MapManifestStorageService _mapManifestStorage = new();
    readonly MapManifestOwner _mapManifestOwner = new();

    public MapManifest CurrentMapManifest => _mapManifestOwner.CurrentManifest
        ?? MapManifest.FromMap(MapSession.CurrentMap);

    public string MapManifestIdText => CurrentMapManifest.Id;

    public string MapManifestCoordinateSystemText =>
        $"{CurrentMapManifest.CoordinateSystem.Type} / {CurrentMapManifest.CoordinateSystem.Unit}";

    public int DatasetCount => CurrentMapManifest.Datasets.Length;

    public string DatasetEmptyState => DatasetCount == 0
        ? "当前无数据集"
        : $"当前有 {DatasetCount} 个数据集（只读）";

    void InitializeMapManifest() => _mapManifestOwner.SetBaseline(
        MapManifest.FromMap(MapSession.CurrentMap));

    void ResetMapManifestFromCurrentMap() => _mapManifestOwner.New(
        MapManifest.FromMap(MapSession.CurrentMap));

    public async Task<bool> OpenMapManifestAsync(string path)
    {
        var result = await _mapManifestStorage.LoadAsync(path);
        if (!result.Succeeded || result.Value is null)
        {
            _mapManifestOwner.MarkError(result.Message);
            FooterMessage = result.Message;
            RaiseMapDocumentChanged();
            return false;
        }
        _mapManifestOwner.Load(path, result.Value);
        FooterMessage = "地图 Manifest 已打开。";
        RaiseMapDocumentChanged();
        return true;
    }

    public async Task<bool> SaveMapManifestAsync(string path)
    {
        var result = await _mapManifestStorage.SaveAsync(path, CurrentMapManifest);
        if (!result.Succeeded || result.Value is null)
        {
            _mapManifestOwner.MarkError(result.Message);
            FooterMessage = result.Message;
            RaiseMapDocumentChanged();
            return false;
        }
        _mapManifestOwner.Save(result.Value);
        FooterMessage = "地图 Manifest 已保存。";
        RaiseMapDocumentChanged();
        return true;
    }
}

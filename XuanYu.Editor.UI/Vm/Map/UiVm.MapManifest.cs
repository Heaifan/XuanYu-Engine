using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

// MAP-DOC-A-R1：Map Workspace 只投影 Manifest 身份与容器数量，不复制 Editor State。
public sealed partial class UiVm
{
    readonly MapManifestStorageService _mapManifestStorage = new();
    readonly MapManifestOwner _mapManifestOwner = new();
    readonly MapWorkingStorage _mapWorkingStorage = new();

    public MapManifest CurrentMapManifest => _mapManifestOwner.CurrentManifest
        ?? MapManifest.FromMap(MapSession.CurrentMap);

    public string MapManifestIdText => CurrentMapManifest.Id;

    public string MapManifestCoordinateSystemText =>
        $"{CurrentMapManifest.CoordinateSystem.Type} / {CurrentMapManifest.CoordinateSystem.Unit}";

    public int DatasetCount => DatasetItems.Count;

    public string DatasetEmptyState => DatasetCount == 0
        ? "当前无数据集"
        : $"当前有 {DatasetCount} 个数据集";

    void InitializeMapManifest() => _mapManifestOwner.SetBaseline(
        MapManifest.FromMap(MapSession.CurrentMap));

    void ResetMapManifestFromCurrentMap()
    {
        _mapManifestOwner.New(MapManifest.FromMap(MapSession.CurrentMap));
        ResetDatasetProjection();
    }

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
        var registry = new MapDatasetRegistry(path, result.Value);
        var documents = await registry.LoadRegionDocumentsAsync();
        if (!documents.Succeeded || documents.Value is null) { FooterMessage = documents.Message; return false; }
        var runtime = MapDatasetRegionBinding.Build(MapSession.CurrentMap, result.Value, documents.Value);
        if (!runtime.Succeeded || runtime.Value is null) { FooterMessage = runtime.Message; return false; }
        var replaced = MapSession.ReplaceCurrentMap(runtime.Value, true, path);
        if (!replaced.IsSuccess) { FooterMessage = replaced.Error!.Value.Message; return false; }
        _mapManifestOwner.Load(path, result.Value);
        _datasetRegistry = registry;
        await RefreshDatasetProjectionAsync();
        FooterMessage = "地图 Manifest 已打开。";
        RaiseMapDocumentChanged();
        return true;
    }

    public async Task<bool> SaveMapManifestAsync(string path)
    {
        if (_datasetRegistry is not null)
        {
            var regions = await _datasetRegistry.SaveRegionContentAsync(MapSession.CurrentMap);
            if (!regions.Succeeded) { FooterMessage = regions.Message; return false; }
        }
        var result = _mapWorkingStorage.HasWorkspace
            ? await _mapWorkingStorage.PromoteAsync(path, CurrentMapManifest)
            : await _mapManifestStorage.SaveAsync(path, CurrentMapManifest);
        if (!result.Succeeded || result.Value is null)
        {
            _mapManifestOwner.MarkError(result.Message);
            FooterMessage = result.Message;
            RaiseMapDocumentChanged();
            return false;
        }
        _mapManifestOwner.Save(result.Value);
        _datasetRegistry = new MapDatasetRegistry(result.Value, CurrentMapManifest);
        MapSession.MarkSaved(result.Value);
        await RefreshDatasetProjectionAsync();
        FooterMessage = "地图 Manifest 已保存。";
        RaiseMapDocumentChanged();
        return true;
    }
}

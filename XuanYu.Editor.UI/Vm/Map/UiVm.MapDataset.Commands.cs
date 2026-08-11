using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    async Task<bool> EnsureDatasetRegistryAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentMapManifestPath))
        {
            FooterMessage = "请先保存地图 Manifest，再创建数据集。";
            return false;
        }
        _datasetRegistry ??= new MapDatasetRegistry(CurrentMapManifestPath, CurrentMapManifest);
        return true;
    }

    public async Task<bool> CreateDatasetAsync()
    {
        if (!await EnsureDatasetRegistryAsync()) return false;
        var result = await _datasetRegistry!.CreateAsync(DatasetCreateId, DatasetCreateType);
        if (!result.Succeeded) return DatasetFailed(result.Message);
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        DatasetSelectedId = result.Value!.Id;
        FooterMessage = "数据集已创建并注册。";
        await RefreshDatasetProjectionAsync();
        RaiseMapDocumentChanged();
        return true;
    }

    public async Task<bool> UnregisterDatasetAsync(string? id = null)
    {
        if (_datasetRegistry is null) return DatasetFailed("当前没有已打开的地图 Manifest。");
        var result = await _datasetRegistry.UnregisterAsync(id ?? DatasetSelectedId);
        if (!result.Succeeded) return DatasetFailed(result.Message);
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        FooterMessage = "数据集已解除注册，文件未删除。";
        await RefreshDatasetProjectionAsync();
        RaiseMapDocumentChanged();
        return true;
    }

    bool DatasetFailed(string message)
    {
        FooterMessage = message;
        RaiseMapDocumentChanged();
        return false;
    }
}

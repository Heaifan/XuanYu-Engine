using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    async Task<bool> EnsureDatasetRegistryAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentMapManifestPath))
            return false;
        _datasetRegistry ??= new MapDatasetRegistry(CurrentMapManifestPath, CurrentMapManifest);
        return true;
    }

    public async Task<bool> CreateDatasetAsync()
    {
        if (!await EnsureDatasetRegistryAsync()) return DatasetFailed("请先保存地图 Manifest，再创建数据集。");
        try
        {
            var result = await _datasetRegistry!.CreateAutoAsync(DatasetCreateType);
            if (!result.Succeeded) return DatasetFailed(result.Message);
            _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
            DatasetSelectedId = result.Value!.Id;
            FooterMessage = $"数据集创建成功：{result.Value.Id}（{result.Value.Type}）";
            LogDatasetOutcome(true, "创建", result.Value.Id, result.Value.Type, "");
            await RefreshDatasetProjectionAsync();
            RaiseMapDocumentChanged();
            return true;
        }
        catch (Exception ex)
        {
            return DatasetFailed($"数据集创建失败：{ex.Message}");
        }
    }

    public async Task<bool> UnregisterDatasetAsync()
    {
        if (_datasetRegistry is null) return DatasetFailed("当前没有已打开的地图 Manifest。", false);
        var targetId = DatasetSelectedId;
        if (string.IsNullOrWhiteSpace(targetId)) return DatasetFailed("请先选择要解除注册的数据集。", false);
        var target = targetId!;
        var removedIndex = _datasetItems.ToList().FindIndex(item => item.Id == target);
        var result = await _datasetRegistry.UnregisterAsync(target);
        if (!result.Succeeded) return DatasetFailed(result.Message, false);
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        FooterMessage = "数据集已解除注册，文件未删除。";
        LogDatasetOutcome(true, "解除注册", target, "", "");
        await RefreshDatasetProjectionAsync();
        DatasetSelectedId = NextDatasetId(removedIndex);
        RaiseMapDocumentChanged();
        return true;
    }

    string? NextDatasetId(int removedIndex)
    {
        if (_datasetItems.Count == 0) return null;
        var index = Math.Min(Math.Max(removedIndex, 0), _datasetItems.Count - 1);
        return _datasetItems[index].Id;
    }

    bool DatasetFailed(string message, bool create = true)
    {
        FooterMessage = message;
        LogDatasetOutcome(false, create ? "创建" : "解除注册", DatasetSelectedId ?? "", DatasetCreateType, message);
        RaiseMapDocumentChanged();
        return false;
    }
}

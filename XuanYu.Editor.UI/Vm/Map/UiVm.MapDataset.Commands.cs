using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    async Task<bool> EnsureDatasetRegistryAsync()
    {
        if (_datasetRegistry is not null) return true;
        var path = CurrentMapManifestPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            var working = await _mapWorkingStorage.EnsureAsync(CurrentMapManifest);
            if (!working.Succeeded || working.Value is null) return false;
            path = working.Value;
        }
        _datasetRegistry = new MapDatasetRegistry(path, CurrentMapManifest);
        return true;
    }

    public async Task<bool> CreateDatasetAsync()
    {
        if (!await EnsureDatasetRegistryAsync()) return DatasetFailed("无法初始化地图工作区，数据集未创建。");
        try
        {
            var result = await _datasetRegistry!.CreateAutoAsync(DatasetCreateType);
            if (!result.Succeeded) return DatasetFailed(result.Message);
            _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
            var runtime = MapDatasetRuntimeProjection.Apply(MapSession.CurrentMap, _datasetRegistry.CurrentManifest);
            if (!runtime.Succeeded || runtime.Value is null) return DatasetFailed(runtime.Message);
            var applied = MapSession.ApplyRuntimeLayerProjection(runtime.Value);
            if (!applied.IsSuccess) return DatasetFailed(applied.Error!.Value.Message);
            _authoringModeTargetSync = true;
            try { DatasetSelectedId = result.Value!.Id; }
            finally { _authoringModeTargetSync = false; }
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

    public async Task<bool> UnregisterDatasetAsync(string? requestedId = null)
    {
        if (_datasetRegistry is null) return DatasetFailed("当前没有已打开的地图 Manifest。", false);
        var targetId = requestedId ?? DatasetSelectedId;
        if (string.IsNullOrWhiteSpace(targetId)) return DatasetFailed("请先选择要解除注册的数据集。", false);
        var target = targetId!;
        if (TryGetDatasetIdForLayer(SelectedLayer?.LayerId ?? default, out var selectedLayerDatasetId) &&
            string.Equals(selectedLayerDatasetId, target, StringComparison.OrdinalIgnoreCase))
            ClearLayerSelection();
        var removedIndex = _datasetItems.ToList().FindIndex(item => item.Id == target);
        if (_regionDrawing.IsActive) CancelRegionDrawingFromEscape();
        if (_roadDrawing.IsActive) CancelRoadDrawingFromEscape();
        var result = await _datasetRegistry.UnregisterAsync(target);
        if (!result.Succeeded) return DatasetFailed(result.Message, false);
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        var runtime = MapDatasetRuntimeProjection.Remove(MapSession.CurrentMap, target);
        var applied = MapSession.ApplyRuntimeLayerProjection(runtime);
        if (!applied.IsSuccess) return DatasetFailed(applied.Error!.Value.Message, false);
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

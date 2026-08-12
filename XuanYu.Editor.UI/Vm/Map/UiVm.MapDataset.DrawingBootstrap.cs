using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isRegionDrawingBootstrapBusy;

    public bool IsRegionDrawingBootstrapBusy => _isRegionDrawingBootstrapBusy;
    public bool CanRequestRegionDrawing => IsRegionEditMode && !IsRegionDrawingBootstrapBusy;

    public async Task<bool> BeginRegionDrawingAsync()
    {
        if (!CanRequestRegionDrawing) return false;
        _isRegionDrawingBootstrapBusy = true;
        RaiseRegionDrawingBootstrapBindings();
        try
        {
            var created = RegionDatasetItems.Count == 0;
            var target = await EnsureRegionDrawingTargetAsync();
            if (target is null) return false;
            SetDatasetDrawingTarget(target.Id);
            SelectTool("区域绘制");
            if (created)
                FooterMessage = $"已自动创建区域数据集「{target.Name}」\nID：{target.Id}\n已进入区域绘制。";
            return IsRegionDrawingTool;
        }
        finally
        {
            _isRegionDrawingBootstrapBusy = false;
            RaiseRegionDrawingBootstrapBindings();
        }
    }

    async Task<MapDatasetRow?> EnsureRegionDrawingTargetAsync()
    {
        var selected = SelectedDataset;
        if (selected is not null && selected.Type != "区域")
        {
            if (_datasetItems.All(item => item.Type != "区域")) return await AutoCreateRegionDatasetAsync();
            return RejectRegionDrawing("请先选择一个区域数据集。");
        }

        var regions = _datasetItems.Where(item => item.Type == "区域").ToArray();
        var target = selected ?? regions.FirstOrDefault();
        if (target is null) return await AutoCreateRegionDatasetAsync();
        if (target.IsLocked) return RejectRegionDrawing("当前区域数据集已锁定，无法绘制。请解锁当前数据集。");
        if (target.Status != "正常") return RejectRegionDrawing("当前区域数据集无效，无法绘制。请检查 Dataset 文件。");
        if (_datasetRegistry is not null)
        {
            var entry = await _datasetRegistry.FindByIdAsync(target.Id);
            if (entry is not { Status: MapDatasetStatus.Normal })
            {
                await RefreshDatasetProjectionAsync();
                return RejectRegionDrawing("当前区域数据集无效，无法绘制。请检查 Dataset 文件。");
            }
        }
        if (selected is null) DatasetSelectedId = target.Id;
        return target;
    }

    async Task<MapDatasetRow?> AutoCreateRegionDatasetAsync()
    {
        DatasetCreateType = MapDatasetTypes.Region;
        if (!await CreateDatasetAsync()) return null;
        var created = SelectedDataset;
        if (created is null) return null;
        return created;
    }

    MapDatasetRow? RejectRegionDrawing(string message)
    {
        FooterState = "状态：不可用";
        FooterMessage = message;
        return null;
    }

    void RaiseRegionDrawingBootstrapBindings()
    {
        OnPropertyChanged(nameof(IsRegionDrawingBootstrapBusy));
        OnPropertyChanged(nameof(CanRequestRegionDrawing));
    }
}

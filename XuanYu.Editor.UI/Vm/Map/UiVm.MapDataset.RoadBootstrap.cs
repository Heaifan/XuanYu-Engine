using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isRoadDrawingBootstrapBusy;
    public bool IsRoadDrawingBootstrapBusy => _isRoadDrawingBootstrapBusy;
    public bool CanRequestRoadDrawing => IsRoadEditMode && !_isRoadDrawingBootstrapBusy;
    public bool CanStartRoadDrawing => IsRoadEditMode && SelectedDataset is { Type: "道路", Status: "正常", IsLocked: false };
    public async Task<bool> BeginRoadDrawingAsync()
    {
        if (!CanRequestRoadDrawing) return false;
        _isRoadDrawingBootstrapBusy = true; RaiseRoadBootstrapBindings();
        try
        {
            var target = await EnsureRoadTargetAsync(); if (target is null) return false;
            SetDatasetDrawingTarget(target.Id); SelectTool("道路绘制");
            FooterMessage = $"已进入道路绘制：{target.Name}。"; return IsRoadDrawingTool;
        }
        finally { _isRoadDrawingBootstrapBusy = false; RaiseRoadBootstrapBindings(); }
    }
    async Task<MapDatasetRow?> EnsureRoadTargetAsync()
    {
        var selected = SelectedDataset;
        if (selected is not null && selected.Type != "道路")
            return RoadDatasetItems.Count == 0 ? await AutoCreateRoadDatasetAsync() : RejectRoad("请先选择一个道路数据集。");
        var target = selected ?? RoadDatasetItems.FirstOrDefault();
        if (target is null) return await AutoCreateRoadDatasetAsync();
        if (target.IsLocked) return RejectRoad("当前道路数据集已锁定，无法绘制。");
        if (target.Status != "正常") return RejectRoad("当前道路数据集无效，无法绘制。请检查 Dataset 文件。");
        if (selected is null) DatasetSelectedId = target.Id;
        return target;
    }
    async Task<MapDatasetRow?> AutoCreateRoadDatasetAsync()
    {
        DatasetCreateType = MapDatasetTypes.Road; if (!await CreateDatasetAsync()) return null; return SelectedDataset;
    }
    MapDatasetRow? RejectRoad(string message) { FooterState = "状态：不可用"; FooterMessage = message; return null; }
    void RaiseRoadBootstrapBindings() { OnPropertyChanged(nameof(IsRoadDrawingBootstrapBusy)); OnPropertyChanged(nameof(CanRequestRoadDrawing)); }
}

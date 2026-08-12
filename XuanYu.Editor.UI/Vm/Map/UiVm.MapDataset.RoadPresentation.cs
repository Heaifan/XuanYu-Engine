namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MapDatasetRow? RoadDrawingTarget => SelectedDataset is { Type: "道路" } selected ? selected : RoadDatasetItems.FirstOrDefault();
    public IReadOnlyList<MapDatasetRow> RoadDatasetItems => _datasetItems.Where(item => item.Type == "道路").ToArray();
    public string RoadDrawingTargetName => RoadDrawingTarget?.Name ?? "当前没有道路数据集";
    public string RoadDrawingTargetId => RoadDrawingTarget?.Id ?? "点击“绘制道路”自动创建";
    public string RoadDrawingTargetStatus => RoadDrawingTarget is { } item ? $"道路数据集 · {item.Status}" : "点击“绘制道路”将自动创建";
}

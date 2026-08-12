namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MapDatasetRow? RegionDrawingTarget => SelectedDataset is { Type: "region" } selected
        ? selected : RegionDatasetItems.FirstOrDefault();

    public string RegionDrawingTargetName => RegionDrawingTarget?.Name ?? "当前没有区域数据集";
    public string RegionDrawingTargetId => RegionDrawingTarget?.Id ?? "点击“绘制区域”自动创建";
    public string RegionDrawingTargetStatus => RegionDrawingTarget is { } item
        ? $"区域数据集 · {item.Status}" : "点击“绘制区域”将自动创建";
}

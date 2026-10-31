using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool IsMarkerPlacementTool => IsTool(EditorToolId.MarkerPlacement);
    public bool CanStartMarkerPlacement => IsRegionEditMode && IsMarkerAuthoringMode &&
        SelectedDataset is { Type: MapDatasetTypes.Marker, Status: "正常", IsLocked: false };
    public bool CanRequestMarkerPlacement => IsRegionEditMode && IsMarkerAuthoringMode;
    public string MarkerPlacementTargetName => SelectedDataset?.Type == MapDatasetTypes.Marker ? SelectedDataset.Name : "当前没有地图标记数据集";
    public string MarkerPlacementTargetId => SelectedDataset?.Type == MapDatasetTypes.Marker ? SelectedDataset.Id : "点击“放置地图标记”自动创建";

    public async Task<bool> BeginMarkerPlacementAsync()
    {
        if (!CanRequestMarkerPlacement) return false;
        if (SelectedDataset?.Type != MapDatasetTypes.Marker)
        {
            DatasetCreateType = MapDatasetTypes.Marker;
            if (!await CreateDatasetAsync()) return false;
        }
        if (SelectedDataset is not { Type: MapDatasetTypes.Marker, Status: "正常", IsLocked: false } target) return false;
        SetDatasetDrawingTarget(target.Id); SelectTool("标记放置");
        FooterMessage = $"已进入地图标记放置：{target.Name}。";
        return IsMarkerPlacementTool;
    }
}

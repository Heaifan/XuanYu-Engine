namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InspectorSelectionTitle => SelectedMarker?.DisplayName ?? SelectedDataset?.Name ?? SelectionTitle;
    public string InspectorSelectionSubtitle => IsMarkerInspector ? "MapMarker · 地图点要素" : SelectedDataset is { } item ? item.TypeIdText :
        IsEntityInspector ? InspectorEntitySubtitle : SelectionSubtitle;
    public bool HasInspectorSelection => InspectorIdentity != InspectorObjectKind.Empty;
    public bool IsInspectorEmpty => InspectorIdentity == InspectorObjectKind.Empty;
    public string InspectorSectionTitle => IsMarkerInspector ? "地图标记属性" : HasSelectedDataset ? "数据集属性" : "基础信息";
}

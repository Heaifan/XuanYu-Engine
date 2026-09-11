using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public MapGeometrySelection? SelectedMapGeometry => _selectedMapGeometry;
    public MapMarker? SelectedMarker => _selectedMapGeometry is { Kind: MapGeometryFeatureKind.Marker } selection &&
        MapMarkerId.TryParse(selection.FeatureId, out var id)
            ? MapSession.CurrentMap.Markers.FirstOrDefault(item => item.MarkerId == id) : null;
    public bool IsMarkerInspector => SelectedMarker is not null;
    public bool IsMarkerInspectorReadOnly => SelectedMarker is { } marker &&
        (marker.IsLocked || MapLayerRules.Find(MapSession.CurrentMap.Layers, marker.LayerId)?.IsLocked == true);
    public string MarkerInspectorIdText => SelectedMarker?.MarkerId.Value ?? "";
    public string MarkerInspectorTypeText => SelectedMarker is null ? "" : "MapMarker";
    public string MarkerInspectorDatasetText => MarkerDataset()?.Name ?? "未注册";
    public string MarkerInspectorStatusText => MarkerStatus();
    public double MarkerInspectorPositionX => SelectedMarker?.Position.X ?? 0;
    public double MarkerInspectorPositionY => SelectedMarker?.Position.Y ?? 0;
    public bool IsRegionInspectorPlaceholderVisible => IsRegionEditMode && !IsMarkerInspector &&
        !HasCurrentLayerSelection && _selectedMapGeometry is null;
    public bool IsGenericInspectorVisible => HasInspectorSelection && !IsMarkerInspector &&
        !HasCurrentLayerSelection && !IsEntityInspector && !IsRegionEditMode;
    public bool IsMapWorkspaceInspectorVisible => !IsEntityInspector && !IsMarkerInspector;
    public bool IsLayerInspectorVisible => HasCurrentLayerSelection && !IsMarkerInspector;

    public bool CommitMarkerPosition(double x, double y)
    {
        if (SelectedMarker is not { } marker) { FooterMessage = "地图标记提交失败：未选择有效标记。"; return false; }
        if (!double.IsFinite(x) || !double.IsFinite(y)) { FooterMessage = "地图标记提交失败：请输入有效数字。"; return false; }
        var result = MapSession.EditMarkerPosition(marker.MarkerId, new(x, y));
        FooterMessage = result.IsSuccess ? "地图标记位置已提交。" : result.Error?.Message ?? "地图标记提交失败。";
        if (result.IsSuccess) RaiseMarkerInspectorBindings();
        return result.IsSuccess;
    }

    void RaiseMarkerInspectorBindings()
    {
        OnPropertyChanged(nameof(SelectedMapGeometry)); OnPropertyChanged(nameof(SelectedMarker));
        OnPropertyChanged(nameof(IsMarkerInspector)); OnPropertyChanged(nameof(IsMarkerInspectorReadOnly));
        OnPropertyChanged(nameof(MarkerInspectorIdText)); OnPropertyChanged(nameof(MarkerInspectorTypeText));
        OnPropertyChanged(nameof(MarkerInspectorDatasetText)); OnPropertyChanged(nameof(MarkerInspectorStatusText));
        OnPropertyChanged(nameof(MarkerInspectorPositionX)); OnPropertyChanged(nameof(MarkerInspectorPositionY));
        OnPropertyChanged(nameof(IsRegionInspectorPlaceholderVisible)); OnPropertyChanged(nameof(IsGenericInspectorVisible));
        OnPropertyChanged(nameof(IsMapWorkspaceInspectorVisible)); OnPropertyChanged(nameof(IsLayerInspectorVisible));
        OnPropertyChanged(nameof(InspectorSelectionTitle)); OnPropertyChanged(nameof(InspectorSelectionSubtitle));
        OnPropertyChanged(nameof(HasInspectorSelection)); OnPropertyChanged(nameof(IsInspectorEmpty));
        OnPropertyChanged(nameof(InspectorSectionTitle)); OnPropertyChanged(nameof(InspectorFields));
    }

    MapDatasetRow? MarkerDataset() => SelectedMarker is { } marker &&
        TryGetDatasetIdForLayer(marker.LayerId, out var id)
            ? _datasetItems.FirstOrDefault(item => item.Id == id) : null;
    string MarkerStatus() => SelectedMarker is not { } marker ? "" :
        !marker.IsVisible ? "隐藏" : marker.IsLocked || MapLayerRules.Find(MapSession.CurrentMap.Layers, marker.LayerId)?.IsLocked == true
            ? "锁定" : MarkerDataset()?.Status ?? "未注册";
}

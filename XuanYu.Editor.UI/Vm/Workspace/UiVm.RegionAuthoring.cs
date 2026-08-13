using System.Windows.Input;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _authoringModeTargetSync;

    public RegionAuthoringMode CurrentRegionAuthoringMode { get; private set; } = RegionAuthoringMode.RegionSurface;
    public bool IsRegionSurfaceAuthoringMode => CurrentRegionAuthoringMode == RegionAuthoringMode.RegionSurface;
    public bool IsRoadAuthoringMode => CurrentRegionAuthoringMode == RegionAuthoringMode.Road;
    public bool IsMarkerAuthoringMode => CurrentRegionAuthoringMode == RegionAuthoringMode.Marker;
    public ICommand SelectRegionAuthoringModeCommand { get; private set; } = null!;

    void SelectRegionAuthoringModeCommandTarget(object? value) => SelectRegionAuthoringMode(value?.ToString());

    public void SelectRegionAuthoringMode(string? displayName)
    {
        if (!IsRegionEditMode || !TryParseRegionAuthoringMode(displayName, out var mode)) return;
        ApplyRegionAuthoringMode(mode);
    }

    void SyncRegionAuthoringModeFromDatasetSelection()
    {
        if (!IsRegionWorkspace) return;
        ApplyRegionAuthoringMode(SelectedDataset?.Type == MapDatasetTypes.Road ? RegionAuthoringMode.Road
            : SelectedDataset?.Type == MapDatasetTypes.Marker ? RegionAuthoringMode.Marker : RegionAuthoringMode.RegionSurface);
    }

    void ApplyRegionAuthoringMode(RegionAuthoringMode mode)
    {
        if (CurrentRegionAuthoringMode == mode) return;
        CancelRegionDrawingFromEscape();
        CancelRoadDrawingFromEscape();
        ClearMapGeometrySelection();
        if (IsEditMode) SelectTool("选择", logTool: false);
        CurrentRegionAuthoringMode = mode;
        OnPropertyChanged(nameof(CurrentRegionAuthoringMode));
        OnPropertyChanged(nameof(IsRegionSurfaceAuthoringMode));
        OnPropertyChanged(nameof(IsRoadAuthoringMode));
        OnPropertyChanged(nameof(IsMarkerAuthoringMode));
        OnPropertyChanged(nameof(CanRequestRegionDrawing));
        OnPropertyChanged(nameof(CanRequestRoadDrawing));
        OnPropertyChanged(nameof(CanStartRegionDrawing));
        OnPropertyChanged(nameof(CanStartRoadDrawing)); OnPropertyChanged(nameof(CanStartMarkerPlacement));
        SelectAuthoringTarget(mode);
    }

    void SelectAuthoringTarget(RegionAuthoringMode mode)
    {
        var type = mode == RegionAuthoringMode.Road ? MapDatasetTypes.Road : mode == RegionAuthoringMode.Marker ? MapDatasetTypes.Marker : MapDatasetTypes.Region;
        if (SelectedDataset?.Type == type) return;
        var target = _datasetItems.FirstOrDefault(item => item.Type == type && item.Status == "正常");
        if (target?.Id == DatasetSelectedId) return;
        _authoringModeTargetSync = true;
        try { DatasetSelectedId = target?.Id; }
        finally { _authoringModeTargetSync = false; }
    }

    static bool TryParseRegionAuthoringMode(string? value, out RegionAuthoringMode mode)
    {
        mode = value switch
        {
            "区域面" => RegionAuthoringMode.RegionSurface,
            "道路" => RegionAuthoringMode.Road,
            "地图标记" => RegionAuthoringMode.Marker,
            _ => default
        };
        return value is "区域面" or "道路" or "地图标记";
    }
}

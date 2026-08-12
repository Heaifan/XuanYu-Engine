using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed record MapDatasetRow(string Name, string Type, string Id, string Status, string Source, bool IsSelected = false,
    bool IsVisible = true, bool IsLocked = false, int Order = 0)
{
    public string VisibilityActionText => IsVisible ? "隐藏" : "显示";
    public string LockActionText => IsLocked ? "解锁" : "锁定";
    public string TypeIdText => $"{Type} · {Id}";
}

public sealed partial class UiVm
{
    IReadOnlyList<MapDatasetRow> _datasetItems = [];
    MapDatasetRegistry? _datasetRegistry;
    string _datasetCreateType = MapDatasetTypes.Region;

    public IReadOnlyList<MapDatasetTypeOption> DatasetTypeOptions => MapDatasetTypePresentation.Options;
    public IReadOnlyList<MapDatasetRow> DatasetItems => _datasetItems;
    public IReadOnlyList<MapDatasetRow> RegionDatasetItems => _datasetItems.Where(item => item.Type == "区域").ToArray();
    public bool IsDatasetEmpty => _datasetItems.Count == 0;
    public string DatasetCreateType
    {
        get => _datasetCreateType;
        set
        {
            if (!MapDatasetTypes.IsKnown(value) || !Set(ref _datasetCreateType, value)) return;
            OnPropertyChanged(nameof(DatasetCreateTypeOption));
        }
    }

    public MapDatasetTypeOption DatasetCreateTypeOption
    {
        get => DatasetTypeOptions.First(option => option.Value == DatasetCreateType);
        set { if (value is not null) DatasetCreateType = value.Value; }
    }

    async Task RefreshDatasetProjectionAsync()
    {
        var entries = _datasetRegistry is null
            ? []
            : await _datasetRegistry.EnumerateAsync();
        _datasetItems = entries.Select(entry => new MapDatasetRow(
            entry.Descriptor.Name ?? MapDatasetTypePresentation.Display(entry.Descriptor.Type),
            MapDatasetTypePresentation.Display(entry.Descriptor.Type), entry.Descriptor.Id,
            StatusText(entry.Status), entry.Descriptor.Source,
            string.Equals(entry.Descriptor.Id, _datasetSelectedId, StringComparison.OrdinalIgnoreCase),
            State(entry.Descriptor.Id).IsVisible, State(entry.Descriptor.Id).IsLocked, State(entry.Descriptor.Id).Order))
            .OrderBy(item => item.Order).ToArray();
        if (SelectedDataset is null) _datasetSelectedId = null;
        DatasetNameText = SelectedDataset?.Name ?? "";
        NotifyDatasetProjection();
    }

    DatasetLayerState State(string id) => _datasetRegistry!.CurrentManifest.DatasetLayerStates
        .First(item => string.Equals(item.DatasetId, id, StringComparison.OrdinalIgnoreCase));

    void ResetDatasetProjection()
    {
        _datasetRegistry = null;
        _datasetItems = [];
        DatasetSelectedId = null;
        NotifyDatasetProjection();
    }

    void NotifyDatasetProjection()
    {
        OnPropertyChanged(nameof(DatasetItems));
        OnPropertyChanged(nameof(RegionDatasetItems));
        OnPropertyChanged(nameof(RegionDrawingTargetName));
        OnPropertyChanged(nameof(RegionDrawingTargetId));
        OnPropertyChanged(nameof(RegionDrawingTargetStatus));
        OnPropertyChanged(nameof(DatasetLayerItems));
        OnPropertyChanged(nameof(IsDatasetEmpty));
        OnPropertyChanged(nameof(DatasetCount));
        OnPropertyChanged(nameof(DatasetEmptyState));
        OnPropertyChanged(nameof(SelectedDataset));
        OnPropertyChanged(nameof(HasSelectedDataset));
        OnPropertyChanged(nameof(CanStartRegionDrawing));
        OnPropertyChanged(nameof(CanDeleteLayer));
        OnPropertyChanged(nameof(CanUnregisterDataset));
        OnPropertyChanged(nameof(DatasetLayerEmptyState));
        OnPropertyChanged(nameof(InspectorSelectionTitle));
        OnPropertyChanged(nameof(InspectorSelectionSubtitle));
        OnPropertyChanged(nameof(HasInspectorSelection));
        OnPropertyChanged(nameof(IsInspectorEmpty));
        OnPropertyChanged(nameof(InspectorSectionTitle));
        OnPropertyChanged(nameof(InspectorFields));
    }

    static string StatusText(MapDatasetStatus status) => status switch
    {
        MapDatasetStatus.Normal => "正常",
        MapDatasetStatus.Missing => "缺失",
        _ => "无效"
    };
}

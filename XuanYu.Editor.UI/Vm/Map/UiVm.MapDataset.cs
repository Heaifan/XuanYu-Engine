using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed record MapDatasetRow(string Type, string Id, string Status, string Source);

public sealed partial class UiVm
{
    IReadOnlyList<MapDatasetRow> _datasetItems = [];
    MapDatasetRegistry? _datasetRegistry;
    string _datasetCreateType = MapDatasetTypes.Region;
    string _datasetSelectedId = "";

    public IReadOnlyList<MapDatasetTypeOption> DatasetTypeOptions => MapDatasetTypePresentation.Options;
    public IReadOnlyList<MapDatasetRow> DatasetItems => _datasetItems;
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

    public string DatasetSelectedId { get => _datasetSelectedId; set => Set(ref _datasetSelectedId, value); }

    async Task RefreshDatasetProjectionAsync()
    {
        var entries = _datasetRegistry is null
            ? []
            : await _datasetRegistry.EnumerateAsync();
        _datasetItems = entries.Select(entry => new MapDatasetRow(
            MapDatasetTypePresentation.Display(entry.Descriptor.Type), entry.Descriptor.Id,
            StatusText(entry.Status), entry.Descriptor.Source)).ToArray();
        NotifyDatasetProjection();
    }

    void ResetDatasetProjection()
    {
        _datasetRegistry = null;
        _datasetItems = [];
        NotifyDatasetProjection();
    }

    void NotifyDatasetProjection()
    {
        OnPropertyChanged(nameof(DatasetItems));
        OnPropertyChanged(nameof(IsDatasetEmpty));
        OnPropertyChanged(nameof(DatasetCount));
        OnPropertyChanged(nameof(DatasetEmptyState));
    }

    static string StatusText(MapDatasetStatus status) => status switch
    {
        MapDatasetStatus.Normal => "正常",
        MapDatasetStatus.Missing => "缺失",
        _ => "无效"
    };
}

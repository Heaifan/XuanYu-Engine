using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed record MapDatasetRow(string Type, string Id, string Status, string Source);

public sealed partial class UiVm
{
    readonly List<MapDatasetRow> _datasetItems = [];
    MapDatasetRegistry? _datasetRegistry;
    string _datasetCreateId = "";
    string _datasetCreateType = MapDatasetTypes.Region;
    string _datasetSelectedId = "";

    public IReadOnlyList<string> DatasetTypeOptions => MapDatasetTypes.All;
    public IReadOnlyList<MapDatasetRow> DatasetItems => _datasetItems;
    public bool IsDatasetEmpty => _datasetItems.Count == 0;
    public string DatasetCreateId { get => _datasetCreateId; set => Set(ref _datasetCreateId, value); }
    public string DatasetCreateType { get => _datasetCreateType; set => Set(ref _datasetCreateType, value); }
    public string DatasetSelectedId { get => _datasetSelectedId; set => Set(ref _datasetSelectedId, value); }

    async Task RefreshDatasetProjectionAsync()
    {
        _datasetItems.Clear();
        if (_datasetRegistry is not null)
        {
            var entries = await _datasetRegistry.EnumerateAsync();
            _datasetItems.AddRange(entries.Select(entry => new MapDatasetRow(
                entry.Descriptor.Type, entry.Descriptor.Id, StatusText(entry.Status), entry.Descriptor.Source)));
        }
        NotifyDatasetProjection();
    }

    void ResetDatasetProjection()
    {
        _datasetRegistry = null;
        _datasetItems.Clear();
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

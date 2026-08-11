namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string? _datasetSelectedId;

    public string? DatasetSelectedId
    {
        get => _datasetSelectedId;
        set
        {
            if (value == _datasetSelectedId) return;
            _datasetSelectedId = value;
            RefreshDatasetSelectionProjection();
        }
    }

    public MapDatasetRow? SelectedDataset => _datasetItems.FirstOrDefault(item => item.IsSelected);
    public bool HasSelectedDataset => SelectedDataset is not null;
    public bool CanUnregisterDataset => HasSelectedDataset;
    public IReadOnlyList<MapDatasetRow> DatasetLayerItems => _datasetItems;
    public string DatasetLayerEmptyState => IsDatasetEmpty ? "当前地图暂无数据集图层" : "";
    public void SelectDataset(string id)
    {
        if (_datasetItems.All(item => item.Id != id)) return;
        DatasetSelectedId = id;
    }

    void RefreshDatasetSelectionProjection()
    {
        _datasetItems = _datasetItems.Select(item => item with
        {
            IsSelected = string.Equals(item.Id, _datasetSelectedId, StringComparison.OrdinalIgnoreCase)
        }).ToArray();
        NotifyDatasetSelection();
    }

    void NotifyDatasetSelection()
    {
        OnPropertyChanged(nameof(DatasetItems));
        OnPropertyChanged(nameof(DatasetLayerItems));
        OnPropertyChanged(nameof(SelectedDataset));
        OnPropertyChanged(nameof(HasSelectedDataset));
        OnPropertyChanged(nameof(CanUnregisterDataset));
    }
}

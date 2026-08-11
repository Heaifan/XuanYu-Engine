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
            DatasetNameText = SelectedDataset?.Name ?? "";
        }
    }

    public MapDatasetRow? SelectedDataset => _datasetItems.FirstOrDefault(item => item.IsSelected);
    public bool HasSelectedDataset => SelectedDataset is not null;
    public bool CanUnregisterDataset => HasSelectedDataset && !SelectedDataset!.IsLocked;
    public IReadOnlyList<MapDatasetRow> DatasetLayerItems => _datasetItems;
    public string DatasetLayerEmptyState => IsDatasetEmpty ? "当前地图暂无数据集图层" : "";
    public void SelectDataset(string id)
    {
        if (_datasetItems.All(item => item.Id != id)) return;
        DatasetSelectedId = id;
    }

    public Task ToggleDatasetVisibilityAsync(string id) => UpdateLayersAsync(
        _datasetRegistry!.CurrentManifest.DatasetLayerStates.Select(item => item.DatasetId == id ? item with { IsVisible = !item.IsVisible } : item));

    public Task ToggleDatasetLockAsync(string id) => UpdateLayersAsync(
        _datasetRegistry!.CurrentManifest.DatasetLayerStates.Select(item => item.DatasetId == id ? item with { IsLocked = !item.IsLocked } : item));

    public async Task ReorderDatasetLayerAsync(string id, int targetIndex)
    {
        if (_datasetRegistry is null) return;
        var states = _datasetRegistry.CurrentManifest.DatasetLayerStates.OrderBy(item => item.Order).ToList();
        var state = states.FirstOrDefault(item => item.DatasetId == id);
        if (state is null) return;
        states.Remove(state);
        states.Insert(Math.Clamp(targetIndex, 0, states.Count), state);
        await UpdateLayersAsync(states);
    }

    async Task UpdateLayersAsync(IEnumerable<XuanYu.Editor.MapDocument.DatasetLayerState> states)
    {
        if (_datasetRegistry is null) return;
        var saved = _datasetRegistry.UpdateLayerStates(states);
        if (!saved.Succeeded) { FooterMessage = saved.Message; return; }
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        await RefreshDatasetProjectionAsync();
        RaiseMapDocumentChanged();
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

using XuanYu.Editor.MapDocument;

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
            if (_regionDrawing.IsActive) CancelRegionDrawingFromEscape();
            if (_roadDrawing.IsActive) CancelRoadDrawingFromEscape();
            _datasetSelectedId = value;
            SetDatasetDrawingTarget(value);
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
        var runtime = MapDatasetRuntimeProjection.Apply(MapSession.CurrentMap, _datasetRegistry.CurrentManifest);
        if (!runtime.Succeeded || runtime.Value is null) { FooterMessage = runtime.Message; return; }
        var applied = MapSession.ApplyRuntimeLayerProjection(runtime.Value);
        if (!applied.IsSuccess) { FooterMessage = applied.Error!.Value.Message; return; }
        await RefreshDatasetProjectionAsync();
        SetDatasetDrawingTarget(DatasetSelectedId);
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
        OnPropertyChanged(nameof(RegionDatasetItems));
        OnPropertyChanged(nameof(RoadDatasetItems));
        OnPropertyChanged(nameof(RegionDrawingTargetName));
        OnPropertyChanged(nameof(RegionDrawingTargetId));
        OnPropertyChanged(nameof(RegionDrawingTargetStatus));
        OnPropertyChanged(nameof(DatasetLayerItems));
        OnPropertyChanged(nameof(SelectedDataset));
        OnPropertyChanged(nameof(HasSelectedDataset));
        OnPropertyChanged(nameof(CanStartRegionDrawing));
        OnPropertyChanged(nameof(CanStartRoadDrawing));
        OnPropertyChanged(nameof(CanUnregisterDataset));
        OnPropertyChanged(nameof(InspectorSelectionTitle));
        OnPropertyChanged(nameof(InspectorSelectionSubtitle));
        OnPropertyChanged(nameof(HasInspectorSelection));
        OnPropertyChanged(nameof(IsInspectorEmpty));
        OnPropertyChanged(nameof(InspectorSectionTitle));
        OnPropertyChanged(nameof(InspectorFields));
    }
}

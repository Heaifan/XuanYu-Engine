using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层选择状态与列表刷新（选中是 UI 临时状态；内容/活动变化后重建列表）。
public sealed partial class UiVm
{
    public bool CanDeleteLayer => SelectedLayer is { } layer && CanDeleteLayerInternal(layer);

    bool CanDeleteLayerInternal(MapLayerRowViewModel layer)
    {
        if (TryGetDatasetIdForLayer(layer.LayerId, out var id))
            return _datasetItems.FirstOrDefault(item => item.Id == id) is { IsLocked: false };
        return MapLayerRules.CanRemove(MapSession.CurrentMap.Layers, layer.LayerId) is null;
    }

    // 内容/活动图层变化后重建列表（保持选中与活动标记）。
    void RefreshLayerItems()
    {
        _layerPresenter.Refresh(MapSession.CurrentMap.Layers, MapSession.ActiveRegionLayerId,
            layer => new MapLayerRowViewModel(this, layer));
        OnPropertyChanged(nameof(LayerItems));
        OnPropertyChanged(nameof(SelectedLayer));
        OnLayerSelectionChanged();
        RaiseLayerContextBindings();
    }

    public void ClearLayerSelection()
    {
        if (!_layerPresenter.ClearSelection()) return;
        OnLayerSelectionChanged();
    }

    void OnLayerSelectionChanged()
    {
        _layerInspectorNameText = SelectedLayer?.Name ?? "";
        OnPropertyChanged(nameof(HasLayerSelection));
        OnPropertyChanged(nameof(CanMoveLayerUp));
        OnPropertyChanged(nameof(CanMoveLayerDown));
        OnPropertyChanged(nameof(UserLayerCount));
        OnPropertyChanged(nameof(CanReorderLayers));
        OnPropertyChanged(nameof(IsLayerReorderHintVisible));
        OnPropertyChanged(nameof(CanDeleteLayer));
        OnPropertyChanged(nameof(LayerInspectorNameText));
        OnPropertyChanged(nameof(LayerInspectorKindText));
        OnPropertyChanged(nameof(LayerInspectorIdText));
        OnPropertyChanged(nameof(LayerInspectorOrderText));
        OnPropertyChanged(nameof(LayerInspectorIsRegion));
        OnPropertyChanged(nameof(LayerInspectorIsSystem));
        OnPropertyChanged(nameof(LayerInspectorVisible));
        OnPropertyChanged(nameof(LayerInspectorLocked));
        OnPropertyChanged(nameof(HasCurrentLayerSelection));
    }
}

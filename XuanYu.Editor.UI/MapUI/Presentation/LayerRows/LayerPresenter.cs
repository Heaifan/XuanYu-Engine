using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed class LayerPresenter : INotifyPropertyChanged
{
    List<MapLayerRowViewModel> _items = [];
    MapLayerId? _selectedLayerId;
    bool _mapMode, _regionMode;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<MapLayerRowViewModel?>? SelectionRequested;
    public IReadOnlyList<MapLayerRowViewModel> Items => _items;
    public IReadOnlyList<MapLayerRowViewModel> VisibleItems => !_mapMode && !_regionMode
        ? [] : _items.Where(row => _mapMode || row.IsRegion).ToArray();
    public MapLayerRowViewModel? SelectedLayer
    {
        get => _items.FirstOrDefault(row => row.LayerId == _selectedLayerId);
        set => SelectionRequested?.Invoke(value);
    }
    public bool HasSelection => SelectedLayer is not null;
    public int UserLayerCount(ImmutableArray<MapLayer> layers) => MapLayerStack.RegionLayers(layers).Length;
    public bool CanReorderLayers(ImmutableArray<MapLayer> layers) => UserLayerCount(layers) > 1;
    public bool IsReorderHintVisible(ImmutableArray<MapLayer> layers) => !CanReorderLayers(layers);

    public void SetWorkspaceMode(bool mapMode, bool regionMode)
    {
        if (_mapMode == mapMode && _regionMode == regionMode) return;
        _mapMode = mapMode;
        _regionMode = regionMode;
        OnPropertyChanged(nameof(VisibleItems));
    }

    public void Refresh(ImmutableArray<MapLayer> layers, MapLayerId activeLayerId,
        Func<MapLayer, MapLayerRowViewModel> createRow)
    {
        _items = layers.OrderByDescending(layer => layer.Order).Select(createRow).ToList();
        foreach (var row in _items) row.IsActive = row.LayerId == activeLayerId;
        OnPropertyChanged(nameof(Items)); OnPropertyChanged(nameof(VisibleItems));
        OnPropertyChanged(nameof(SelectedLayer)); OnPropertyChanged(nameof(HasSelection));
    }

    public bool SetSelectedLayer(MapLayerRowViewModel? row)
    {
        var id = row?.LayerId;
        if (_selectedLayerId == id) return false;
        _selectedLayerId = id;
        OnPropertyChanged(nameof(SelectedLayer)); OnPropertyChanged(nameof(HasSelection));
        return true;
    }

    public bool ClearSelection() => SetSelectedLayer(null);

    public bool CanMoveSelected(ImmutableArray<MapLayer> layers, bool up)
    {
        if (SelectedLayer is not { } layer) return false;
        return MapLayerRules.CanMove(layers, layer.LayerId, up) is null;
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

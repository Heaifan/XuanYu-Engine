using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

internal sealed class LayerPresenter
{
    List<MapLayerRowViewModel> _items = [];
    MapLayerId? _selectedLayerId;

    public IReadOnlyList<MapLayerRowViewModel> Items => _items;
    public MapLayerRowViewModel? SelectedLayer => _items.FirstOrDefault(row => row.LayerId == _selectedLayerId);
    public bool HasSelection => SelectedLayer is not null;
    public int UserLayerCount(ImmutableArray<MapLayer> layers) => MapLayerStack.RegionLayers(layers).Length;
    public bool CanReorderLayers(ImmutableArray<MapLayer> layers) => UserLayerCount(layers) > 1;
    public bool IsReorderHintVisible(ImmutableArray<MapLayer> layers) => !CanReorderLayers(layers);

    public void Refresh(ImmutableArray<MapLayer> layers, MapLayerId activeLayerId,
        Func<MapLayer, MapLayerRowViewModel> createRow)
    {
        _items = layers.OrderByDescending(layer => layer.Order).Select(createRow).ToList();
        foreach (var row in _items) row.IsActive = row.LayerId == activeLayerId;
    }

    public bool SetSelectedLayer(MapLayerRowViewModel? row)
    {
        var id = row?.LayerId;
        if (_selectedLayerId == id) return false;
        _selectedLayerId = id;
        return true;
    }

    public bool ClearSelection() => SetSelectedLayer(null);

    public bool CanMoveSelected(ImmutableArray<MapLayer> layers, bool up)
    {
        if (SelectedLayer is not { } layer) return false;
        return MapLayerRules.CanMove(layers, layer.LayerId, up) is null;
    }
}

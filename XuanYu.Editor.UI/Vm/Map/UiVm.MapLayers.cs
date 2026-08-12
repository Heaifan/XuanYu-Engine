using XuanYu.Core.Results;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层列表与工具栏命令入口（唯一数据源 = MapSession.CurrentMap）。
public sealed partial class UiVm
{
    List<MapLayerRowViewModel> _layerItems = [];
    MapLayerId? _selectedLayerId;

    public IReadOnlyList<MapLayerRowViewModel> LayerItems => _layerItems;

    public MapLayerRowViewModel? SelectedLayer
    {
        get => _layerItems.FirstOrDefault(l => l.LayerId == _selectedLayerId);
        set
        {
            var id = value?.LayerId;
            if (_selectedLayerId == id) return;
            _selectedLayerId = id;
            if (IsRegionEditMode && value is not null && TryGetDatasetIdForLayer(value.LayerId, out var datasetId))
                DatasetSelectedId = datasetId;
            OnLayerSelectionChanged();
        }
    }

    public bool HasLayerSelection => SelectedLayer is not null;

    public void AddLayer()
    {
        var result = MapSession.AddRegionLayer();
        if (!result.IsSuccess) { FailLayerEdit("添加图层", result); return; }
        var name = MapSession.CurrentMap.Layers[^1].DisplayName;
        LogLayer($"添加图层：名称={name}");
        FooterMessage = $"已添加图层：{name}。";
        SelectedLayer = _layerItems.FirstOrDefault(); // 新图层在列表最上方，自动选中
    }

    public void MoveLayerUp() => MoveLayer(up: true);

    public void MoveLayerDown() => MoveLayer(up: false);

    public void DeleteLayer()
    {
        if (SelectedLayer is not { } layer) return;
        DeleteLayer(layer.LayerId);
    }

    void DeleteLayer(MapLayerId layerId)
    {
        var layer = _layerItems.FirstOrDefault(item => item.LayerId == layerId);
        if (layer is null) { MapEditError = "要删除的图层已不存在。"; return; }
        var result = MapSession.RemoveLayer(layerId);
        if (!result.IsSuccess) { FailLayerEdit("图层删除", result); return; }
        LogLayer($"删除图层：{layer.Name}");
        FooterMessage = $"已删除图层：{layer.Name}。";
    }

    public bool SetLayerVisibility(MapLayerId layerId, bool visible)
    {
        var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, layerId);
        var result = MapSession.SetLayerVisibility(layerId, visible);
        if (!result.IsSuccess) { FailLayerEdit("图层可见性", result); return false; }
        LogLayer($"图层可见性：{layer?.DisplayName ?? ""}={(visible ? "显示" : "隐藏")}");
        return true;
    }

    public bool SetLayerLock(MapLayerId layerId, bool locked)
    {
        var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, layerId);
        var result = MapSession.SetLayerLocked(layerId, locked);
        if (!result.IsSuccess) { FailLayerEdit("图层锁定", result); return false; }
        // 仅状态真实变化时记录一次（同值 No-op 不记录）。
        if (layer is not null && layer.IsLocked != locked)
            LogLayerLockChanged(layer, layer.IsLocked, locked);
        return true;
    }

    public void SetActiveLayer()
    {
        if (SelectedLayer is not { } layer) return;
        var result = MapSession.SetActiveRegionLayer(layer.LayerId);
        if (!result.IsSuccess) { FailLayerEdit("设置当前图层", result); return; }
        LogLayer($"设置当前图层：{layer.Name}");
        FooterMessage = $"当前图层已设为：{layer.Name}。";
    }

    void MoveLayer(bool up)
    {
        if (SelectedLayer is not { } layer) return;
        var result = up ? MapSession.MoveLayerUp(layer.LayerId) : MapSession.MoveLayerDown(layer.LayerId);
        if (!result.IsSuccess) { FailLayerEdit("调整图层顺序", result); return; }
        LogLayer($"调整图层顺序：{layer.Name}，{(up ? "上移" : "下移")}");
        FooterMessage = $"图层已{(up ? "上移" : "下移")}：{layer.Name}。";
    }

    void FailLayerEdit(string operation, EngineResult result)
    {
        MapEditError = result.Error?.Message ?? "";
        OnPropertyChanged(nameof(MapEditError));
        LogLayer($"{operation}失败：{result.Error?.Message}");
    }
}

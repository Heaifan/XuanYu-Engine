using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D4：活动区域图层（会话临时状态：不进历史、不设 Dirty、不产生内容变更事件）。
// 内容修改后自动规范化到有效区域图层（H10）。
public sealed partial class MapEditSession
{
    // 活动图层变更（会话临时状态，不触发 ContentChanged/Dirty）。
    public event Action<MapLayerId>? ActiveRegionLayerChanged;

    // 设置当前活动区域图层。
    public EngineResult SetActiveRegionLayer(MapLayerId layerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "设置当前图层必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, layerId);
        if (layer is null) return Fail("UnknownLayer", "图层不存在。");
        if (layer.Kind != MapLayerKind.Region)
            return Fail("NotRegionLayer", "只有区域图层可以设为当前图层。");
        SetActiveLayerCore(layerId);
        return Ok();
    }

    EngineResult MoveLayer(MapLayerId layerId, bool up)
    {
        var error = MapLayerRules.CanMove(_currentMap.Layers, layerId, up);
        if (error is not null) return Fail("LayerMoveRejected", error);
        return CommitMapChange(
            map => map with
            {
                Layers = up
                    ? MapLayerStack.MoveUp(map.Layers, layerId)
                    : MapLayerStack.MoveDown(map.Layers, layerId)
            },
            MapEditReason.LayerMoved);
    }

    // 内容变化后活动图层规范化：失效时回退第一个区域图层（H10）。
    void NormalizeActiveLayer()
    {
        if (MapLayerRules.Find(_currentMap.Layers, ActiveRegionLayerId) is { Kind: MapLayerKind.Region }) return;
        var first = MapLayerStack.RegionLayers(_currentMap.Layers).FirstOrDefault();
        if (first is not null) SetActiveLayerCore(first.LayerId);
    }

    void SetActiveLayerCore(MapLayerId layerId)
    {
        if (_activeRegionLayerId == layerId) return;
        _activeRegionLayerId = layerId;
        ActiveRegionLayerChanged?.Invoke(layerId);
    }
}

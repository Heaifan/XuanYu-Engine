using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D4：图层内容修改命令（走 CommitMapChange：单历史节点、失败零污染）。
public sealed partial class MapEditSession
{
    public EngineResult AddRegionLayer()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "添加图层必须在编辑写线程执行。");
        var name = MapLayerRules.NextRegionName(_currentMap.Layers);
        var layer = MapLayerStack.CreateRegionLayer(_currentMap.Layers, name);
        var result = CommitMapChange(
            map => map with { Layers = map.Layers.Add(layer) },
            MapEditReason.LayerAdded);
        if (result.IsSuccess) SetActiveLayerCore(layer.LayerId);
        return result;
    }

    public EngineResult RenameLayer(MapLayerId layerId, string name)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "重命名图层必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, layerId);
        if (layer is null) return Fail("UnknownLayer", "图层不存在。");
        if (MapLayerRules.IsSystemLayer(layer.Kind))
            return Fail("SystemLayerProtected", "系统图层不能重命名。");
        if (MapLayerRules.ValidateName(name) is { } error) return Fail("InvalidLayerName", error);
        var trimmed = name.Trim();
        if (layer.DisplayName == trimmed) return Ok(); // No-op：同值成功且无历史
        return CommitMapChange(
            map => map with
            {
                Layers = map.Layers.SetItem(
                    map.Layers.IndexOf(layer), MapLayerStack.Rename(layer, trimmed))
            },
            MapEditReason.LayerRenamed);
    }

    public EngineResult RemoveLayer(MapLayerId layerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "删除图层必须在编辑写线程执行。");
        var error = MapLayerRules.CanRemove(_currentMap.Layers, layerId);
        if (error is not null) return Fail("LayerRemovalRejected", error);
        var regions = MapLayerStack.RegionLayers(_currentMap.Layers);
        var index = MapLayerRules.IndexOfId(regions, layerId);
        var result = CommitMapChange(
            map => map with { Layers = MapLayerStack.Remove(map.Layers, layerId) },
            MapEditReason.LayerRemoved);
        if (result.IsSuccess && layerId == ActiveRegionLayerId)
        {
            // 删除活动图层：活动转移到相邻区域图层（下方优先，无则最下方）。
            var remaining = MapLayerStack.RegionLayers(_currentMap.Layers);
            var next = remaining[System.Math.Min(index, remaining.Length - 1)];
            SetActiveLayerCore(next.LayerId);
        }
        return result;
    }

    public EngineResult MoveLayerUp(MapLayerId layerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "调整图层顺序必须在编辑写线程执行。");
        return MoveLayer(layerId, up: true);
    }
    public EngineResult MoveLayerDown(MapLayerId layerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "调整图层顺序必须在编辑写线程执行。");
        return MoveLayer(layerId, up: false);
    }

    public EngineResult SetLayerVisibility(MapLayerId layerId, bool visible)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改图层可见性必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, layerId);
        if (layer is null) return Fail("UnknownLayer", "图层不存在。");
        if (layer.IsVisible == visible) return Ok(); // No-op：同值成功且无历史
        return CommitMapChange(
            map => map with
            {
                Layers = map.Layers.SetItem(
                    map.Layers.IndexOf(layer), MapLayerStack.SetVisibility(layer, visible))
            },
            MapEditReason.LayerVisibilityChanged);
    }

    public EngineResult SetLayerLocked(MapLayerId layerId, bool locked)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改图层锁定必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, layerId);
        if (layer is null) return Fail("UnknownLayer", "图层不存在。");
        if (layer.IsLocked == locked) return Ok(); // No-op：同值成功且无历史
        return CommitMapChange(
            map => map with
            {
                Layers = map.Layers.SetItem(
                    map.Layers.IndexOf(layer), MapLayerStack.SetLocked(layer, locked))
            },
            MapEditReason.LayerLockChanged);
    }
}

using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：选择状态。只保存稳定 ID；选择不产生 Dirty、不写入历史。
public sealed partial class MapEditSession
{
    public EngineResult SelectMap()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "选择地图必须在编辑写线程执行。");
        SetSelection(MapSelection.Map);
        return Ok();
    }

    public EngineResult SelectLayer(MapLayerId layerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "选择图层必须在编辑写线程执行。");
        if (_currentMap.Layers.All(l => l.LayerId != layerId))
            return Fail("UnknownLayer", "图层不存在。");
        SetSelection(MapSelection.Layer(layerId));
        return Ok();
    }

    public EngineResult SelectRegion(MapRegionId regionId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "选择区域必须在编辑写线程执行。");
        var region = _currentMap.Regions.FirstOrDefault(r => r.RegionId == regionId);
        if (region is null)
            return Fail("UnknownRegion", "区域不存在。");
        SetSelection(MapSelection.Region(regionId, region.LayerId));
        return Ok();
    }

    public EngineResult ClearSelection()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "清除选择必须在编辑写线程执行。");
        SetSelection(MapSelection.None);
        return Ok();
    }

    // CurrentMap 变化后规范化：区域仍在→保持；区域不在但图层在→回退图层；
    // 图层不在→回退地图。新建/替换→选择地图。
    void NormalizeSelection()
    {
        var layerId = _selection.LayerId;
        var regionId = _selection.RegionId;
        switch (_selection.Kind)
        {
            case MapSelectionKind.Region when regionId is { } rid &&
                _currentMap.Regions.Any(r => r.RegionId == rid):
                return;
            case MapSelectionKind.Region when layerId is { } lid &&
                _currentMap.Layers.Any(l => l.LayerId == lid):
                SetSelection(MapSelection.Layer(lid));
                return;
            case MapSelectionKind.Region:
                SetSelection(MapSelection.Map);
                return;
            case MapSelectionKind.Layer when layerId is { } lid &&
                _currentMap.Layers.Any(l => l.LayerId == lid):
                return;
            case MapSelectionKind.Layer:
                SetSelection(MapSelection.Map);
                return;
            default:
                return;
        }
    }

    bool SetSelection(MapSelection selection)
    {
        if (_selection == selection) return false;
        _selection = selection;
        RaiseSelectionChanged();
        return true;
    }

    void RaiseSelectionChanged() =>
        SelectionChanged?.Invoke(new MapSelectionChangedEventArgs(_selection));
}

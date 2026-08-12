using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult CreateRoad(MapRoadDraft draft)
    {
        if (!draft.CanComplete) return Fail("TooFewRoadPoints", "道路至少需要两个节点才能提交。");
        return CreateRoad(draft.Complete(MapRoadId.New()));
    }

    public EngineResult CreateRoad(MapRoad road)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "创建道路必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, road.LayerId);
        if (layer?.IsLocked == true) return Fail("RoadLayerLocked", "道路所属图层已锁定。");
        return CommitMapChange(map => map with { Roads = (map.Roads.IsDefault ? [] : map.Roads).Add(road) }, MapEditReason.RoadCreated);
    }
}

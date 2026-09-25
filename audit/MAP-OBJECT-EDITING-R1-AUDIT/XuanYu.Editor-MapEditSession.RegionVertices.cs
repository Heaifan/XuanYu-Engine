using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult InsertRegionVertex(MapRegionId regionId, int segmentIndex, MapPoint point)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "新增区域顶点必须在编辑写线程执行。");
        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return Fail("UnknownRegion", "区域不存在。");
        if (segmentIndex < 0 || segmentIndex >= region.Vertices.Length)
            return Fail("InvalidRegionSegment", "新增顶点必须位于有效区域边上。");
        return EditRegionVertices(regionId, region.Vertices.Insert((segmentIndex + 1) % region.Vertices.Length, point));
    }
}

using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionVertexSnapResult(
    MapPoint ResolvedPoint,
    bool IsSnapped,
    MapRegionId? TargetRegionId,
    int TargetVertexIndex)
{
    public static RegionVertexSnapResult Raw(MapPoint point) => new(point, false, null, -1);
}

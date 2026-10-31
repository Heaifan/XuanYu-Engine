using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionVertexSnapResult(
    MapPoint ResolvedPoint,
    bool IsSnapped,
    MapRegionId? TargetRegionId,
    int TargetVertexIndex,
    double ScreenDistance)
{
    public static RegionVertexSnapResult Raw(MapPoint point) => new(point, false, null, -1, double.PositiveInfinity);
}

using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionEdgeSnapResult(
    MapPoint ResolvedPoint,
    RegionSnapKind Kind,
    MapRegionId? TargetRegionId,
    int TargetIndex,
    double DistanceSquared)
{
    public bool IsSnapped => Kind != RegionSnapKind.None;

    public static RegionEdgeSnapResult Raw(MapPoint point) =>
        new(point, RegionSnapKind.None, null, -1, double.PositiveInfinity);
}

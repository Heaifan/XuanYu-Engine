using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionEdgeSnapLockResolver
{
    public static bool TryResolve(
        MapRegionId regionId, int segmentIndex, ScreenPoint pointer,
        ImmutableArray<RegionEdgeSnapRegion> candidates, double releaseRadius,
        out RegionEdgeSnapResult result)
    {
        result = default;
        var region = candidates.FirstOrDefault(item => item.RegionId == regionId);
        if (region.RegionId != regionId || segmentIndex < 0 || segmentIndex >= region.Vertices.Length)
            return false;
        var next = (segmentIndex + 1) % region.Vertices.Length;
        var start = region.Vertices[segmentIndex]; var end = region.Vertices[next];
        if (!RegionEdgeSnapGeometry.TryClosestPoint(pointer, start.ScreenPoint, end.ScreenPoint,
                out var closest, out var parameter)) return false;
        var distance = DistanceSquared(pointer, closest);
        if (distance > releaseRadius * releaseRadius) return false;
        var world = new MapPoint(start.WorldPoint.X + (end.WorldPoint.X - start.WorldPoint.X) * parameter,
            start.WorldPoint.Y + (end.WorldPoint.Y - start.WorldPoint.Y) * parameter);
        result = new(world, RegionSnapKind.Edge, regionId, segmentIndex, distance);
        return true;
    }

    static double DistanceSquared(ScreenPoint a, ScreenPoint b) =>
        Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
}

using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionEdgeSnapResolver
{
    public static RegionEdgeSnapResult Resolve(
        MapRegionId sourceRegionId,
        MapPoint rawWorldPoint,
        ScreenPoint rawScreenPoint,
        ImmutableArray<RegionEdgeSnapRegion> candidates,
        RegionEdgeSnapSettings settings)
    {
        settings.Validate();
        var radiusSquared = settings.EnterRadiusPx * settings.EnterRadiusPx;
        var vertex = FindVertex(sourceRegionId, rawScreenPoint, candidates, radiusSquared);
        if (vertex is { } target)
            return new(target.WorldPoint, RegionSnapKind.Vertex, target.RegionId, target.VertexIndex, target.DistanceSquared);
        var edge = FindEdge(sourceRegionId, rawScreenPoint, candidates, radiusSquared);
        return edge is { } hit
            ? new(hit.WorldPoint, RegionSnapKind.Edge, hit.RegionId, hit.EdgeIndex, hit.DistanceSquared)
            : RegionEdgeSnapResult.Raw(rawWorldPoint);
    }

    static VertexHit? FindVertex(MapRegionId source, ScreenPoint raw,
        ImmutableArray<RegionEdgeSnapRegion> regions, double radiusSquared)
    {
        VertexHit? best = null;
        foreach (var region in regions)
        {
            if (region.RegionId == source) continue;
            for (var index = 0; index < region.Vertices.Length; index++)
            {
                var vertex = region.Vertices[index];
                var distance = DistanceSquared(raw, vertex.ScreenPoint);
                if (distance <= radiusSquared && IsBetter(best, distance, region.RegionId, index))
                    best = new(region.RegionId, index, vertex.WorldPoint, distance);
            }
        }
        return best;
    }

    static EdgeHit? FindEdge(MapRegionId source, ScreenPoint raw,
        ImmutableArray<RegionEdgeSnapRegion> regions, double radiusSquared)
    {
        EdgeHit? best = null;
        foreach (var region in regions)
        {
            if (region.RegionId == source || region.Vertices.Length < 2) continue;
            for (var index = 0; index < region.Vertices.Length; index++)
            {
                var next = (index + 1) % region.Vertices.Length;
                var start = region.Vertices[index];
                var end = region.Vertices[next];
                if (!RegionEdgeSnapGeometry.TryClosestPoint(raw, start.ScreenPoint, end.ScreenPoint,
                        out var closest, out var parameter) || parameter <= double.Epsilon || parameter >= 1 - double.Epsilon)
                    continue;
                var distance = DistanceSquared(raw, closest);
                if (distance > radiusSquared || !IsBetter(best, distance, region.RegionId, index)) continue;
                var world = new MapPoint(start.WorldPoint.X + (end.WorldPoint.X - start.WorldPoint.X) * parameter,
                    start.WorldPoint.Y + (end.WorldPoint.Y - start.WorldPoint.Y) * parameter);
                best = new(region.RegionId, index, world, distance);
            }
        }
        return best;
    }

    static bool IsBetter(VertexHit? current, double distance, MapRegionId id, int index) =>
        current is null || distance < current.Value.DistanceSquared ||
        distance == current.Value.DistanceSquared && Compare(id, current.Value.RegionId, index, current.Value.VertexIndex);

    static bool IsBetter(EdgeHit? current, double distance, MapRegionId id, int index) =>
        current is null || distance < current.Value.DistanceSquared ||
        distance == current.Value.DistanceSquared && Compare(id, current.Value.RegionId, index, current.Value.EdgeIndex);

    static bool Compare(MapRegionId id, MapRegionId currentId, int index, int currentIndex) =>
        string.CompareOrdinal(id.Value ?? "", currentId.Value ?? "") < 0 ||
        id == currentId && index < currentIndex;

    static double DistanceSquared(ScreenPoint a, ScreenPoint b) =>
        Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);

    readonly record struct VertexHit(MapRegionId RegionId, int VertexIndex, MapPoint WorldPoint, double DistanceSquared);
    readonly record struct EdgeHit(MapRegionId RegionId, int EdgeIndex, MapPoint WorldPoint, double DistanceSquared);
}

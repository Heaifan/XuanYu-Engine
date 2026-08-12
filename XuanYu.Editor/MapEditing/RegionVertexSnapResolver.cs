using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionVertexSnapResolver
{
    public static RegionVertexSnapResult Resolve(
        MapRegionId sourceRegionId,
        MapPoint rawPoint,
        double pointerX,
        double pointerY,
        MapDefinition map,
        ViewProjectionState projection,
        RegionVertexSnapState state,
        Func<RegionSpatialBounds, ImmutableArray<MapRegionId>> localQuery,
        Func<MapRegionId, MapRegion?> regionLookup,
        RegionVertexSnapSettings settings)
    {
        settings.Validate();
        if (state.IsSnapped && TryKeep(state, pointerX, pointerY, map, projection, regionLookup, settings))
            return new(state.TargetPoint, true, state.TargetRegionId, state.TargetVertexIndex);
        state.Clear();
        if (!TryQueryBounds(pointerX, pointerY, map, projection, settings.EnterRadiusPx, out var bounds))
            return RegionVertexSnapResult.Raw(rawPoint);
        ImmutableArray<MapRegionId> candidates;
        try { candidates = localQuery(bounds); }
        catch (InvalidOperationException) { return RegionVertexSnapResult.Raw(rawPoint); }
        var best = FindNearest(sourceRegionId, pointerX, pointerY, map, projection, candidates, regionLookup, settings.EnterRadiusPx);
        if (best is not { } target) return RegionVertexSnapResult.Raw(rawPoint);
        state.Acquire(target.RegionId, target.VertexIndex, target.Point);
        return new(target.Point, true, target.RegionId, target.VertexIndex);
    }

    static bool TryKeep(RegionVertexSnapState state, double x, double y, MapDefinition map,
        ViewProjectionState projection, Func<MapRegionId, MapRegion?> lookup, RegionVertexSnapSettings settings)
    {
        if (state.TargetRegionId is not { } id || lookup(id) is not { } region ||
            state.TargetVertexIndex >= region.Vertices.Length) return false;
        var point = region.Vertices[state.TargetVertexIndex];
        if (!TryProject(point, map, projection, out var screen)) return false;
        if (DistanceSquared(x, y, screen.X, screen.Y) > settings.ReleaseRadiusPx * settings.ReleaseRadiusPx) return false;
        state.Acquire(id, state.TargetVertexIndex, point);
        return true;
    }

    static SnapTarget? FindNearest(MapRegionId source, double x, double y, MapDefinition map,
        ViewProjectionState projection, ImmutableArray<MapRegionId> candidates,
        Func<MapRegionId, MapRegion?> lookup, double radius)
    {
        SnapTarget? best = null; var radiusSquared = radius * radius;
        foreach (var id in candidates)
        {
            if (id == source || lookup(id) is not { } region) continue;
            for (var index = 0; index < region.Vertices.Length; index++)
            {
                var point = region.Vertices[index];
                if (!TryProject(point, map, projection, out var screen)) continue;
                var distance = DistanceSquared(x, y, screen.X, screen.Y);
                if (distance > radiusSquared || !IsBetter(best, distance, id, index)) continue;
                best = new(id, index, point, distance);
            }
        }
        return best;
    }

    static bool IsBetter(SnapTarget? current, double distance, MapRegionId id, int index) =>
        current is null || distance < current.Value.Distance ||
        distance == current.Value.Distance && (string.CompareOrdinal(id.Value, current.Value.RegionId.Value) < 0 ||
        id == current.Value.RegionId && index < current.Value.VertexIndex);

    static bool TryQueryBounds(double x, double y, MapDefinition map, ViewProjectionState projection,
        double radius, out RegionSpatialBounds bounds)
    {
        var points = new List<MapPoint>(4);
        foreach (var (px, py) in new[] { (x - radius, y - radius), (x + radius, y - radius),
            (x - radius, y + radius), (x + radius, y + radius) })
            if (!MapSurfacePicker.TryPick(map, projection, px, py, out var point)) { bounds = default; return false; }
            else points.Add(point);
        bounds = new(points.Min(p => p.X), points.Min(p => p.Y), points.Max(p => p.X), points.Max(p => p.Y));
        return true;
    }

    static bool TryProject(MapPoint point, MapDefinition map, ViewProjectionState projection, out ScreenPoint screen) =>
        projection.TryProjectWorldPoint(new(point.X, point.Y, map.Surface.BaseHeightMeters), out screen);

    static double DistanceSquared(double ax, double ay, double bx, double by) =>
        Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2);

    readonly record struct SnapTarget(MapRegionId RegionId, int VertexIndex, MapPoint Point, double Distance);
}

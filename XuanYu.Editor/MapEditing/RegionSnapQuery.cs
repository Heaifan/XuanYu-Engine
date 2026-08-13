using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionSnapQuery
{
    public static bool TryBounds(double x, double y, double radius,
        MapDefinition map, ViewProjectionState projection, out RegionSpatialBounds bounds)
    {
        var points = new List<MapPoint>(4);
        foreach (var (px, py) in new[] { (x - radius, y - radius), (x + radius, y - radius),
            (x - radius, y + radius), (x + radius, y + radius) })
            if (!MapSurfacePicker.TryPick(map, projection, px, py, out var point))
            { bounds = default; return false; }
            else points.Add(point);
        bounds = new(points.Min(p => p.X), points.Min(p => p.Y), points.Max(p => p.X), points.Max(p => p.Y));
        return true;
    }

    public static ImmutableArray<RegionEdgeSnapRegion> BuildCandidates(
        ImmutableArray<MapRegionId> ids, Func<MapRegionId, MapRegion?> lookup,
        MapDefinition map, ViewProjectionState projection)
    {
        var result = ImmutableArray.CreateBuilder<RegionEdgeSnapRegion>();
        foreach (var id in ids)
            if (lookup(id) is { } region && TryBuildRegion(region, map.Surface.BaseHeightMeters, projection, out var candidate))
                result.Add(candidate);
        return result.ToImmutable();
    }

    static bool TryBuildRegion(MapRegion region, double height, ViewProjectionState projection,
        out RegionEdgeSnapRegion candidate)
    {
        var vertices = ImmutableArray.CreateBuilder<RegionEdgeSnapVertex>(region.Vertices.Length);
        for (var index = 0; index < region.Vertices.Length; index++)
        {
            var point = region.Vertices[index];
            if (!projection.TryProjectWorldPoint(new(point.X, point.Y, height), out var screen))
            { candidate = default; return false; }
            vertices.Add(new(region.RegionId, index, point, screen));
        }
        candidate = new(region.RegionId, vertices.ToImmutable());
        return true;
    }
}

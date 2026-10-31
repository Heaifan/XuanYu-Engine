using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static partial class MapGeometryContextHitTester
{
    static void TrySegments(ViewProjectionState p, IReadOnlyList<MapPoint> points, MapGeometrySelection selection,
        bool closed, double x, double y, double height, ref MapGeometryContextHit best)
    {
        var count = closed ? points.Count : points.Count - 1;
        for (var i = 0; i < count; i++)
        {
            var next = (i + 1) % points.Count;
            if (!p.TryProjectWorldPoint(World(points[i], height), out var a) ||
                !p.TryProjectWorldPoint(World(points[next], height), out var b)) continue;
            var (distance, t) = SegmentProjection(a.X, a.Y, b.X, b.Y, x, y);
            if (distance <= EdgeRadius && distance < best.DistanceDip)
            {
                var start = points[i]; var end = points[next];
                var projected = new MapPoint(start.X + t * (end.X - start.X), start.Y + t * (end.Y - start.Y));
                best = new(MapGeometryContextKind.Edge, selection, -1, i, distance, t, projected);
            }
        }
    }

    static bool TryFace(MapDefinition map, ViewProjectionState p, double x, double y, double height,
        out MapGeometryContextHit hit)
    {
        foreach (var region in map.Regions
                     .Where(item => EditableRegion(map, item))
                     .OrderByDescending(item => MapLayerRules.Find(map.Layers, item.LayerId)?.Order ?? int.MinValue))
        {
            var points = region.Vertices.Select(point => p.TryProjectWorldPoint(World(point, height), out var s)
                ? (true, s.X, s.Y) : (false, 0d, 0d)).ToArray();
            if (points.Length >= 3 && points.All(point => point.Item1) &&
                Inside(points.Select(point => (point.Item2, point.Item3)), x, y))
            {
                hit = new(MapGeometryContextKind.Face, new(MapGeometryFeatureKind.Region, region.RegionId.ToString()), -1, -1, 0);
                return true;
            }
        }
        hit = MapGeometryContextHit.Empty;
        return false;
    }

    static bool Inside(IEnumerable<(double X, double Y)> source, double x, double y)
    {
        var points = source.ToArray(); var inside = false;
        for (var i = 0; i < points.Length; i++)
        {
            var a = points[i]; var b = points[(i + points.Length - 1) % points.Length];
            if ((a.Y > y) != (b.Y > y) && x < (b.X - a.X) * (y - a.Y) / (b.Y - a.Y) + a.X) inside = !inside;
        }
        return inside;
    }

    static (double Distance, double T) SegmentProjection(double ax, double ay, double bx, double by, double x, double y)
    {
        var dx = bx - ax; var dy = by - ay;
        if (dx == 0 && dy == 0) return (Distance(ax, ay, x, y), 0);
        var t = Math.Clamp(((x - ax) * dx + (y - ay) * dy) / (dx * dx + dy * dy), 0, 1);
        return (Distance(ax + t * dx, ay + t * dy, x, y), t);
    }

    static double Distance(double ax, double ay, double bx, double by) => Math.Sqrt(Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2));
    static XuanYu.Core.Math.Vector3d World(MapPoint point, double height) => new(point.X, point.Y, height);
}

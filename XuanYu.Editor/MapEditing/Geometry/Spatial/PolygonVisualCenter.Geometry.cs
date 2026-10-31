using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

static class PolygonVisualCenterGeometry
{
    internal readonly record struct Bounds(double MinX, double MinY, double MaxX, double MaxY)
    {
        public double Width => MaxX - MinX;
        public double Height => MaxY - MinY;
    }

    internal readonly record struct Candidate(MapPoint Center, double Distance);

    internal static bool IsValid(IReadOnlyList<MapPoint> polygon, out Bounds bounds)
    {
        bounds = default;
        if (polygon.Count < 3) return false;
        var minX = double.PositiveInfinity; var minY = double.PositiveInfinity;
        var maxX = double.NegativeInfinity; var maxY = double.NegativeInfinity;
        foreach (var p in polygon)
        {
            if (!double.IsFinite(p.X) || !double.IsFinite(p.Y)) return false;
            minX = Math.Min(minX, p.X); minY = Math.Min(minY, p.Y);
            maxX = Math.Max(maxX, p.X); maxY = Math.Max(maxY, p.Y);
        }
        bounds = new(minX, minY, maxX, maxY);
        return bounds.Width > 1e-9 && bounds.Height > 1e-9;
    }

    internal static Candidate Evaluate(MapPoint center, IReadOnlyList<MapPoint> polygon) =>
        new(center, SignedDistance(center, polygon));

    internal static MapPoint Centroid(IReadOnlyList<MapPoint> polygon, Bounds bounds)
    {
        var area = 0.0; var x = 0.0; var y = 0.0;
        for (var i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i]; var b = polygon[(i + 1) % polygon.Count];
            var cross = a.X * b.Y - b.X * a.Y;
            area += cross; x += (a.X + b.X) * cross; y += (a.Y + b.Y) * cross;
        }
        if (Math.Abs(area) <= 1e-9) return new((bounds.MinX + bounds.MaxX) * .5, (bounds.MinY + bounds.MaxY) * .5);
        return new(x / (3 * area), y / (3 * area));
    }

    internal static double SignedDistance(MapPoint point, IReadOnlyList<MapPoint> polygon)
    {
        var inside = false; var minDistance = double.PositiveInfinity;
        for (var i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i]; var b = polygon[(i + 1) % polygon.Count];
            if ((a.Y > point.Y) != (b.Y > point.Y) &&
                point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X) inside = !inside;
            minDistance = Math.Min(minDistance, SegmentDistance(point, a, b));
        }
        return (inside ? 1 : -1) * minDistance;
    }

    static double SegmentDistance(MapPoint p, MapPoint a, MapPoint b)
    {
        var dx = b.X - a.X; var dy = b.Y - a.Y;
        var lengthSquared = dx * dx + dy * dy;
        if (lengthSquared <= 1e-18)
        {
            var dx0 = p.X - a.X; var dy0 = p.Y - a.Y;
            return Math.Sqrt(dx0 * dx0 + dy0 * dy0);
        }
        var t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lengthSquared, 0, 1);
        var x = a.X + t * dx; var y = a.Y + t * dy;
        var dx1 = p.X - x; var dy1 = p.Y - y;
        return Math.Sqrt(dx1 * dx1 + dy1 * dy1);
    }
}

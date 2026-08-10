using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

static class MapVectorOverlayTriangulation
{
    public static IReadOnlyList<int> Triangulate(IReadOnlyList<MapPoint> points)
    {
        var order = Enumerable.Range(0, points.Count).ToList();
        if (SignedArea(points) < 0) order.Reverse();
        var result = new List<int>();
        while (order.Count > 3)
        {
            var ear = -1;
            for (var i = 0; i < order.Count; i++)
            {
                var a = points[order[(i + order.Count - 1) % order.Count]];
                var b = points[order[i]];
                var c = points[order[(i + 1) % order.Count]];
                if (Cross(a, b, c) <= 0 || order.Any(x => x != order[i] && x != order[(i + order.Count - 1) % order.Count]
                    && x != order[(i + 1) % order.Count] && Inside(points[x], a, b, c))) continue;
                ear = i; result.AddRange([order[(i + order.Count - 1) % order.Count], order[i], order[(i + 1) % order.Count]]);
                break;
            }
            if (ear < 0) return [];
            order.RemoveAt(ear);
        }
        if (order.Count == 3) result.AddRange(order);
        return result;
    }

    static double SignedArea(IReadOnlyList<MapPoint> p) =>
        p.Select((x, i) => x.X * p[(i + 1) % p.Count].Y - p[(i + 1) % p.Count].X * x.Y).Sum() / 2;
    static double Cross(MapPoint a, MapPoint b, MapPoint c) =>
        (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    static bool Inside(MapPoint p, MapPoint a, MapPoint b, MapPoint c) =>
        Cross(a, b, p) >= 0 && Cross(b, c, p) >= 0 && Cross(c, a, p) >= 0;
}

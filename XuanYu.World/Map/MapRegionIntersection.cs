using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R3-D1：正式区域的非相邻边相交校验。相交、接触、重叠均非法。
internal static class MapRegionIntersection
{
    public static bool HasNonAdjacentIntersection(ImmutableArray<MapPoint> vertices)
    {
        for (var first = 0; first < vertices.Length; first++)
        {
            var nextFirst = (first + 1) % vertices.Length;
            for (var second = first + 1; second < vertices.Length; second++)
            {
                var nextSecond = (second + 1) % vertices.Length;
                if (nextFirst == second || (first == 0 && nextSecond == 0)) continue;
                if (Intersects(vertices[first], vertices[nextFirst],
                    vertices[second], vertices[nextSecond])) return true;
            }
        }

        return false;
    }

    static bool Intersects(MapPoint a, MapPoint b, MapPoint c, MapPoint d)
    {
        var abC = Orientation(a, b, c);
        var abD = Orientation(a, b, d);
        var cdA = Orientation(c, d, a);
        var cdB = Orientation(c, d, b);
        if (abC == 0 && OnSegment(a, b, c)) return true;
        if (abD == 0 && OnSegment(a, b, d)) return true;
        if (cdA == 0 && OnSegment(c, d, a)) return true;
        if (cdB == 0 && OnSegment(c, d, b)) return true;
        return abC != abD && cdA != cdB;
    }

    static double Orientation(MapPoint a, MapPoint b, MapPoint c) =>
        (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    static bool OnSegment(MapPoint a, MapPoint b, MapPoint point) =>
        point.X >= Math.Min(a.X, b.X) && point.X <= Math.Max(a.X, b.X)
        && point.Y >= Math.Min(a.Y, b.Y) && point.Y <= Math.Max(a.Y, b.Y);
}

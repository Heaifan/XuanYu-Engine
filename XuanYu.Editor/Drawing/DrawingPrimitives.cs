using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public enum DrawingPrimitiveKind
{
    Point,
    Polyline,
    Polygon
}

public sealed record DrawingPolyline(IReadOnlyList<MapPoint> Points);

public sealed record DrawingPolygon(IReadOnlyList<MapPoint> Points);

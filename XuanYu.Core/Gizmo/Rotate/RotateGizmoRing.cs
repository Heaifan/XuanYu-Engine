namespace XuanYu.Core.Gizmo;

// 一条旋转环的屏幕折线几何。命中以"指针到折线最近距离"为唯一真源，
// 与 MoveGizmo 的"可见几何=命中区"原则一致，禁止另开大半径。
public readonly record struct RotateGizmoRing(
    RotateGizmoAxis Axis,
    ScreenPoint Center,
    IReadOnlyList<ScreenPoint> Points,
    double ScreenRadius)
{
    // 屏幕半径过小说明该环几乎正对相机（边视），不可靠，不应作为命中目标。
    public bool IsEdgeOn => ScreenRadius < RotateGizmoLayout.EdgeOnScreenRadius;

    public double NearestDistance(double x, double y)
    {
        if (Points.Count == 0) return double.MaxValue;
        var min = double.MaxValue;
        for (var i = 0; i < Points.Count; i++)
        {
            var a = Points[i];
            var b = Points[(i + 1) % Points.Count];
            var d = SegmentDistance(a, b, x, y);
            if (d < min) min = d;
        }
        return min;
    }

    static double SegmentDistance(ScreenPoint a, ScreenPoint b, double x, double y)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var length2 = (dx * dx) + (dy * dy);
        if (length2 == 0)
            return global::System.Math.Sqrt(((x - a.X) * (x - a.X)) + ((y - a.Y) * (y - a.Y)));
        var sx = x - a.X;
        var sy = y - a.Y;
        var t = ((sx * dx) + (sy * dy)) / length2;
        t = global::System.Math.Clamp(t, 0, 1);
        var px = a.X + (t * dx);
        var py = a.Y + (t * dy);
        return global::System.Math.Sqrt(((x - px) * (x - px)) + ((y - py) * (y - py)));
    }
}

namespace XuanYu.Core.Gizmo;

public readonly record struct MoveGizmoPlane(
    MoveGizmoAxis Axis,
    ScreenPoint A,
    ScreenPoint B,
    ScreenPoint C,
    ScreenPoint D,
    ScreenPoint HitA,
    ScreenPoint HitB,
    ScreenPoint HitC,
    ScreenPoint HitD)
{
    public bool ContainsVisible(double x, double y) =>
        InTriangle(x, y, A, B, C) || InTriangle(x, y, A, C, D);

    public bool Contains(double x, double y) =>
        InTriangle(x, y, HitA, HitB, HitC) || InTriangle(x, y, HitA, HitC, HitD);

    public double CenterDistanceSquared(double x, double y)
    {
        var cx = (A.X + B.X + C.X + D.X) / 4.0;
        var cy = (A.Y + B.Y + C.Y + D.Y) / 4.0;
        var dx = x - cx;
        var dy = y - cy;
        return (dx * dx) + (dy * dy);
    }

    static bool InTriangle(
        double x,
        double y,
        ScreenPoint a,
        ScreenPoint b,
        ScreenPoint c)
    {
        var d1 = Sign(x, y, a, b);
        var d2 = Sign(x, y, b, c);
        var d3 = Sign(x, y, c, a);
        var hasNegative = d1 < 0 || d2 < 0 || d3 < 0;
        var hasPositive = d1 > 0 || d2 > 0 || d3 > 0;
        return !(hasNegative && hasPositive);
    }

    static double Sign(double x, double y, ScreenPoint a, ScreenPoint b) =>
        ((x - b.X) * (a.Y - b.Y)) - ((a.X - b.X) * (y - b.Y));
}

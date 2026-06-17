namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>Gizmo 命中和视觉用的点线距离计算。</summary>
public static class GizmoDist
{
    public static double PointPoint(double x1, double y1, double x2, double y2) =>
        Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

    public static double PointSegment(double px, double py,
        double x1, double y1, double x2, double y2)
    {
        var dx = x2 - x1; var dy = y2 - y1;
        var lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-10) return PointPoint(px, py, x1, y1);
        var t = Math.Clamp(((px - x1) * dx + (py - y1) * dy) / lenSq, 0.0, 1.0);
        return PointPoint(px, py, x1 + t * dx, y1 + t * dy);
    }
}

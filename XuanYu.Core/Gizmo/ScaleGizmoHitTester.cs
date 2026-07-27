using XuanYu.Core.Math;

namespace XuanYu.Core.Gizmo;

public static class ScaleGizmoHitTester
{
    public const double HitMargin = 5.0;

    // CPU 命中布局与 Vulkan 绘制共用 ScaleGizmoLayout，保证“看见的位置 = 实际命中位置”。
    public static ScaleGizmoHandle? HitTest(
        ScaleGizmoLayout layout, double x, double y, double margin = HitMargin)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y)) return null;
        var candidates = new (ScaleGizmoHandle handle, double dist, double half)[]
        {
            (ScaleGizmoHandle.Uniform, DistToPoint(layout.Center, x, y), layout.CenterSizeDip / 2.0),
            (ScaleGizmoHandle.X, DistToSegment(layout.Center, layout.AxisEnd[0], x, y), layout.HandleSizeDip / 2.0),
            (ScaleGizmoHandle.Y, DistToSegment(layout.Center, layout.AxisEnd[1], x, y), layout.HandleSizeDip / 2.0),
            (ScaleGizmoHandle.Z, DistToSegment(layout.Center, layout.AxisEnd[2], x, y), layout.HandleSizeDip / 2.0),
        };
        ScaleGizmoHandle? best = null;
        var bestEff = double.MaxValue;
        foreach (var c in candidates)
        {
            var eff = c.dist - c.half;
            if (eff <= margin && eff < bestEff) { bestEff = eff; best = c.handle; }
        }
        return best;
    }

    static double DistToPoint(ScreenPoint p, double x, double y) =>
        System.Math.Sqrt((p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y));

    static double DistToSegment(ScreenPoint a, ScreenPoint b, double x, double y)
    {
        var dx = b.X - a.X; var dy = b.Y - a.Y;
        var len2 = dx * dx + dy * dy;
        double t = len2 <= 0 ? 0.0 : ((x - a.X) * dx + (y - a.Y) * dy) / len2;
        t = System.Math.Max(0.0, System.Math.Min(1.0, t));
        var px = a.X + t * dx; var py = a.Y + t * dy;
        return System.Math.Sqrt((px - x) * (px - x) + (py - y) * (py - y));
    }
}

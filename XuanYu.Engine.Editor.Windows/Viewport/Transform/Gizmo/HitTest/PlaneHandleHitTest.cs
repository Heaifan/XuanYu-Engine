namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo.HitTest;

/// <summary>
/// Plane 手柄的四边形命中测试。
/// 使用 PointInConvexQuad（三角形分解法）。
/// </summary>
public static class PlaneHandleHitTest
{
    public static bool Test(
        double mx, double my,
        double c0x, double c0y, double c1x, double c1y,
        double c2x, double c2y, double c3x, double c3y)
    {
        return PointInTriangle(mx, my, c0x, c0y, c1x, c1y, c2x, c2y) ||
               PointInTriangle(mx, my, c0x, c0y, c2x, c2y, c3x, c3y);
    }

    /// <summary>重心法判断点是否在三角形内。</summary>
    private static bool PointInTriangle(
        double px, double py,
        double ax, double ay, double bx, double by, double cx, double cy)
    {
        var d0 = (px - ax) * (by - ay) - (py - ay) * (bx - ax);
        var d1 = (px - bx) * (cy - by) - (py - by) * (cx - bx);
        var d2 = (px - cx) * (ay - cy) - (py - cy) * (ax - cx);
        var hasNeg = d0 < 0 || d1 < 0 || d2 < 0;
        var hasPos = d0 > 0 || d1 > 0 || d2 > 0;
        return !(hasNeg && hasPos);
    }
}

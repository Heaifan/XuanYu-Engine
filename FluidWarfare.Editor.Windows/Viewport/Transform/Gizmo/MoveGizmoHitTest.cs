namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的屏幕空间命中测试。
/// 使用 MoveGizmoLayout 数据，不单独计算。
/// </summary>
public static class MoveGizmoHitTest
{
    private const double AxisHitRadius = 8.0;
    private const double PlaneHitRadius = 12.0;

    /// <summary>返回鼠标下最近的 Gizmo 元素。无命中时返回 None。</summary>
    public static MoveGizmoElement HitTest(MoveGizmoLayout layout, double pointerX, double pointerY)
    {
        var bestElement = MoveGizmoElement.None;
        var bestDist = double.MaxValue;

        // 轴命中测试
        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX,
            layout.AxisDegenerateX, pointerX, pointerY,
            MoveGizmoElement.AxisX, AxisHitRadius, ref bestElement, ref bestDist);

        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisY, layout.AxisEndPixelY_AxisY,
            layout.AxisDegenerateY, pointerX, pointerY,
            MoveGizmoElement.AxisY, AxisHitRadius, ref bestElement, ref bestDist);

        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisZ, layout.AxisEndPixelY_AxisZ,
            layout.AxisDegenerateZ, pointerX, pointerY,
            MoveGizmoElement.AxisZ, AxisHitRadius, ref bestElement, ref bestDist);

        return bestElement;
    }

    private static void TestAxis(
        double px, double py, double ax, double ay, bool degenerate,
        double mx, double my, MoveGizmoElement element,
        double radius, ref MoveGizmoElement best, ref double bestDist)
    {
        if (degenerate) return;
        var d = DistToSegment(mx, my, px, py, ax, ay);
        if (d < radius && d < bestDist) { best = element; bestDist = d; }
    }

    private static double DistToSegment(double px, double py,
        double x1, double y1, double x2, double y2)
    {
        var dx = x2 - x1; var dy = y2 - y1;
        var lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-10) return Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
        var t = Math.Clamp(((px - x1) * dx + (py - y1) * dy) / lenSq, 0.0, 1.0);
        var cx = x1 + t * dx; var cy = y1 + t * dy;
        return Math.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
    }
}

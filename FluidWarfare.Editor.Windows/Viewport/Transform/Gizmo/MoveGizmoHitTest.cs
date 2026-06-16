namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的屏幕空间命中测试。
/// 使用 MoveGizmoLayout 数据，不单独计算。支持轴、平面块、中心手柄。
/// </summary>
public static class MoveGizmoHitTest
{
    private const double AxisHitRadius = 8.0;
    private const double PlaneHitRadius = 14.0;
    private const double CenterHitRadius = 10.0;

    /// <summary>返回鼠标下最近的 Gizmo 元素。无命中时返回 None。</summary>
    public static MoveGizmoElement HitTest(MoveGizmoLayout layout, double pointerX, double pointerY)
    {
        var best = MoveGizmoElement.None;
        var bestDist = double.MaxValue;

        // 轴命中测试
        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX,
            !layout.AxisDegenerateX, pointerX, pointerY,
            MoveGizmoElement.AxisX, AxisHitRadius, ref best, ref bestDist);

        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisY, layout.AxisEndPixelY_AxisY,
            !layout.AxisDegenerateY, pointerX, pointerY,
            MoveGizmoElement.AxisY, AxisHitRadius, ref best, ref bestDist);

        TestAxis(layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisZ, layout.AxisEndPixelY_AxisZ,
            !layout.AxisDegenerateZ, pointerX, pointerY,
            MoveGizmoElement.AxisZ, AxisHitRadius, ref best, ref bestDist);

        // 平面块命中测试
        TestPlane(layout.PlaneXY_Corner0X, layout.PlaneXY_Corner0Y,
            layout.PlaneXY_Corner1X, layout.PlaneXY_Corner1Y,
            layout.PlaneXY_Corner2X, layout.PlaneXY_Corner2Y,
            layout.PlaneXY_Corner3X, layout.PlaneXY_Corner3Y,
            !layout.AxisDegenerateX && !layout.AxisDegenerateY,
            pointerX, pointerY, MoveGizmoElement.PlaneXY,
            PlaneHitRadius, ref best, ref bestDist);

        TestPlane(layout.PlaneXZ_Corner0X, layout.PlaneXZ_Corner0Y,
            layout.PlaneXZ_Corner1X, layout.PlaneXZ_Corner1Y,
            layout.PlaneXZ_Corner2X, layout.PlaneXZ_Corner2Y,
            layout.PlaneXZ_Corner3X, layout.PlaneXZ_Corner3Y,
            !layout.AxisDegenerateX && !layout.AxisDegenerateZ,
            pointerX, pointerY, MoveGizmoElement.PlaneXZ,
            PlaneHitRadius, ref best, ref bestDist);

        TestPlane(layout.PlaneYZ_Corner0X, layout.PlaneYZ_Corner0Y,
            layout.PlaneYZ_Corner1X, layout.PlaneYZ_Corner1Y,
            layout.PlaneYZ_Corner2X, layout.PlaneYZ_Corner2Y,
            layout.PlaneYZ_Corner3X, layout.PlaneYZ_Corner3Y,
            !layout.AxisDegenerateY && !layout.AxisDegenerateZ,
            pointerX, pointerY, MoveGizmoElement.PlaneYZ,
            PlaneHitRadius, ref best, ref bestDist);

        // 中心手柄命中测试（兜底——只有在其他都没命中时才考虑）
        var distToCenter = Math.Sqrt(
            (pointerX - layout.PivotPixelX) * (pointerX - layout.PivotPixelX) +
            (pointerY - layout.PivotPixelY) * (pointerY - layout.PivotPixelY));
        if (distToCenter < CenterHitRadius && distToCenter < bestDist)
            best = MoveGizmoElement.ViewPlane;

        return best;
    }

    private static void TestAxis(
        double px, double py, double ax, double ay, bool active,
        double mx, double my, MoveGizmoElement element,
        double radius, ref MoveGizmoElement best, ref double bestDist)
    {
        if (!active) return;
        var d = DistToSegment(mx, my, px, py, ax, ay);
        if (d < radius && d < bestDist) { best = element; bestDist = d; }
    }

    private static void TestPlane(
        double c0x, double c0y, double c1x, double c1y,
        double c2x, double c2y, double c3x, double c3y,
        bool active, double mx, double my,
        MoveGizmoElement element, double radius,
        ref MoveGizmoElement best, ref double bestDist)
    {
        if (!active) return;
        var cx = (c0x + c1x + c2x + c3x) / 4;
        var cy = (c0y + c1y + c2y + c3y) / 4;
        var d = Math.Sqrt((mx - cx) * (mx - cx) + (my - cy) * (my - cy));
        if (d < radius && d < bestDist) { best = element; bestDist = d; }
    }

    private static double DistToSegment(double px, double py,
        double x1, double y1, double x2, double y2)
    {
        var dx = x2 - x1; var dy = y2 - y1;
        var lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-10) return Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
        var t = Math.Clamp(((px - x1) * dx + (py - y1) * dy) / lenSq, 0.0, 1.0);
        var cx2 = x1 + t * dx; var cy2 = y1 + t * dy;
        return Math.Sqrt((px - cx2) * (px - cx2) + (py - cy2) * (py - cy2));
    }
}

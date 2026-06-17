using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo.HitTest;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的屏幕空间命中测试。
/// 使用 MoveGizmoLayout 数据。轴命中 = 点到线段距离，平面 = 四边形命中。
/// 优先级：轴杆 → 箭头 → 平面块 → 中心手柄。
/// </summary>
public static class MoveGizmoHitTest
{
    private const double ShaftR = 4.0, ArrowR = 12.0, PlaneR = 16.0, CenterR = 12.0;

    public static MoveGizmoElement HitTest(MoveGizmoLayout l, double px, double py)
    {
        var best = MoveGizmoElement.None;
        var bestD = double.MaxValue;

        Shaft(l.AxisStartX_X, l.AxisStartY_X, l.AxisEndPixelX_AxisX, l.AxisEndPixelY_AxisX,
            !l.AxisDegenerateX, px, py, MoveGizmoElement.AxisX, ref best, ref bestD);
        Shaft(l.AxisStartX_Y, l.AxisStartY_Y, l.AxisEndPixelX_AxisY, l.AxisEndPixelY_AxisY,
            !l.AxisDegenerateY, px, py, MoveGizmoElement.AxisY, ref best, ref bestD);
        Shaft(l.AxisStartX_Z, l.AxisStartY_Z, l.AxisEndPixelX_AxisZ, l.AxisEndPixelY_AxisZ,
            !l.AxisDegenerateZ, px, py, MoveGizmoElement.AxisZ, ref best, ref bestD);

        Tip(l.AxisEndPixelX_AxisX, l.AxisEndPixelY_AxisX, !l.AxisDegenerateX,
            px, py, MoveGizmoElement.AxisX, ref best, ref bestD);
        Tip(l.AxisEndPixelX_AxisY, l.AxisEndPixelY_AxisY, !l.AxisDegenerateY,
            px, py, MoveGizmoElement.AxisY, ref best, ref bestD);
        Tip(l.AxisEndPixelX_AxisZ, l.AxisEndPixelY_AxisZ, !l.AxisDegenerateZ,
            px, py, MoveGizmoElement.AxisZ, ref best, ref bestD);

        Quad(Measure.PlaneXY_Corners(l), !l.AxisDegenerateX && !l.AxisDegenerateY,
            px, py, MoveGizmoElement.PlaneXY, ref best, ref bestD);
        Quad(Measure.PlaneXZ_Corners(l), !l.AxisDegenerateX && !l.AxisDegenerateZ,
            px, py, MoveGizmoElement.PlaneXZ, ref best, ref bestD);
        Quad(Measure.PlaneYZ_Corners(l), !l.AxisDegenerateY && !l.AxisDegenerateZ,
            px, py, MoveGizmoElement.PlaneYZ, ref best, ref bestD);

        var dc = GizmoDist.PointPoint(px, py, l.PivotPixelX, l.PivotPixelY);
        if (dc < CenterR && dc < bestD) best = MoveGizmoElement.ViewPlane;
        return best;
    }

    private static void Shaft(double sx, double sy, double ex, double ey,
        bool a, double mx, double my, MoveGizmoElement e,
        ref MoveGizmoElement b, ref double bd)
    {
        if (!a) return;
        var d = GizmoDist.PointSegment(mx, my, sx, sy, ex, ey);
        if (d < ShaftR && d < bd) { b = e; bd = d; }
    }

    private static void Tip(double ax, double ay, bool a,
        double mx, double my, MoveGizmoElement e,
        ref MoveGizmoElement b, ref double bd)
    {
        if (!a) return;
        var d = GizmoDist.PointPoint(mx, my, ax, ay);
        if (d < ArrowR && d < bd) { b = e; bd = d; }
    }

    private static void Quad(
        (double X0, double Y0, double X1, double Y1,
         double X2, double Y2, double X3, double Y3) c,
        bool a, double mx, double my, MoveGizmoElement e,
        ref MoveGizmoElement b, ref double bd)
    {
        if (!a) return;
        if (!PlaneHandleHitTest.Test(mx, my, c.X0, c.Y0, c.X1, c.Y1, c.X2, c.Y2, c.X3, c.Y3))
            return;
        var cx = (c.X0 + c.X1 + c.X2 + c.X3) / 4.0;
        var cy = (c.Y0 + c.Y1 + c.Y2 + c.Y3) / 4.0;
        var d = GizmoDist.PointPoint(mx, my, cx, cy);
        if (d < bd) { b = e; bd = d; }
    }
}

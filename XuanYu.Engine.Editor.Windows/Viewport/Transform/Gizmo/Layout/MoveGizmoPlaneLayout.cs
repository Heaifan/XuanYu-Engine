namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo.Layout;

/// <summary>
/// Plane 手柄的屏幕四边形顶点（四个角）。
/// 每个平面使用 Inner/Outer 双偏移量，确保离开中心手柄区域。
/// </summary>
public static class MoveGizmoPlaneLayout
{
    // ─── PlaneXY（X 轴与 Y 轴平面） ──────────────────────────

    public static double Corner0X(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneInner + l.AxisDirY_X * MoveGizmoLayout.PlaneInner;
    public static double Corner0Y(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneInner + l.AxisDirY_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner1X(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneOuter + l.AxisDirY_X * MoveGizmoLayout.PlaneInner;
    public static double Corner1Y(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirY_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner2X(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneOuter + l.AxisDirY_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner2Y(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirY_Y * MoveGizmoLayout.PlaneOuter;
    public static double Corner3X(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneInner + l.AxisDirY_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner3Y(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneInner + l.AxisDirY_Y * MoveGizmoLayout.PlaneOuter;

    // ─── PlaneXZ（X 轴与 Z 轴平面） ──────────────────────────

    public static double Corner0X_XZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneInner + l.AxisDirZ_X * MoveGizmoLayout.PlaneInner;
    public static double Corner0Y_XZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneInner + l.AxisDirZ_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner1X_XZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_X * MoveGizmoLayout.PlaneInner;
    public static double Corner1Y_XZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner2X_XZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner2Y_XZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_Y * MoveGizmoLayout.PlaneOuter;
    public static double Corner3X_XZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirX_X * MoveGizmoLayout.PlaneInner + l.AxisDirZ_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner3Y_XZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirX_Y * MoveGizmoLayout.PlaneInner + l.AxisDirZ_Y * MoveGizmoLayout.PlaneOuter;

    // ─── PlaneYZ（Y 轴与 Z 轴平面） ──────────────────────────

    public static double Corner0X_YZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirY_X * MoveGizmoLayout.PlaneInner + l.AxisDirZ_X * MoveGizmoLayout.PlaneInner;
    public static double Corner0Y_YZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirY_Y * MoveGizmoLayout.PlaneInner + l.AxisDirZ_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner1X_YZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirY_X * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_X * MoveGizmoLayout.PlaneInner;
    public static double Corner1Y_YZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirY_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_Y * MoveGizmoLayout.PlaneInner;
    public static double Corner2X_YZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirY_X * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner2Y_YZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirY_Y * MoveGizmoLayout.PlaneOuter + l.AxisDirZ_Y * MoveGizmoLayout.PlaneOuter;
    public static double Corner3X_YZ(MoveGizmoLayout l) =>
        l.PivotPixelX + l.AxisDirY_X * MoveGizmoLayout.PlaneInner + l.AxisDirZ_X * MoveGizmoLayout.PlaneOuter;
    public static double Corner3Y_YZ(MoveGizmoLayout l) =>
        l.PivotPixelY + l.AxisDirY_Y * MoveGizmoLayout.PlaneInner + l.AxisDirZ_Y * MoveGizmoLayout.PlaneOuter;
}

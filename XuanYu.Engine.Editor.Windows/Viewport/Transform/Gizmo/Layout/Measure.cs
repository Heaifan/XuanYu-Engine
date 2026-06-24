using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo.Layout;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// 从 MoveGizmoLayout 提取结构化的四边形角坐标，供 HitTest 和 DrawList 使用。
/// 保持单文件 ≤ 100 行。
/// </summary>
public static class Measure
{
    public static (double X0, double Y0, double X1, double Y1,
        double X2, double Y2, double X3, double Y3) PlaneXY_Corners(MoveGizmoLayout l) => (
        MoveGizmoPlaneLayout.Corner0X(l), MoveGizmoPlaneLayout.Corner0Y(l),
        MoveGizmoPlaneLayout.Corner1X(l), MoveGizmoPlaneLayout.Corner1Y(l),
        MoveGizmoPlaneLayout.Corner2X(l), MoveGizmoPlaneLayout.Corner2Y(l),
        MoveGizmoPlaneLayout.Corner3X(l), MoveGizmoPlaneLayout.Corner3Y(l));

    public static (double X0, double Y0, double X1, double Y1,
        double X2, double Y2, double X3, double Y3) PlaneXZ_Corners(MoveGizmoLayout l) => (
        MoveGizmoPlaneLayout.Corner0X_XZ(l), MoveGizmoPlaneLayout.Corner0Y_XZ(l),
        MoveGizmoPlaneLayout.Corner1X_XZ(l), MoveGizmoPlaneLayout.Corner1Y_XZ(l),
        MoveGizmoPlaneLayout.Corner2X_XZ(l), MoveGizmoPlaneLayout.Corner2Y_XZ(l),
        MoveGizmoPlaneLayout.Corner3X_XZ(l), MoveGizmoPlaneLayout.Corner3Y_XZ(l));

    public static (double X0, double Y0, double X1, double Y1,
        double X2, double Y2, double X3, double Y3) PlaneYZ_Corners(MoveGizmoLayout l) => (
        MoveGizmoPlaneLayout.Corner0X_YZ(l), MoveGizmoPlaneLayout.Corner0Y_YZ(l),
        MoveGizmoPlaneLayout.Corner1X_YZ(l), MoveGizmoPlaneLayout.Corner1Y_YZ(l),
        MoveGizmoPlaneLayout.Corner2X_YZ(l), MoveGizmoPlaneLayout.Corner2Y_YZ(l),
        MoveGizmoPlaneLayout.Corner3X_YZ(l), MoveGizmoPlaneLayout.Corner3Y_YZ(l));
}

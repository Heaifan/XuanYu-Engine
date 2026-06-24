using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo.Visual;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// 从 MoveGizmoLayout 生成屏幕空间三角形顶点列表。
/// 委托给 AxisVertices（轴杆 + 箭头）和 PlaneVertices（平面块 + 中心圆）。
/// 颜色：X=红, Y=绿, Z=蓝, Hover=黄, ViewPlane=白。
/// </summary>
public static class MoveGizmoDrawList
{
    /// <summary>返回 (X, Y, R, G, B, A) 顶点数组，用于 TriangleList 渲染。</summary>
    public static (float X, float Y, float R, float G, float B, float A)[] Build(
        MoveGizmoLayout layout, MoveGizmoVisualState state,
        MoveGizmoElement active, MoveGizmoElement hovered = MoveGizmoElement.None)
    {
        var verts = new List<(float, float, float, float, float, float)>();
        var alpha = state == MoveGizmoVisualState.Disabled ? 0.3f : 1f;

        // 三轴线 + 箭头
        MoveGizmoAxisVertices.AddAxis(verts, layout, MoveGizmoElement.AxisX, 1, 0, 0, active, hovered, alpha);
        MoveGizmoAxisVertices.AddAxis(verts, layout, MoveGizmoElement.AxisY, 0, 1, 0, active, hovered, alpha);
        MoveGizmoAxisVertices.AddAxis(verts, layout, MoveGizmoElement.AxisZ, 0, 0, 1, active, hovered, alpha);

        // 三个平面块（半透明填充四边形）
        MoveGizmoPlaneVertices.AddPlaneFill(verts, layout, Measure.PlaneXY_Corners(layout),
            0, 0, 1, MoveGizmoElement.PlaneXY, active, hovered, alpha);
        MoveGizmoPlaneVertices.AddPlaneFill(verts, layout, Measure.PlaneXZ_Corners(layout),
            0, 1, 0, MoveGizmoElement.PlaneXZ, active, hovered, alpha);
        MoveGizmoPlaneVertices.AddPlaneFill(verts, layout, Measure.PlaneYZ_Corners(layout),
            1, 0, 0, MoveGizmoElement.PlaneYZ, active, hovered, alpha);

        // 中心手柄（实心圆）
        MoveGizmoPlaneVertices.AddCenterCircle(verts,
            layout.PivotPixelX, layout.PivotPixelY, MoveGizmoLayout.CenterRadius,
            active, hovered, alpha);

        return verts.ToArray();
    }
}

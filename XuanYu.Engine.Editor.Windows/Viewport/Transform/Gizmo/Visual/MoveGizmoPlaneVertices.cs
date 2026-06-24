namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo.Visual;

/// <summary>
/// 生成 Gizmo 三个平面块和中心手柄的三角形顶点。
/// 平面块 = 2 个三角形（6 顶点），中心手柄 = 三角形扇。
/// </summary>
public static class MoveGizmoPlaneVertices
{
    private const int CircleSegments = 16;

    public static void AddPlaneFill(List<(float, float, float, float, float, float)> verts,
        MoveGizmoLayout layout, (double, double, double, double, double, double, double, double) corners,
        float r, float g, float b,
        MoveGizmoElement element, MoveGizmoElement active, MoveGizmoElement hovered, float alpha)
    {
        var (c0x, c0y, c1x, c1y, c2x, c2y, c3x, c3y) = corners;
        var highlighted = active == element || hovered == element;
        var (cr, cg, cb) = highlighted ? (1f, 1f, 0f) : (r, g, b);
        var fillAlpha = highlighted ? 0.55f : 0.22f;

        // 四边形 → 2 个三角形：ABC + ACD
        verts.Add(((float)c0x, (float)c0y, cr, cg, cb, fillAlpha));
        verts.Add(((float)c1x, (float)c1y, cr, cg, cb, fillAlpha));
        verts.Add(((float)c2x, (float)c2y, cr, cg, cb, fillAlpha));

        verts.Add(((float)c0x, (float)c0y, cr, cg, cb, fillAlpha));
        verts.Add(((float)c2x, (float)c2y, cr, cg, cb, fillAlpha));
        verts.Add(((float)c3x, (float)c3y, cr, cg, cb, fillAlpha));
    }

    public static void AddCenterCircle(List<(float, float, float, float, float, float)> verts,
        double cx, double cy, double radius,
        MoveGizmoElement active, MoveGizmoElement hovered, float alpha)
    {
        var isHi = active == MoveGizmoElement.ViewPlane || hovered == MoveGizmoElement.ViewPlane;
        var (r, g, b) = isHi ? (1f, 1f, 1f) : (0.75f, 0.75f, 0.78f);

        // 三角形扇：中心 → 圆周上每对相邻点
        for (var i = 0; i < CircleSegments; i++)
        {
            var a1 = i * 2 * Math.PI / CircleSegments;
            var a2 = (i + 1) * 2 * Math.PI / CircleSegments;
            verts.Add(((float)cx, (float)cy, r, g, b, alpha));
            verts.Add(((float)(cx + radius * Math.Cos(a1)), (float)(cy + radius * Math.Sin(a1)), r, g, b, alpha));
            verts.Add(((float)(cx + radius * Math.Cos(a2)), (float)(cy + radius * Math.Sin(a2)), r, g, b, alpha));
        }
    }
}

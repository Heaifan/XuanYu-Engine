namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// 从 MoveGizmoLayout 生成屏幕空间顶点列表供渲染层使用。
/// 每个轴由一条线段和一个三角形箭头组成。
/// 颜色：X=红, Y=绿, Z=蓝, Hover=黄。
/// </summary>
public static class MoveGizmoDrawList
{
    private const double ArrowSize = 6.0;

    public static (float X, float Y, float R, float G, float B, float A)[] Build(
        MoveGizmoLayout layout, MoveGizmoVisualState state, MoveGizmoElement element)
    {
        var verts = new List<(float, float, float, float, float, float)>();

        AddAxis(verts, layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX,
            layout.AxisDegenerateX, 1, 0, 0, element == MoveGizmoElement.AxisX, state);

        AddAxis(verts, layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisY, layout.AxisEndPixelY_AxisY,
            layout.AxisDegenerateY, 0, 1, 0, element == MoveGizmoElement.AxisY, state);

        AddAxis(verts, layout.PivotPixelX, layout.PivotPixelY,
            layout.AxisEndPixelX_AxisZ, layout.AxisEndPixelY_AxisZ,
            layout.AxisDegenerateZ, 0, 0, 1, element == MoveGizmoElement.AxisZ, state);

        return verts.ToArray();
    }

    private static void AddAxis(List<(float, float, float, float, float, float)> verts,
        double px, double py, double ax, double ay, bool degenerate,
        float r, float g, float b, bool highlighted, MoveGizmoVisualState state)
    {
        if (degenerate) return;

        var (cr, cg, cb) = highlighted ? (1f, 1f, 0f) : (r, g, b);
        var alpha = state == MoveGizmoVisualState.Disabled ? 0.3f : 1f;

        // Line from pivot to axis end
        verts.Add(((float)px, (float)py, cr, cg, cb, alpha));
        verts.Add(((float)ax, (float)ay, cr, cg, cb, alpha));

        // Arrowhead: two short lines at the end
        var dx = ax - px; var dy = ay - py;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1) return;

        var nx = dx / len; var ny = dy / len;
        var tipX = ax - nx * ArrowSize;
        var tipY = ay - ny * ArrowSize;
        var perpX = -ny * ArrowSize * 0.5;
        var perpY = nx * ArrowSize * 0.5;

        verts.Add(((float)ax, (float)ay, cr, cg, cb, alpha));
        verts.Add(((float)(tipX + perpX), (float)(tipY + perpY), cr, cg, cb, alpha));

        verts.Add(((float)ax, (float)ay, cr, cg, cb, alpha));
        verts.Add(((float)(tipX - perpX), (float)(tipY - perpY), cr, cg, cb, alpha));
    }
}

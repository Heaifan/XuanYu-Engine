namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo.Visual;

/// <summary>
/// 生成 Gizmo 三轴和箭头的三角形顶点。
/// 轴杆 = 2 个三角形（6 顶点），箭头 = 1 个三角形（3 顶点）。
/// </summary>
public static class MoveGizmoAxisVertices
{
    public static void AddAxis(List<(float, float, float, float, float, float)> verts,
        MoveGizmoLayout layout, MoveGizmoElement element,
        float r, float g, float b,
        MoveGizmoElement active, MoveGizmoElement hovered, float alpha)
    {
        var (sx, sy, ex, ey, isActive) = element switch
        {
            MoveGizmoElement.AxisX => (layout.AxisStartX_X, layout.AxisStartY_X,
                layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX, !layout.AxisDegenerateX),
            MoveGizmoElement.AxisY => (layout.AxisStartX_Y, layout.AxisStartY_Y,
                layout.AxisEndPixelX_AxisY, layout.AxisEndPixelY_AxisY, !layout.AxisDegenerateY),
            MoveGizmoElement.AxisZ => (layout.AxisStartX_Z, layout.AxisStartY_Z,
                layout.AxisEndPixelX_AxisZ, layout.AxisEndPixelY_AxisZ, !layout.AxisDegenerateZ),
            _ => (0.0, 0.0, 0.0, 0.0, false),
        };
        if (!isActive) return;

        var highlighted = active == element || hovered == element;
        var (cr, cg, cb) = highlighted ? (1f, 1f, 0f) : (r, g, b);
        var thickness = highlighted ? 5.0 : 3.0;

        // 轴杆：屏幕空间四边形 = 2 个三角形（6 顶点）
        var dx = ex - sx; var dy = ey - sy;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1.0) return;
        var nx = dx / len; var ny = dy / len;
        var perpX = -ny * thickness * 0.5;
        var perpY = nx * thickness * 0.5;

        // 三角形 1：p0→p1→p2
        verts.Add(((float)(sx + perpX), (float)(sy + perpY), cr, cg, cb, alpha));
        verts.Add(((float)(sx - perpX), (float)(sy - perpY), cr, cg, cb, alpha));
        verts.Add(((float)(ex + perpX), (float)(ey + perpY), cr, cg, cb, alpha));
        // 三角形 2：p1→p3→p2
        verts.Add(((float)(sx - perpX), (float)(sy - perpY), cr, cg, cb, alpha));
        verts.Add(((float)(ex - perpX), (float)(ey - perpY), cr, cg, cb, alpha));
        verts.Add(((float)(ex + perpX), (float)(ey + perpY), cr, cg, cb, alpha));

        // 箭头：实心三角形（3 顶点），从轴端往回移 ArrowLength
        var arrowLen = MoveGizmoLayout.ArrowLength;
        var arrowHalfW = arrowLen * 0.45;
        var tipX = ex - nx * arrowLen;
        var tipY = ey - ny * arrowLen;
        var apX = -ny * arrowHalfW;
        var apY = nx * arrowHalfW;

        verts.Add(((float)ex, (float)ey, cr, cg, cb, alpha));
        verts.Add(((float)(tipX + apX), (float)(tipY + apY), cr, cg, cb, alpha));
        verts.Add(((float)(tipX - apX), (float)(tipY - apY), cr, cg, cb, alpha));
    }
}

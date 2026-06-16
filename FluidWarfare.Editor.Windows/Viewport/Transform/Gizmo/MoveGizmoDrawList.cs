namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// 从 MoveGizmoLayout 生成屏幕空间顶点列表供渲染层使用。
/// 包括三轴线+箭头、三个平面块四边形、中心手柄圆。
/// 颜色：X=红, Y=绿, Z=蓝, Plane=半透, Hover=黄, ViewPlane=白。
/// </summary>
public static class MoveGizmoDrawList
{
    private const double ArrowSize = 6.0;
    private const int CircleSegments = 12;

    public static (float X, float Y, float R, float G, float B, float A)[] Build(
        MoveGizmoLayout layout, MoveGizmoVisualState state,
        MoveGizmoElement active, MoveGizmoElement hovered = MoveGizmoElement.None)
    {
        var verts = new List<(float, float, float, float, float, float)>();

        var alpha = state == MoveGizmoVisualState.Disabled ? 0.3f : 1f;

        // 三轴线 + 箭头
        AddAxis(verts, layout, MoveGizmoElement.AxisX, 1, 0, 0, active, hovered, alpha);
        AddAxis(verts, layout, MoveGizmoElement.AxisY, 0, 1, 0, active, hovered, alpha);
        AddAxis(verts, layout, MoveGizmoElement.AxisZ, 0, 0, 1, active, hovered, alpha);

        // 三个平面块（半透明四边形，用线框）
        var plAlpha = (active == MoveGizmoElement.PlaneXY ||
                       active == MoveGizmoElement.PlaneXZ ||
                       active == MoveGizmoElement.PlaneYZ ||
                       hovered == MoveGizmoElement.PlaneXY ||
                       hovered == MoveGizmoElement.PlaneXZ ||
                       hovered == MoveGizmoElement.PlaneYZ) ? 0.6f : 0.25f;

        AddPlaneQuad(verts, layout, MoveGizmoElement.PlaneXY, 0, 0, 1, active, hovered, plAlpha);
        AddPlaneQuad(verts, layout, MoveGizmoElement.PlaneXZ, 0, 1, 0, active, hovered, plAlpha);
        AddPlaneQuad(verts, layout, MoveGizmoElement.PlaneYZ, 1, 0, 0, active, hovered, plAlpha);

        // 中心手柄（小圆）
        var isCenterHi = active == MoveGizmoElement.ViewPlane || hovered == MoveGizmoElement.ViewPlane;
        var (cr, cg, cb) = isCenterHi ? (1f, 1f, 1f) : (0.7f, 0.7f, 0.7f);
        AddCircle(verts, layout.PivotPixelX, layout.PivotPixelY,
            layout.CenterHandleRadius, cr, cg, cb, alpha);

        return verts.ToArray();
    }

    private static void AddAxis(List<(float, float, float, float, float, float)> verts,
        MoveGizmoLayout layout, MoveGizmoElement element,
        float r, float g, float b,
        MoveGizmoElement active, MoveGizmoElement hovered, float alpha)
    {
        var (px, py, ax, ay, isActive) = element switch
        {
            MoveGizmoElement.AxisX => (layout.PivotPixelX, layout.PivotPixelY,
                layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX, !layout.AxisDegenerateX),
            MoveGizmoElement.AxisY => (layout.PivotPixelX, layout.PivotPixelY,
                layout.AxisEndPixelX_AxisY, layout.AxisEndPixelY_AxisY, !layout.AxisDegenerateY),
            MoveGizmoElement.AxisZ => (layout.PivotPixelX, layout.PivotPixelY,
                layout.AxisEndPixelX_AxisZ, layout.AxisEndPixelY_AxisZ, !layout.AxisDegenerateZ),
            _ => (0.0, 0.0, 0.0, 0.0, false),
        };
        if (!isActive) return;

        var highlighted = active == element || hovered == element;
        var (cr, cg, cb) = highlighted ? (1f, 1f, 0f) : (r, g, b);

        verts.Add(((float)px, (float)py, cr, cg, cb, alpha));
        verts.Add(((float)ax, (float)ay, cr, cg, cb, alpha));

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

    private static void AddPlaneQuad(List<(float, float, float, float, float, float)> verts,
        MoveGizmoLayout layout, MoveGizmoElement element,
        float r, float g, float b,
        MoveGizmoElement active, MoveGizmoElement hovered, float alpha)
    {
        var (c0x, c0y, c1x, c1y, c2x, c2y, c3x, c3y) = element switch
        {
            MoveGizmoElement.PlaneXY => (layout.PlaneXY_Corner0X, layout.PlaneXY_Corner0Y,
                layout.PlaneXY_Corner1X, layout.PlaneXY_Corner1Y,
                layout.PlaneXY_Corner2X, layout.PlaneXY_Corner2Y,
                layout.PlaneXY_Corner3X, layout.PlaneXY_Corner3Y),
            MoveGizmoElement.PlaneXZ => (layout.PlaneXZ_Corner0X, layout.PlaneXZ_Corner0Y,
                layout.PlaneXZ_Corner1X, layout.PlaneXZ_Corner1Y,
                layout.PlaneXZ_Corner2X, layout.PlaneXZ_Corner2Y,
                layout.PlaneXZ_Corner3X, layout.PlaneXZ_Corner3Y),
            MoveGizmoElement.PlaneYZ => (layout.PlaneYZ_Corner0X, layout.PlaneYZ_Corner0Y,
                layout.PlaneYZ_Corner1X, layout.PlaneYZ_Corner1Y,
                layout.PlaneYZ_Corner2X, layout.PlaneYZ_Corner2Y,
                layout.PlaneYZ_Corner3X, layout.PlaneYZ_Corner3Y),
            _ => (0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0),
        };

        var highlighted = active == element || hovered == element;
        var (cr, cg, cb) = highlighted ? (1f, 1f, 0f) : (r, g, b);

        verts.Add(((float)c0x, (float)c0y, cr, cg, cb, alpha));
        verts.Add(((float)c1x, (float)c1y, cr, cg, cb, alpha));
        verts.Add(((float)c1x, (float)c1y, cr, cg, cb, alpha));
        verts.Add(((float)c2x, (float)c2y, cr, cg, cb, alpha));
        verts.Add(((float)c2x, (float)c2y, cr, cg, cb, alpha));
        verts.Add(((float)c3x, (float)c3y, cr, cg, cb, alpha));
        verts.Add(((float)c3x, (float)c3y, cr, cg, cb, alpha));
        verts.Add(((float)c0x, (float)c0y, cr, cg, cb, alpha));
    }

    private static void AddCircle(List<(float, float, float, float, float, float)> verts,
        double cx, double cy, double radius, float r, float g, float b, float alpha)
    {
        for (var i = 0; i < CircleSegments; i++)
        {
            var a1 = i * 2 * Math.PI / CircleSegments;
            var a2 = (i + 1) * 2 * Math.PI / CircleSegments;
            verts.Add(((float)(cx + radius * Math.Cos(a1)), (float)(cy + radius * Math.Sin(a1)), r, g, b, alpha));
            verts.Add(((float)(cx + radius * Math.Cos(a2)), (float)(cy + radius * Math.Sin(a2)), r, g, b, alpha));
        }
    }
}

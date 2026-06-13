using FluidWarfare.Core.Math;
using FluidWarfare.Render.Camera;

namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>
/// 视口导航 Overlay 布局计算。
/// 平台无关：输入视口尺寸和相机姿态，输出 Gizmo/按钮的像素坐标。
/// 绘制和 HitTest 必须使用同一份布局结果。
/// </summary>
public sealed record ViewportNavigationLayout
{
    // ─── 布局参数 ──────────────────────────────────────────────
    /// <summary>右上边距。</summary>
    public const float MarginRight = 14f;
    /// <summary>上边距。</summary>
    public const float MarginTop = 14f;

    /// <summary>Gizmo 区域尺寸。</summary>
    public const float GizmoSize = 96f;
    /// <summary>中心球半径。</summary>
    public const float CenterRadius = 10f;
    /// <summary>前端轴端半径。</summary>
    public const float FrontAxisRadius = 9f;
    /// <summary>后端轴端半径。</summary>
    public const float BackAxisRadius = 6f;
    /// <summary>轴线长度。</summary>
    public const float AxisLength = 30f;

    /// <summary>导航按钮尺寸。</summary>
    public const float ButtonSize = 28f;
    /// <summary>按钮间距。</summary>
    public const float ButtonSpacing = 6f;

    /// <summary>视口缩小阈值。</summary>
    public const float MinViewportWidth = 320f;
    public const float MinViewportHeight = 240f;

    // ─── 计算结果 ──────────────────────────────────────────────

    /// <summary>视口宽度。</summary>
    public int ViewportWidth { get; init; }

    /// <summary>视口高度。</summary>
    public int ViewportHeight { get; init; }

    /// <summary>统一缩放因子（缩小视口时使用）。</summary>
    public float Scale { get; init; } = 1f;

    /// <summary>Gizmo 中心像素 X。</summary>
    public float GizmoCenterX { get; init; }

    /// <summary>Gizmo 中心像素 Y。</summary>
    public float GizmoCenterY { get; init; }

    /// <summary>导航按钮起始 X。</summary>
    public float ButtonAreaX { get; init; }

    /// <summary>导航按钮起始 Y。</summary>
    public float ButtonAreaY { get; init; }

    // ─── 六个轴端的屏幕投影 ──────────────────────────────────

    /// <summary>每个轴端的投影信息。</summary>
    public IReadOnlyList<AxisProjection> AxisProjections { get; init; }
        = Array.Empty<AxisProjection>();

    // ─── 按钮矩形 ──────────────────────────────────────────────

    /// <summary>缩放按钮矩形。</summary>
    public Rect ZoomButtonRect { get; init; }

    /// <summary>平移按钮矩形。</summary>
    public Rect PanButtonRect { get; init; }

    /// <summary>聚焦按钮矩形。</summary>
    public Rect FrameButtonRect { get; init; }

    /// <summary>投影按钮矩形。</summary>
    public Rect ProjectionButtonRect { get; init; }

    /// <summary>Gizmo 中心圆区域（拖动检测）。</summary>
    public Circle GizmoCenterCircle { get; init; }

    // ─── 类型 ──────────────────────────────────────────────────

    /// <summary>轴端在屏幕上的投影。</summary>
    public sealed record AxisProjection(
        ViewportNavigationElement Element,
        float ScreenX,
        float ScreenY,
        float Depth,
        float Radius,
        (float R, float G, float B) Color);

    /// <summary>矩形区域。</summary>
    public sealed record Rect(float X, float Y, float W, float H);

    /// <summary>圆形区域。</summary>
    public sealed record Circle(float CenterX, float CenterY, float Radius);

    // ─── 工厂方法 ──────────────────────────────────────────────

    /// <summary>
    /// 从视口尺寸和相机姿态计算布局。
    /// </summary>
    public static ViewportNavigationLayout Compute(
        int viewportWidth, int viewportHeight,
        SceneCameraPose cameraPose)
    {
        // 视口过小时缩放
        var scale = 1f;
        if (viewportWidth < MinViewportWidth || viewportHeight < MinViewportHeight)
        {
            var sx = viewportWidth / MinViewportWidth;
            var sy = viewportHeight / MinViewportHeight;
            scale = Math.Min(sx, sy);
        }

        var gizmoX = viewportWidth - MarginRight * scale - GizmoSize * scale * 0.5f;
        var gizmoY = MarginTop * scale + GizmoSize * scale * 0.5f;

        // 计算轴端投影
        var projections = ComputeAxisProjections(cameraPose, gizmoX, gizmoY, scale);

        // 按钮区
        var btnX = viewportWidth - MarginRight * scale - ButtonSize * scale;
        var btnY = gizmoY + GizmoSize * scale * 0.5f + ButtonSpacing * scale;
        var btnS = ButtonSize * scale;
        var spacing = ButtonSpacing * scale;

        return new ViewportNavigationLayout
        {
            ViewportWidth = viewportWidth,
            ViewportHeight = viewportHeight,
            Scale = scale,
            GizmoCenterX = gizmoX,
            GizmoCenterY = gizmoY,
            ButtonAreaX = btnX,
            ButtonAreaY = btnY,
            AxisProjections = projections,
            GizmoCenterCircle = new Circle(gizmoX, gizmoY, CenterRadius * scale),
            ZoomButtonRect = new Rect(btnX, btnY, btnS, btnS),
            PanButtonRect = new Rect(btnX, btnY + (btnS + spacing), btnS, btnS),
            FrameButtonRect = new Rect(btnX, btnY + 2 * (btnS + spacing), btnS, btnS),
            ProjectionButtonRect = new Rect(btnX, btnY + 3 * (btnS + spacing), btnS, btnS),
        };
    }

    private static IReadOnlyList<AxisProjection> ComputeAxisProjections(
        SceneCameraPose cameraPose,
        float gizmoCenterX, float gizmoCenterY,
        float scale)
    {
        // 相机方向向量
        var fwdX = cameraPose.TargetX - cameraPose.PositionX;
        var fwdY = cameraPose.TargetY - cameraPose.PositionY;
        var fwdZ = cameraPose.TargetZ - cameraPose.PositionZ;
        var fwdLen = (float)Math.Sqrt(fwdX * fwdX + fwdY * fwdY + fwdZ * fwdZ);
        if (fwdLen < 1e-10f) fwdLen = 1f;
        fwdX /= fwdLen; fwdY /= fwdLen; fwdZ /= fwdLen;

        // WorldUp = (0, 0, 1)
        // Right = normalize(Forward × WorldUp)
        var rightX = fwdY * 1 - fwdZ * 0;
        var rightY = fwdZ * 0 - fwdX * 1;
        var rightZ = fwdX * 0 - fwdY * 0;
        var rightLen = (float)Math.Sqrt(rightX * rightX + rightY * rightY);
        if (rightLen > 1e-10f) { rightX /= rightLen; rightY /= rightLen; }

        // ViewUp = normalize(Right × Forward)
        var vupX = rightY * fwdZ - rightZ * fwdY;
        var vupY = rightZ * fwdX - rightX * fwdZ;
        var vupZ = rightX * fwdY - rightY * fwdX;
        var vupLen = (float)Math.Sqrt(vupX * vupX + vupY * vupY + vupZ * vupZ);
        if (vupLen > 1e-10f) { vupX /= vupLen; vupY /= vupLen; vupZ /= vupLen; }

        // 六个世界轴
        var worldAxes = new[]
        {
            (ViewportNavigationElement.PositiveX, 1f, 0f, 0f, 1f, 0f, 0f),
            (ViewportNavigationElement.NegativeX, -1f, 0f, 0f, 0.5f, 0f, 0f),
            (ViewportNavigationElement.PositiveY, 0f, 1f, 0f, 0f, 1f, 0f),
            (ViewportNavigationElement.NegativeY, 0f, -1f, 0f, 0f, 0.5f, 0f),
            (ViewportNavigationElement.PositiveZ, 0f, 0f, 1f, 0f, 0f, 1f),
            (ViewportNavigationElement.NegativeZ, 0f, 0f, -1f, 0.3f, 0.3f, 1f),
        };

        var axisLen = AxisLength * scale;
        var frontR = FrontAxisRadius * scale;
        var backR = BackAxisRadius * scale;

        var results = new List<AxisProjection>(6);
        foreach (var (elem, ax, ay, az, cr, cg, cb) in worldAxes)
        {
            // 投影到屏幕
            var sx = ax * rightX + ay * rightY + az * rightZ;
            var sy = -(ax * vupX + ay * vupY + az * vupZ);
            var depth = ax * fwdX + ay * fwdY + az * fwdZ;

            var px = gizmoCenterX + sx * axisLen;
            var py = gizmoCenterY + sy * axisLen;
            var radius = depth > 0 ? frontR : backR;

            results.Add(new AxisProjection(elem, px, py, depth, radius, (cr, cg, cb)));
        }

        // 深度排序：背面先绘制，正面后绘制
        results.Sort((a, b) => a.Depth.CompareTo(b.Depth));

        return results;
    }

    /// <summary>
    /// HitTest：根据像素坐标检测命中的导航元素。
    /// </summary>
    public ViewportNavigationElement HitTest(float pixelX, float pixelY)
    {
        // 1. 轴端（精确匹配，半径内最近者优先）
        // 从未命中不可能超过两个轴端（正/负背对），先检测所有轴端
        foreach (var proj in AxisProjections)
        {
            var dx = pixelX - proj.ScreenX;
            var dy = pixelY - proj.ScreenY;
            if (dx * dx + dy * dy <= proj.Radius * proj.Radius)
                return proj.Element;
        }

        // 2. Gizmo 中心区域
        var gdx = pixelX - GizmoCenterCircle.CenterX;
        var gdy = pixelY - GizmoCenterCircle.CenterY;
        if (gdx * gdx + gdy * gdy <= GizmoCenterCircle.Radius * GizmoCenterCircle.Radius)
            return ViewportNavigationElement.GizmoCenter;

        // 3. 导航按钮
        if (IsInRect(pixelX, pixelY, ZoomButtonRect)) return ViewportNavigationElement.ZoomButton;
        if (IsInRect(pixelX, pixelY, PanButtonRect)) return ViewportNavigationElement.PanButton;
        if (IsInRect(pixelX, pixelY, FrameButtonRect)) return ViewportNavigationElement.FrameButton;
        if (IsInRect(pixelX, pixelY, ProjectionButtonRect)) return ViewportNavigationElement.ProjectionButton;

        return ViewportNavigationElement.None;
    }

    /// <summary>将 HitTest 结果转换为导航动作。</summary>
    public ViewportNavigationAction ElementToAction(ViewportNavigationElement element)
    {
        return element switch
        {
            ViewportNavigationElement.PositiveX => ViewportNavigationAction.SnapPositiveX,
            ViewportNavigationElement.NegativeX => ViewportNavigationAction.SnapNegativeX,
            ViewportNavigationElement.PositiveY => ViewportNavigationAction.SnapPositiveY,
            ViewportNavigationElement.NegativeY => ViewportNavigationAction.SnapNegativeY,
            ViewportNavigationElement.PositiveZ => ViewportNavigationAction.SnapPositiveZ,
            ViewportNavigationElement.NegativeZ => ViewportNavigationAction.SnapNegativeZ,
            ViewportNavigationElement.GizmoCenter => ViewportNavigationAction.Orbit,
            ViewportNavigationElement.ZoomButton => ViewportNavigationAction.Zoom,
            ViewportNavigationElement.PanButton => ViewportNavigationAction.Pan,
            ViewportNavigationElement.FrameButton => ViewportNavigationAction.Frame,
            ViewportNavigationElement.ProjectionButton => ViewportNavigationAction.ToggleProjection,
            _ => ViewportNavigationAction.None
        };
    }

    private static bool IsInRect(float px, float py, Rect r) =>
        px >= r.X && px <= r.X + r.W && py >= r.Y && py <= r.Y + r.H;
}

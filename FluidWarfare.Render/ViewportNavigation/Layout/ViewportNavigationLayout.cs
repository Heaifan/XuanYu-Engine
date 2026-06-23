namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>
/// 视口导航 Overlay 布局计算结果。
/// 平台无关：由 ViewportNavigationLayoutCompute.Compute 工厂创建。
/// 绘制和 HitTest 共用此布局。
/// </summary>
public sealed record ViewportNavigationLayout
{
    // ─── 布局参数 ──────────────────────────────────────────────
    public const float MarginRight = 18f;
    public const float MarginTop = 16f;
    public const float GizmoSize = 104f;
    public const float CenterRadius = 12f;
    public const float FrontAxisRadius = 10f;
    public const float BackAxisRadius = 7f;
    public const float AxisLength = 34f;
    public const float GizmoOrbitRadius = AxisLength + FrontAxisRadius + 8f;
    public const float ButtonSize = 30f;
    public const float ButtonSpacing = 6f;
    public const float MinViewportWidth = 320f;
    public const float MinViewportHeight = 240f;

    // ─── 计算结果 ──────────────────────────────────────────────
    public int ViewportWidth { get; init; }
    public int ViewportHeight { get; init; }
    public float Scale { get; init; } = 1f;
    public float GizmoCenterX { get; init; }
    public float GizmoCenterY { get; init; }
    public float ButtonAreaX { get; init; }
    public float ButtonAreaY { get; init; }
    public IReadOnlyList<AxisProjection> AxisProjections { get; init; } = Array.Empty<AxisProjection>();
    public Rect PanButtonRect { get; init; }
    public Rect FrameButtonRect { get; init; }
    public Rect ProjectionButtonRect { get; init; }
    public Circle GizmoCenterCircle { get; init; }
    public Circle GizmoOrbitCircle { get; init; }
}

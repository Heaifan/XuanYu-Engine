namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>
/// 可交互的导航元素标识。
/// </summary>
public enum ViewportNavigationElement
{
    /// <summary>未命中任何 Overlay 元素。</summary>
    None,

    // ─── Gizmo 轴端 ────────────────────────────────────────
    /// <summary>+X 轴端（红）。</summary>
    PositiveX,
    /// <summary>-X 轴端（暗红）。</summary>
    NegativeX,
    /// <summary>+Y 轴端（绿）。</summary>
    PositiveY,
    /// <summary>-Y 轴端（暗绿）。</summary>
    NegativeY,
    /// <summary>+Z 轴端（蓝）。</summary>
    PositiveZ,
    /// <summary>-Z 轴端（暗蓝）。</summary>
    NegativeZ,

    // ─── Gizmo 区域 ─────────────────────────────────────────
    /// <summary>导航球中心拖动区域。</summary>
    GizmoCenter,

    // ─── 导航按钮 ──────────────────────────────────────────
    /// <summary>缩放按钮。</summary>
    ZoomButton,
    /// <summary>平移按钮。</summary>
    PanButton,
    /// <summary>聚焦按钮。</summary>
    FrameButton,
    /// <summary>投影模式切换按钮。</summary>
    ProjectionButton
}

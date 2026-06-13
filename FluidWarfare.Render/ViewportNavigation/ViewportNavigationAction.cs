namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>
/// Overlay 交互触发的相机动作。
/// </summary>
public enum ViewportNavigationAction
{
    /// <summary>无动作。</summary>
    None,

    /// <summary>环绕旋转（Gizmo 拖动）。</summary>
    Orbit,

    /// <summary>跳转到 +X 标准视图。</summary>
    SnapPositiveX,

    /// <summary>跳转到 -X 标准视图。</summary>
    SnapNegativeX,

    /// <summary>跳转到 +Y 标准视图。</summary>
    SnapPositiveY,

    /// <summary>跳转到 -Y 标准视图。</summary>
    SnapNegativeY,

    /// <summary>跳转到 +Z（顶视图）。</summary>
    SnapPositiveZ,

    /// <summary>跳转到 -Z（底视图）。</summary>
    SnapNegativeZ,

    /// <summary>平移拖动。</summary>
    Pan,

    /// <summary>缩放拖动。</summary>
    Zoom,

    /// <summary>聚焦所选或查看全部。</summary>
    Frame,

    /// <summary>切换透视/正交。</summary>
    ToggleProjection
}

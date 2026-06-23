namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>
/// 视口导航 Overlay 对左键按下的处理结果。
/// NativeHost 依据结果决定是否阻断世界 Picking，以及是否捕获鼠标。
/// </summary>
public enum ViewportNavigationPressResult
{
    /// <summary>未命中 Overlay，继续世界实体/地面 Picking。</summary>
    NotHandled,

    /// <summary>命中一次性操作，不进入拖动，但阻断世界 Picking。</summary>
    HandledClick,

    /// <summary>命中拖动操作，阻断世界 Picking 并捕获鼠标。</summary>
    BeginDrag
}

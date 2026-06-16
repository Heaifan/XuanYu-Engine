namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

/// <summary>
/// 场景工具（移动、旋转等）对鼠标左键按下的响应结果。
/// 在 WndProc 中同步判断，用于仲裁 Overlay 导航→场景工具→遗留 Picking 的输入消费顺序。
/// </summary>
public enum ViewportSceneToolPressResult
{
    /// <summary>场景工具不处理此按下事件，事件继续传递给遗留 Picking。</summary>
    NotHandled,

    /// <summary>场景工具开始拖拽，消费此按下事件。WndProc 会捕获鼠标并阻断遗留 Picking。</summary>
    BeginDrag,
}

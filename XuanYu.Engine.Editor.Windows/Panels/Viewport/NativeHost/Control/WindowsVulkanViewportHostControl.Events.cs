using XuanYu.Engine.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

partial class WindowsVulkanViewportHostControl
{
    // ─── 原始输入事件 ─────────────────────────────────────────
    public event Action<int, int, int>? RawPointerButtonDown;
    public event Action<int, int, int>? RawPointerButtonUp;
    public event Action<int, int>? RawPointerMoved;
    public event Action<int>? RawKeyDown;
    public event Action<int>? RawKeyUp;
    public event Action<int, int>? RawMouseWheel;
    public event Action? RawInputFocusLost;

    // ─── Overlay 导航输入事件 ─────────────────────────────────
    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;
    public event Func<int, int, bool>? NavigationPointerMoved;
    public event Action? NavigationPointerReleased;
    public event Action? NavigationCaptureLost;

    // ─── 遗留事件 ──────────────────────────────────────────
    public event Action<int, int>? PickRequested;
    public new event Action<int, int>? PointerMoved;
    public event Action? PointerLeft;

    // ─── 场景工具输入事件 ────────────────────────────────────
    public event Func<int, int, ViewportSceneToolPressResult>? SceneToolPointerPressed;
    public event Action<int, int>? SceneToolPointerReleased;
    public event EventHandler<WindowsVulkanViewportHostInfo>? HostInfoChanged;
}

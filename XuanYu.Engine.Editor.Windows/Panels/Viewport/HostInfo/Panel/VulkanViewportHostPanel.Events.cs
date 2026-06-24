using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>Partial：原生窗口事件转发接线。</summary>
sealed partial class VulkanViewportHostPanel
{
    partial void WireNativeHostEvents(WindowsVulkanViewportHostControl ctl)
    {
        ctl.HostInfoChanged += HandleHostInfoChanged;
        ctl.RawPointerButtonDown += (b, x, y) => RawPointerButtonDown?.Invoke(b, x, y);
        ctl.RawPointerButtonUp += (b, x, y) => RawPointerButtonUp?.Invoke(b, x, y);
        ctl.RawPointerMoved += (x, y) => RawPointerMoved?.Invoke(x, y);
        ctl.RawKeyDown += vk => RawKeyDown?.Invoke(vk);
        ctl.RawKeyUp += vk => RawKeyUp?.Invoke(vk);
        ctl.RawMouseWheel += (d, m) => RawMouseWheel?.Invoke(d, m);
        ctl.RawInputFocusLost += () => RawInputFocusLost?.Invoke();
        ctl.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
        ctl.SceneToolPointerPressed += (x, y) => SceneToolPointerPressed?.Invoke(x, y) ?? ViewportSceneToolPressResult.NotHandled;
        ctl.SceneToolPointerReleased += (x, y) => SceneToolPointerReleased?.Invoke(x, y);
        ctl.NavigationPointerPressed += (x, y) => NavigationPointerPressed?.Invoke(x, y) ?? ViewportNavigationPressResult.NotHandled;
        ctl.NavigationPointerMoved += (x, y) => NavigationPointerMoved?.Invoke(x, y) == true;
        ctl.NavigationPointerReleased += () => NavigationPointerReleased?.Invoke();
        ctl.NavigationCaptureLost += () => NavigationCaptureLost?.Invoke();
        ctl.PointerMoved += (x, y) => PointerMoved?.Invoke(x, y);
        ctl.PointerLeft += () => PointerLeft?.Invoke();
    }
}

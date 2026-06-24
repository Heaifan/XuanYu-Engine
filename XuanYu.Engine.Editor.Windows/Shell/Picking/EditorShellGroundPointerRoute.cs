using Avalonia.Threading;
using XuanYu.Engine.Editor.ViewportGround;
using XuanYu.Engine.Editor.Windows.Panels.Status;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Input.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Render.ViewportNavigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Picking;

/// <summary>地面指针移动路由。负责视口地面 hover/指针移动事件转发及调度合并。</summary>
sealed class EditorShellGroundPointerRoute(
    Scene3dSessionLifecycle lifecycle,
    VulkanViewportHostPanel? vhPanel,
    EditorGroundHoverInputRoute groundHoverRoute,
    EditorGroundPointerState groundPointerState,
    ViewportNavigationRoute navigationRoute,
    StatusBarPanel? statusBarPanel,
    EditorSelectionRoute selectionRoute,
    Func<ViewportNavigationElement, ViewportNavigationElement, bool> applyOverlayVisualState,
    Action<VulkanScene3dFrameReason> scheduleFrame)
{
    bool _groundPointerUpdatePending;
    long _lastGroundPointerUpdateTicks;

    /// <summary>
    /// 鼠标在视口内移动 → 地面射线求交 → 状态栏反馈。
    /// 采用"最新值覆盖 + 单次调度"合并模式，最多约每 16ms 更新一次。
    /// </summary>
    public void HandleViewportPointerMoved(int pixelX, int pixelY)
    {
        if (lifecycle.State.Session?.Status != VulkanScene3dSessionStatus.Active) return;
        var nh = vhPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nh.HasNativeHandle || nh.Width < 1 || nh.Height < 1) return;

        if (_groundPointerUpdatePending) { _lastGroundPointerUpdateTicks = (pixelX << 16) | (pixelY & 0xFFFF); return; }
        _groundPointerUpdatePending = true;
        var cx = pixelX; var cy = pixelY;
        Dispatcher.UIThread.Post(() =>
        {
            _groundPointerUpdatePending = false;
            if (_lastGroundPointerUpdateTicks != 0)
            {
                cx = (int)(_lastGroundPointerUpdateTicks >> 16);
                cy = (int)(_lastGroundPointerUpdateTicks & 0xFFFF);
                _lastGroundPointerUpdateTicks = 0;
            }
            var host = vhPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
            groundHoverRoute.HandlePointerMoved(new(cx, cy, lifecycle, groundPointerState, navigationRoute,
                msg => { if (statusBarPanel is not null) statusBarPanel.SetGroundPosition(msg); },
                msg => { if (statusBarPanel is not null) statusBarPanel.SetCurrentSelection(msg); }), host);
        }, DispatcherPriority.Background);
    }

    /// <summary>鼠标离开视口 → 清除地面坐标显示。</summary>
    public void HandleViewportPointerLeft()
    {
        var nav = groundHoverRoute.HandlePointerLeft(new(0, 0, lifecycle, groundPointerState, navigationRoute,
            msg => { if (statusBarPanel is not null) statusBarPanel.SetGroundPosition(msg); },
            msg => { if (statusBarPanel is not null) statusBarPanel.SetCurrentSelection(msg); }),
            selectionRoute);
        if (nav.VisualStateChanged)
            applyOverlayVisualState(nav.Hovered, nav.Active);
    }
}

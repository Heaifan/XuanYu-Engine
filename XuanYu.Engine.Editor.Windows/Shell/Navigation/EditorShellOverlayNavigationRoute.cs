using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.ViewportNavigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Navigation;

/// <summary>Overlay 导航事件路由。负责 Overlay 视口导航区的交互转发。</summary>
sealed class EditorShellOverlayNavigationRoute(
    Scene3dSessionLifecycle lifecycle,
    VulkanViewportHostPanel? vhPanel,
    ViewportNavigationRoute navigationRoute,
    ViewportCameraRoute cameraRoute,
    Action<VulkanScene3dFrameReason> scheduleFrame)
{
    public ViewportNavigationLayout? GetPresentedNavigationLayout()
    {
        if (lifecycle.State.Session is null || vhPanel is null)
            return null;

        var snapshot = lifecycle.State.Session.LastPresentedOverlaySnapshot;
        if (!snapshot.IsAvailable || snapshot.Layout is null)
            return null;

        var host = vhPanel.GetNativeHostInfo();
        if (host.Width != snapshot.ViewportWidth || host.Height != snapshot.ViewportHeight)
            return null;

        return snapshot.Layout;
    }

    public bool ApplyOverlayVisualState(ViewportNavigationElement hovered, ViewportNavigationElement active)
    {
        if (lifecycle.State.Session?.SetNavigationOverlayState(hovered, active) == true)
        {
            scheduleFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
            return true;
        }
        return false;
    }

    public ViewportNavigationPressResult HandleOverlayPointerPressed(int pixelX, int pixelY)
    {
        var layout = GetPresentedNavigationLayout();
        if (layout is null) return ViewportNavigationPressResult.NotHandled;

        var response = navigationRoute.Press(pixelX, pixelY, layout);
        if (response.Result == ViewportNavigationPressResult.NotHandled)
            return response.Result;

        ApplyOverlayVisualState(response.Element, response.Element);

        if (response.CameraCommand is not null)
        {
            var camResult = cameraRoute.Apply(response.CameraCommand);
            if (camResult.StateChanged && camResult.NeedsFrame)
                scheduleFrame(camResult.Reason);
            ApplyOverlayVisualState(response.Element, ViewportNavigationElement.None);
            navigationRoute.Release(false);
        }

        return response.Result;
    }

    public bool HandleOverlayPointerMoved(int pixelX, int pixelY)
    {
        var layout = GetPresentedNavigationLayout();
        var vhFallback = vhPanel?.GetNativeHostInfo().Height ?? 1;
        var response = navigationRoute.Move(pixelX, pixelY, layout, cameraRoute, vhFallback);
        if (response.VisualStateChanged)
            ApplyOverlayVisualState(response.Hovered, response.Active);
        if (response.NeedsFrame)
            scheduleFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
        return response.Handled;
    }

    public void HandleOverlayPointerReleased()
    {
        var r = navigationRoute.Release(true);
        if (r.NeedsCleanupFrame)
            ApplyOverlayVisualState(navigationRoute.HoverElement, ViewportNavigationElement.None);
    }

    public void HandleOverlayCaptureLost()
    {
        var r = navigationRoute.Release(true);
        if (r.NeedsCleanupFrame)
            ApplyOverlayVisualState(navigationRoute.HoverElement, ViewportNavigationElement.None);
    }
}

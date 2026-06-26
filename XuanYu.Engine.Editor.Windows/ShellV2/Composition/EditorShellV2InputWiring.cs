using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Render.ViewportNavigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using static XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer.NativeViewportPointerMessages;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>EditorShellV2 输入事件接线。复用已封版的 Route 对象，V2 重新 wiring。</summary>
static class EditorShellV2InputWiring
{
    static bool _isOrbiting, _isPanning;
    static int _lastX, _lastY;

    public static void Wire(EditorShellV2Context ctx)
    {
        var panel = ctx.ViewportPanel;
        var schedule = ctx.FrameScheduler!;
        if (panel is null) return;

        // ─── 中键导航 ────────────────────────────────────
        panel.RawPointerButtonDown += (btn, x, y) =>
        {
            _lastX = x; _lastY = y;
            if (btn == VkMButton && !_isPanning) _isOrbiting = true;
            if (btn == VkMButton && _isPanning) { /* Shift+中键平移由 RawKey 标记 */ }
        };
        panel.RawPointerMoved += (x, y) =>
        {
            var dx = x - _lastX; var dy = y - _lastY;
            _lastX = x; _lastY = y;
            if (_isOrbiting) ApplyOrbit(dx, dy, ctx.Camera, schedule);
            if (_isPanning) ApplyPan(dx, dy, ctx.Camera, schedule);
            ctx.Pointer?.OnPointerMoved(x, y);
        };
        panel.RawPointerButtonUp += (btn, _, _) =>
        {
            if (btn == VkMButton && _isOrbiting) _isOrbiting = false;
            if (_isPanning) _isPanning = false;
        };
        panel.RawMouseWheel += (d, _) =>
        {
            var r = ctx.Camera.Apply(new ViewportCameraCommand.Zoom(d));
            if (r.NeedsFrame) schedule(r.Reason);
        };

        // ─── 键盘 ────────────────────────────────────────
        panel.RawKeyDown += vk =>
        {
            if (vk == 0x10) { _isPanning = !_isOrbiting; _isOrbiting = false; return; } // Shift
            ctx.Pointer?.OnPointerMoved(_lastX, _lastY);
        };
        panel.RawKeyUp += vk => { if (vk == 0x10) _isPanning = false; };

        // ─── Overlay 导航（预留：V2 暂不显示导航叠加层）───
        panel.NavigationPointerPressed += (_, _) => ViewportNavigationPressResult.NotHandled;
        panel.NavigationPointerMoved += (_, _) => false;
        panel.NavigationPointerReleased += () => { };
        panel.NavigationCaptureLost += () => { };

        // ─── SceneTool（Gizmo 预留：9.1A-3 接入）────────
        panel.SceneToolPointerPressed += (_, _) => ViewportSceneToolPressResult.NotHandled;
        panel.SceneToolPointerReleased += (_, _) => { };

        // ─── Picking（预留：9.1A-3 接入）────────────────
        panel.PickRequested += (_, _) => { };
        panel.RawInputFocusLost += () => { _isOrbiting = false; _isPanning = false; };
        panel.PointerLeft += () => { };
    }

    static void ApplyOrbit(int dx, int dy, ViewportCameraRoute camera, Action<VulkanScene3dFrameReason> schedule)
    {
        var r = camera.Apply(new ViewportCameraCommand.Orbit(-dx, -dy));
        if (r.NeedsFrame) schedule(r.Reason);
    }

    static void ApplyPan(int dx, int dy, ViewportCameraRoute camera, Action<VulkanScene3dFrameReason> schedule)
    {
        var r = camera.Apply(new ViewportCameraCommand.Pan(dx, dy, 1));
        if (r.NeedsFrame) schedule(r.Reason);
    }
}

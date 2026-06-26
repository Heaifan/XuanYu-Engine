using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Render.ViewportNavigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using static XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer.NativeViewportPointerMessages;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>EditorShellV2 输入事件接线。使用实例状态机，不共享 static 状态。</summary>
static class EditorShellV2InputWiring
{
    public static void Wire(EditorShellV2Context ctx)
    {
        var panel = ctx.ViewportPanel;
        var schedule = ctx.FrameScheduler!;
        if (panel is null) return;

        var s = new EditorShellV2InputState();

        panel.RawPointerButtonDown += (btn, x, y) =>
        {
            if (btn != VkMButton) return;
            s.OnMiddleDown(x, y);
        };
        panel.RawPointerMoved += (x, y) =>
        {
            // Gizmo drag preview 优先于相机导航
            if (ctx.Pointer.IsDragActive)
            {
                var r = ctx.Pointer.OnPointerMoved(x, y);
                if (r.Action == TransformInteractionAction.Previewed)
                    schedule(VulkanScene3dFrameReason.TransformPreview);
                return;
            }
            var dx = x - s.LastX; var dy = y - s.LastY;
            s.OnMove(x, y);
            if (s.IsOrbiting) ApplyOrbit(dx, dy, ctx.Camera, schedule);
            if (s.IsPanning) ApplyPan(dx, dy, ctx.Camera, schedule);
        };
        panel.RawPointerButtonUp += (btn, _, _) =>
        {
            if (btn == VkMButton) s.OnMiddleUp();
        };
        panel.RawKeyDown += vk =>
        {
            if (vk == 0x10) { s.OnShiftDown(); return; }
            if (vk == 0x1B && ctx.Pointer.IsDragActive) // Esc
            {
                ctx.Pointer.Cancel(TransformInteractionReason.Escape);
                schedule(VulkanScene3dFrameReason.TransformPreview);
                panel.RequestCancelToolCapture();
                return;
            }
        };
        panel.RawKeyUp += vk =>
        {
            if (vk == 0x10) s.OnShiftUp();
        };
        panel.RawMouseWheel += (d, _) =>
        {
            var r = ctx.Camera.Apply(new ViewportCameraCommand.Zoom(d));
            if (r.NeedsFrame) schedule(r.Reason);
        };
        panel.RawInputFocusLost += () => s.OnFocusLost();
        panel.PointerLeft += () => { };

        // Navigation overlay — 在专用 Wiring 文件中接入
        panel.NavigationPointerPressed += (_, _) => ViewportNavigationPressResult.NotHandled;
        panel.NavigationPointerMoved += (_, _) => false;
        panel.NavigationPointerReleased += () => { };
        panel.NavigationCaptureLost += () => { };
    }

    static void ApplyOrbit(int dx, int dy, ViewportCameraRoute camera, Action<VulkanScene3dFrameReason> s)
    {
        var r = camera.Apply(new ViewportCameraCommand.Orbit(-dx, -dy));
        if (r.NeedsFrame) s(r.Reason);
    }

    static void ApplyPan(int dx, int dy, ViewportCameraRoute camera, Action<VulkanScene3dFrameReason> s)
    {
        var r = camera.Apply(new ViewportCameraCommand.Pan(dx, dy, 1));
        if (r.NeedsFrame) s(r.Reason);
    }
}

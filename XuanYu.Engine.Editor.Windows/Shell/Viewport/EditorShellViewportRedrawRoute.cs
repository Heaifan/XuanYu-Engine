using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

namespace XuanYu.Engine.Editor.Windows.Shell.Viewport;

/// <summary>Viewport 重绘路由。负责 Vulkan redraw 调度、resize 结果应用。</summary>
sealed class EditorShellViewportRedrawRoute(
    EditorStartupVulkanRoute startupVulkanRoute,
    VulkanViewportProbeRoute probeRoute,
    VulkanViewportHostPanel? vhPanel,
    Func<bool> isSessionActive,
    Scene3dSessionLifecycle lifecycle,
    ViewportCameraRoute cameraRoute,
    ViewportRenderSceneStore renderSceneStore,
    Func<int> getRenderSeq,
    Scene3dResizeRenderRoute resizeRenderRoute,
    Action<string> appendInfoLog,
    Action<string> appendWarningLog,
    EditorDiagnosticsRefreshRoute diagnosticsRoute,
    Action refreshDiagnostics,
    Action runStartupProbe,
    Action<bool> setSessionActive,
    Action<int> setRenderSeq,
    Action<string> setRenderLastMode)
{
    DispatcherTimer? _viewportResizeRenderTimer;

    public DispatcherTimer? Timer => _viewportResizeRenderTimer;
    public EventHandler TimerTickHandler => HandleViewportResizeRenderTimerTick;

    public void StopTimer() => _viewportResizeRenderTimer?.Stop();
    public void ClearTimer() => _viewportResizeRenderTimer = null;

    public void HandleNativeHostInfoChanged(object? sender, VulkanViewportNativeHostInfo nativeHostInfo)
    {
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        if (!startupVulkanRoute.State.NativeHostReported)
        {
            GizmoDragProbe.Log("Dispatcher.UIThread.Post 入队(StartupProbe)");
            Dispatcher.UIThread.Post(runStartupProbe);
            return;
        }

        ScheduleVulkanViewportRedraw();
    }

    public void ScheduleVulkanViewportRedraw()
    {
        _viewportResizeRenderTimer ??= new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(180)
        };

        _viewportResizeRenderTimer.Tick -= HandleViewportResizeRenderTimerTick;
        _viewportResizeRenderTimer.Tick += HandleViewportResizeRenderTimerTick;
        _viewportResizeRenderTimer.Stop();
        _viewportResizeRenderTimer.Start();
    }

    void HandleViewportResizeRenderTimerTick(object? sender, EventArgs e)
    {
        _viewportResizeRenderTimer?.Stop();
        RedrawVulkanViewportOnce();
    }

    void RedrawVulkanViewportOnce()
    {
        var request = new Scene3dResizeRenderRequest(
            probeRoute.State.Backend.IsAvailable, probeRoute.State.Device.IsCreated,
            vhPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            isSessionActive(), lifecycle.State.Session,
            cameraRoute, renderSceneStore.Current, getRenderSeq());

        var result = resizeRenderRoute.RenderOnce(
            request, lifecycle, probeRoute, appendInfoLog, appendWarningLog);

        ApplyResizeRenderResult(result);
    }

    void ApplyResizeRenderResult(Scene3dResizeRenderResult result)
    {
        var seq = diagnosticsRoute.ApplyResizeResult(result, appendInfoLog, appendWarningLog);
        if (seq > 0) setRenderSeq(seq);
        if (result.Action == Scene3dResizeAction.ClearFallbackAfterFailure) setSessionActive(false);
        if (result.Action is Scene3dResizeAction.ClearFallback or Scene3dResizeAction.ClearFallbackAfterFailure)
        { if (probeRoute.State.Clear.IsSucceeded) { setRenderLastMode("Clear"); refreshDiagnostics(); } }
    }
}

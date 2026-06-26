using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>EditorShellV2 组合根。创建 V2 所需的最小路由集，不引用旧 Shell 面板。</summary>
static class EditorShellV2Composition
{
    public static EditorShellV2Context Build(EditorShellV2 shell)
    {
        var ctx = new EditorShellV2Context();
        ctx.ViewportPanel = shell.FindControl<VulkanViewportHostPanel>("VulkanViewportHostPanelV2");

        ctx.ProbeRoute = new VulkanViewportProbeRoute();
        ctx.ProbeRoute.State.Scene3d = new(0, ctx.ProbeRoute.State.Gate.Message, 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, ctx.ProbeRoute.State.Gate.CanRun ? "可用" : "不可用", 0);
        ctx.StartupVulkan = new EditorStartupVulkanRoute();
        ctx.RenderStore = new ViewportRenderSceneStore();
        ctx.Lifecycle = new Scene3dSessionLifecycle(ctx.RenderStore);
        ctx.CameraRoute = new ViewportCameraRoute();
        ctx.ResizeRender = new Scene3dResizeRenderRoute();
        ctx.Diagnostics = new EditorDiagnosticsRefreshRoute();

        if (ctx.ViewportPanel is not null)
        {
            ctx.ViewportPanel.NativeHostInfoChanged += (_, info) =>
            {
                if (info.Width < 1 || info.Height < 1) return;
                if (!ctx.StartupVulkan.State.NativeHostReported)
                { Dispatcher.UIThread.Post(() => RunStartupProbe(ctx)); return; }
                ScheduleRender(ctx);
            };
            // 空事件桩：为后续 9.1A-2 输入接入预留
            ctx.ViewportPanel.RawPointerButtonDown += (_, _, _) => { };
            ctx.ViewportPanel.RawPointerMoved += (_, _) => { };
            ctx.ViewportPanel.RawPointerButtonUp += (_, _, _) => { };
            ctx.ViewportPanel.RawKeyDown += _ => { };
            ctx.ViewportPanel.RawKeyUp += _ => { };
            ctx.ViewportPanel.RawMouseWheel += (_, _) => { };
            ctx.ViewportPanel.RawInputFocusLost += () => { };
        }
        return ctx;
    }

    static void RunStartupProbe(EditorShellV2Context ctx)
    {
        var req = new EditorStartupVulkanRequest(
            ctx.ProbeRoute, ctx.Lifecycle, ctx.RenderStore,
            () => ctx.ViewportPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            _ => { }, _ => { }, () => { }, () => { });

        ctx.StartupVulkan.RunConstructProbes(req);
        ctx.Diagnostics.ProbeValidation(_ => { }, _ => { });
        ScheduleRender(ctx);
    }

    static void ScheduleRender(EditorShellV2Context ctx)
    {
        var timer = ctx.RenderTimer;
        if (timer is null)
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(180) };
            timer.Tick += (_, _) => RenderOnce(ctx);
            ctx.RenderTimer = timer;
        }
        timer.Stop();
        timer.Start();
    }

    static void RenderOnce(EditorShellV2Context ctx)
    {
        ctx.RenderTimer?.Stop();

        var probe = ctx.ProbeRoute;
        if (!probe.State.Backend.IsAvailable || !probe.State.Device.IsCreated) return;

        var hostInfo = ctx.ViewportPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
        var request = new Scene3dResizeRenderRequest(
            probe.State.Backend.IsAvailable, probe.State.Device.IsCreated,
            hostInfo, ctx.SessionActive, ctx.Lifecycle.State.Session,
            ctx.CameraRoute, ctx.RenderStore.Current, ctx.RenderSeq);

        var result = ctx.ResizeRender.RenderOnce(request, ctx.Lifecycle, probe, _ => { }, _ => { });

        var seq = ctx.Diagnostics.ApplyResizeResult(result, _ => { }, _ => { });
        if (seq > 0) ctx.RenderSeq = seq;
        if (result.Action == Scene3dResizeAction.ClearFallbackAfterFailure) ctx.SessionActive = false;
        if (result.Action is Scene3dResizeAction.ClearFallback or Scene3dResizeAction.ClearFallbackAfterFailure)
        { if (probe.State.Clear.IsSucceeded) ctx.RenderLastMode = "Clear"; }
    }
}

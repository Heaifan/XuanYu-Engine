using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

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
        ctx.Diagnostics = new EditorDiagnosticsRefreshRoute();
        ctx.ResizeRender = new Scene3dResizeRenderRoute();

        // ─── 路由创建（子文件） ───────────────────────────
        EditorShellV2Routes.Create(ctx);

        // ─── 帧调度回调：高频路径只刷新 Viewport，不触发诊断 ──
        ctx.FrameScheduler = reason =>
        {
            ScheduleRender(ctx);
            if (reason != VulkanScene3dFrameReason.TransformPreview
                && reason != VulkanScene3dFrameReason.CameraPan
                && reason != VulkanScene3dFrameReason.CameraZoom)
                ctx.Diagnostics.ProbeValidation(_ => { }, _ => { });
        };

        // ─── 事件接线：NativeHost → 渲染 ────────────────
        if (ctx.ViewportPanel is not null)
        {
            ctx.ViewportPanel.NativeHostInfoChanged += (_, info) =>
            {
                if (info.Width < 1 || info.Height < 1) return;
                if (!ctx.StartupVulkan.State.NativeHostReported)
                { Dispatcher.UIThread.Post(() => RunStartupProbe(ctx)); return; }
                ScheduleRender(ctx);
            };
        }

        // ─── 输入事件接线（子文件） ──────────────────────
        EditorShellV2InputWiring.Wire(ctx);
        EditorShellV2PickingWiring.Wire(ctx);
        EditorShellV2SceneToolWiring.Wire(ctx);

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
        ctx.RenderTimer ??= new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(180) };
        ctx.RenderTimer.Tick -= HandleTick;
        ctx.RenderTimer.Tick += HandleTick;
        ctx.RenderTimer.Stop();
        ctx.RenderTimer.Start();

        void HandleTick(object? _, EventArgs __) => RenderOnce(ctx);
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
            ctx.Camera, ctx.RenderStore.Current, ctx.RenderSeq);

        var result = ctx.ResizeRender.RenderOnce(request, ctx.Lifecycle, probe, _ => { }, _ => { });
        var seq = ctx.Diagnostics.ApplyResizeResult(result, _ => { }, _ => { });
        if (seq > 0) ctx.RenderSeq = seq;
        if (result.Action == Scene3dResizeAction.ClearFallbackAfterFailure) ctx.SessionActive = false;
    }
}

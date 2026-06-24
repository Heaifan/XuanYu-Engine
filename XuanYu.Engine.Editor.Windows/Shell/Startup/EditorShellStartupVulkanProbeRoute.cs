using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Shell.Diagnostics;
using FluidWarfare.Editor.Windows.Shell.Scene3D.Commands;
using FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;

namespace FluidWarfare.Editor.Windows.Shell.Startup;

/// <summary>Startup Vulkan Probe 路由。负责构建启动探测请求、执行转发、结果应用。</summary>
sealed class EditorShellStartupVulkanProbeRoute(
    VulkanViewportProbeRoute probeRoute,
    EditorStartupVulkanRoute startupVulkanRoute,
    Scene3dSessionLifecycle lifecycle,
    ViewportRenderSceneStore renderSceneStore,
    VulkanViewportHostPanel? vhPanel,
    Action<string> appendInfoLog,
    Action<string> appendWarningLog,
    Action refreshDiagnostics,
    Action requestScene3dRestart,
    EditorDiagnosticsRefreshRoute diagnosticsRoute)
{
    public void RunStartupVulkanProbe()
    {
        ApplyResult(startupVulkanRoute.RunConstructProbes(BuildRequest()));
    }

    public void RunAttachedProbes()
    {
        ApplyResult(startupVulkanRoute.TryRunAttachProbes(BuildRequest()));
    }

    public void ProbeVulkanValidation() =>
        diagnosticsRoute.ProbeValidation(appendInfoLog, appendWarningLog);

    EditorStartupVulkanRequest BuildRequest() => new(
        ProbeRoute: probeRoute,
        Lifecycle: lifecycle,
        RenderSceneStore: renderSceneStore,
        GetNativeHostInfo: () => vhPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
        InfoLog: appendInfoLog,
        WarnLog: appendWarningLog,
        RefreshDiagnostics: refreshDiagnostics,
        RequestScene3dStart: requestScene3dRestart);

    void ApplyResult(EditorStartupVulkanResult result)
    {
        if (result.DiagnosticsRefreshRequested)
            refreshDiagnostics();
    }
}

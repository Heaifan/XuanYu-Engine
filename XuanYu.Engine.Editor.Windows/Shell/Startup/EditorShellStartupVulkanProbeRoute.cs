using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Shell.Scene3D.Commands;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

namespace XuanYu.Engine.Editor.Windows.Shell.Startup;

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

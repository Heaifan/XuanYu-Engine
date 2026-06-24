using FluidWarfare.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Render.Vulkan.Clear;
using XuanYu.Engine.Render.Vulkan.Swapchain;

namespace FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;

/// <summary>
/// Vulkan 启动探测编排路由。构造期 Backend→Instance→Device→Surface 链 + 附加期 NativeHost→Swapchain→Clear→AutoStart 链。
/// 不持有 Shell/UI 面板。输出通过 Request 委托和 Result 返回。
/// </summary>
public sealed class EditorStartupVulkanRoute
{
    private readonly EditorStartupVulkanState _state = new();

    public EditorStartupVulkanState State => _state;

    public EditorStartupVulkanResult RunConstructProbes(EditorStartupVulkanRequest request)
    {
        RunConstructChain(request);
        return new(DiagnosticsRefreshRequested: true, Scene3dStartRequested: false);
    }

    public EditorStartupVulkanResult TryRunAttachProbes(EditorStartupVulkanRequest request)
    {
        if (_state.NativeHostReported)
            return new(DiagnosticsRefreshRequested: false, Scene3dStartRequested: false);
        RunAttachChain(request);
        return new(DiagnosticsRefreshRequested: true, Scene3dStartRequested: _state.Scene3dAutoStartAttempted);
    }

    public void Reset() { _state.NativeHostReported = false; _state.Scene3dAutoStartAttempted = false; }

    private void RunConstructChain(EditorStartupVulkanRequest request)
    {
        var p = request.ProbeRoute;
        p.ProbeBackend(request.InfoLog, request.WarnLog);
        p.ProbeInstance(request.InfoLog, request.WarnLog);
        p.ProbeDevice(request.InfoLog, request.WarnLog);
        p.ProbeSurface(request.GetNativeHostInfo(), request.InfoLog, request.WarnLog);
    }

    private void RunAttachChain(EditorStartupVulkanRequest request)
    {
        _state.NativeHostReported = true;
        var host = request.GetNativeHostInfo();
        if (!host.HasNativeHandle)
        {
            request.WarnLog(host.Message);
            request.ProbeRoute.ProbeSurface(host, request.InfoLog, request.WarnLog);
            return;
        }
        request.InfoLog($"Windows Vulkan 视口子窗口已创建，HWND：0x{host.WindowHandle.ToInt64():X16}。");
        RunSwapchainProbe(request, host);
        RunClearProbe(request, host, "初始启动");
        request.InfoLog(request.ProbeRoute.State.Gate.Message);
        TryAutoStartScene3d(request);
    }

    private void RunSwapchainProbe(EditorStartupVulkanRequest request, VulkanViewportNativeHostInfo host)
    {
        if (!TryGetValidSize(host, out var w, out var h, out var msg))
        { request.ProbeRoute.State.Swapchain = new(VulkanSwapchainStatus.Failed, msg, 0, "未知", "未知", 0, 0, 0); return; }
        request.ProbeRoute.ProbeSwapchain(host, w, h, request.InfoLog, request.WarnLog);
    }

    private void RunClearProbe(EditorStartupVulkanRequest request, VulkanViewportNativeHostInfo host, string reason)
    {
        if (!TryGetValidSize(host, out var w, out var h, out var msg))
        { request.ProbeRoute.State.Clear = new(VulkanClearStatus.Failed, msg, "未知", 0, 0, 0); return; }
        request.InfoLog($"Clear | {w}x{h} | {reason}");
        request.ProbeRoute.ProbeClear(host, w, h, reason, request.InfoLog, request.WarnLog);
    }

    private void TryAutoStartScene3d(EditorStartupVulkanRequest request)
    {
        if (_state.Scene3dAutoStartAttempted) return;
        _state.Scene3dAutoStartAttempted = true;
        if (!request.ProbeRoute.State.Gate.CanRun)
        { request.WarnLog($"Scene3D 自动启动跳过：{request.ProbeRoute.State.Gate.Message}"); return; }
        if (request.Lifecycle.State.Session is not null)
        { request.InfoLog("Scene3D 会话已存在，跳过自动启动。"); return; }
        if (request.RenderSceneStore.Current.Objects.Count == 0)
        { request.WarnLog("Scene3D 自动启动跳过：RenderScene 为空。"); return; }
        request.InfoLog("Scene3D 自动启动...");
        request.RequestScene3dStart();
    }

    private static bool TryGetValidSize(VulkanViewportNativeHostInfo host, out uint w, out uint h, out string msg)
    {
        w = 0; h = 0;
        if (host.Width < 1 || host.Height < 1) { msg = "Vulkan 视口尺寸尚未就绪，跳过本次绘制。"; return false; }
        w = checked((uint)host.Width); h = checked((uint)host.Height); msg = string.Empty; return true;
    }
}

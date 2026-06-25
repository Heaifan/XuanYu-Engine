using XuanYu.Engine.Core.Logging;
using XuanYu.Engine.Editor.Windows.Panels.DebugDock;
using XuanYu.Engine.Editor.Windows.Panels.Status;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Feedback;

public sealed class EditorFeedbackRoute
{
    private DebugDockPanel? _debugDock;
    private StatusBarPanel? _statusBar;
    private VulkanViewportHostPanel? _vulkanViewportHost;

    public void Attach(DebugDockPanel? debugDock, StatusBarPanel? statusBar, VulkanViewportHostPanel? vpHost)
    { _debugDock = debugDock; _statusBar = statusBar; _vulkanViewportHost = vpHost; }

    public void Info(string msg) => Log(EngineLogLevel.Info, msg);
    public void Warn(string msg) => Log(EngineLogLevel.Warning, msg);
    public void Error(string msg) => Log(EngineLogLevel.Error, msg);
    private void Log(EngineLogLevel l, string m)
    {
        GizmoDragProbe.MarkUiRefreshed();
        GizmoDragProbe.Log("日志面板刷新");
        _debugDock?.LogPanel?.AppendLogMessage(EngineLogEntry.Create(0.0, l, "Editor", m).ToDisplayString());
    }

    public void SetStartupLogs() => _debugDock?.LogPanel?.SetLogMessages([
        EngineLogEntry.Create(0.0, EngineLogLevel.Info, "Editor", "XuanYu Engine Editor 启动完成。").ToDisplayString(),
        EngineLogEntry.Create(0.0, EngineLogLevel.Info, "Core", "Core 基础模块已加载。").ToDisplayString()]);

    public void SetSelection(string? t) => _statusBar?.SetCurrentSelection(t ?? "无");
    public void SetVulkanStatus(string t) => _statusBar?.SetVulkanStatus(t);
    public void SetGroundPos(string? t) => _statusBar?.SetGroundPosition(t ?? "地面坐标：无");

    public void RefreshViewportStatusLine(bool active, Scene3dSessionState ls, VulkanViewportProbeState ps, string lastMode)
    {
        var sess = ls.Session;
        var isActive = active && sess is not null && sess.Status == VulkanScene3dSessionStatus.Active;
        var s3d = isActive ? $" | Scene3D Active | Frame #{sess!.FrameIndex}" : ps.Gate.CanRun ? " | Scene3D Ready" : " | Scene3D 已隔离";
        _vulkanViewportHost?.ShowClearStatus(ps.Clear.IsSucceeded
            ? $"Vulkan Clear 稳定{s3d} | 最近渲染：{lastMode} | {ps.Clear.ClearColorText}"
            : $"清屏：{ps.Clear.Message} | 最近渲染：{lastMode}");
    }

    public void RefreshAllDiagnostics(VulkanViewportProbeState ps, VulkanViewportNativeHostInfo nh,
        IReadOnlyList<RenderObjectInfo> objects, bool s3dOk, string s3dMsg, string s3dCam,
        int gridVtx, int gridLine, int unitVtx, int unitTri, int renderedUnit, int renderObjCnt, int ignoredObjCnt,
        int drawCalls, string depthFmt, int depthAttach, bool depthTest,
        double instMs, double devMs, double swapMs, double clearMs, double s3dMs)
    {
        var nhMsg = nh.HasNativeHandle ? $"已获取独立子窗口 HWND，尺寸：{nh.Width}x{nh.Height}" : "未获取";
        _debugDock?.SetDiagnostics(
            ps.Backend.Message,
            Fmt(ps.Instance.IsCreated, $"创建成功，API 版本：{ps.Instance.ApiVersionText}，扩展数量：{ps.Instance.ExtensionCount}，用时：{ps.Instance.ElapsedMilliseconds:F2} ms", ps.Instance.Message),
            Fmt(ps.Device.IsCreated, $"创建成功，显卡：{ps.Device.PhysicalDeviceName}，类型：{ps.Device.PhysicalDeviceTypeText}，队列族：{ps.Device.GraphicsQueueFamilyIndex}，用时：{ps.Device.ElapsedMilliseconds:F2} ms", ps.Device.Message),
            nhMsg,
            Fmt(ps.Surface.IsCreated, $"创建成功，平台：{ps.Surface.PlatformText}，用时：{ps.Surface.ElapsedMilliseconds:F2} ms", ps.Surface.Message),
            Fmt(ps.Swapchain.IsCreated, $"创建成功，图像：{ps.Swapchain.ImageCount}，格式：{ps.Swapchain.SurfaceFormatText}，Present：{ps.Swapchain.PresentModeText}，尺寸：{ps.Swapchain.Width}x{ps.Swapchain.Height}，用时：{ps.Swapchain.ElapsedMilliseconds:F2} ms", ps.Swapchain.Message),
            Fmt(ps.Clear.IsSucceeded, $"成功，{ps.Clear.ClearColorText}，尺寸：{ps.Clear.Width}x{ps.Clear.Height}，用时：{ps.Clear.ElapsedMilliseconds:F2} ms", ps.Clear.Message),
            "已退役（MarkerDraw 路径在 8.3.1 移除）",
            Fmt(ps.Validation.IsEnabled, $"已启用，消息 {ps.Validation.MessageCount} 条", ps.Validation.Message));
        _debugDock?.SetScene3d(s3dOk ? "成功" : s3dMsg, s3dCam,
            s3dOk ? $"{gridVtx} 顶点/{gridLine} 线段" : "-",
            s3dOk ? $"{unitVtx} 顶点/{unitTri} 三角形 | 渲染 {renderedUnit}/{renderObjCnt} | 忽略 {ignoredObjCnt}" : "-",
            s3dOk ? $"{drawCalls} | Depth {depthFmt} x{depthAttach} {(depthTest ? "已启用" : "未启用")}" : "-");
        _debugDock?.SetPerformance(instMs.ToString("F2"), devMs.ToString("F2"), swapMs.ToString("F2"),
            clearMs.ToString("F2"), "-", s3dOk ? s3dMs.ToString("F2") : "-");
        var entries = objects.Select(o => $"{o.DisplayName} | unit_marker | ({o.Position.X}, {o.Position.Y}, {o.Position.Z}) | {o.SourcePath ?? "无"}").ToList();
        _debugDock?.SetRenderSceneSummary(
            objects.Count > 0 ? $"RenderScene 调试对象（共 {objects.Count} 个）" : "RenderScene 调试对象", entries);
    }

    private static string Fmt(bool ok, string okMsg, string failMsg) => ok ? okMsg : failMsg;
}

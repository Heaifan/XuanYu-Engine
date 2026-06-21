using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;

namespace FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;

/// <summary>Shell → VulkanStartupRoute 的请求。携带 Route 执行探测链所需的所有外部依赖。</summary>
public sealed record EditorStartupVulkanRequest(
    /// <summary>Vulkan 探测路由实例（ProbeBackend / ProbeInstance / ProbeDevice 等）。</summary>
    VulkanViewportProbeRoute ProbeRoute,
    /// <summary>Scene3D 会话生命周期（用于检查会话状态和自动启动判定）。</summary>
    Scene3dSessionLifecycle Lifecycle,
    /// <summary>RenderScene 存储（用于检查渲染对象数量）。</summary>
    ViewportRenderSceneStore RenderSceneStore,
    /// <summary>获取当前 NativeHost 信息快照的委托。</summary>
    Func<VulkanViewportNativeHostInfo> GetNativeHostInfo,
    /// <summary>普通信息日志。</summary>
    Action<string> InfoLog,
    /// <summary>警告日志。</summary>
    Action<string> WarnLog,
    /// <summary>请求 Shell 刷新诊断面板。</summary>
    Action RefreshDiagnostics,
    /// <summary>请求 Shell 启动 Scene3D 会话。</summary>
    Action RequestScene3dStart);

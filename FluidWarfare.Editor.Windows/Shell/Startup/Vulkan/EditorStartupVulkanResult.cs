namespace FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;

/// <summary>VulkanStartupRoute → Shell 的结果。Shell 据此应用 UI 更新。</summary>
public sealed record EditorStartupVulkanResult(
    /// <summary>是否需要刷新诊断面板。</summary>
    bool DiagnosticsRefreshRequested,
    /// <summary>是否请求启动 Scene3D 会话。</summary>
    bool Scene3dStartRequested);

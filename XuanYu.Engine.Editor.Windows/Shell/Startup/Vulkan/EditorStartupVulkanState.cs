namespace FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;

/// <summary>Vulkan 启动探测的状态标志。由 Route 内部管理，Shell 通过 Result 和属性查询。</summary>
public sealed class EditorStartupVulkanState
{
    /// <summary>NativeHost 首次报告是否已完成（防止重复 Dispatcher.Post 和重复报告）。</summary>
    public bool NativeHostReported { get; set; }

    /// <summary>Scene3D 自动启动是否已尝试（防止重复尝试启动会话）。</summary>
    public bool Scene3dAutoStartAttempted { get; set; }
}

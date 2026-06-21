namespace FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;

/// <summary>Vulkan 启动探测流程中的步骤标识。用于日志和内部状态追踪。</summary>
public enum EditorStartupVulkanStep
{
    Backend,
    Instance,
    Device,
    Surface,
    Swapchain,
    Clear,
    Scene3dIsolation,
    AutoStartScene3d
}

namespace XuanYu.Engine.Render.Vulkan.Surface;

/// <summary>
/// Vulkan Surface 创建探测状态。
/// </summary>
public enum VulkanSurfaceStatus
{
    NotChecked = 0,
    Created = 1,
    Failed = 2,
    UnsupportedPlatform = 3
}

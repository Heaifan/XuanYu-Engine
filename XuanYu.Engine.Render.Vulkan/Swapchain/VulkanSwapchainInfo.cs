namespace XuanYu.Engine.Render.Vulkan.Swapchain;

/// <summary>
/// Vulkan Swapchain 创建结果模型。
/// </summary>
public sealed record VulkanSwapchainInfo(
    VulkanSwapchainStatus Status,
    string Message,
    uint ImageCount,
    string SurfaceFormatText,
    string PresentModeText,
    uint Width,
    uint Height,
    double ElapsedMilliseconds)
{
    public bool IsCreated => Status == VulkanSwapchainStatus.Created;

    public static VulkanSwapchainInfo NotChecked { get; } =
        new(VulkanSwapchainStatus.NotChecked, "Vulkan Swapchain 尚未创建。", 0, "未知", "未知", 0, 0, 0);
}

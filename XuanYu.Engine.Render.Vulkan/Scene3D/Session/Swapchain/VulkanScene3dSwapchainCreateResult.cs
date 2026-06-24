using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>
/// Swapchain 创建/重建的结构化结果。
/// 携带真实 VkResult、请求尺寸和阶段信息。
/// </summary>
public sealed record VulkanScene3dSwapchainCreateResult(
    bool IsSucceeded,
    VulkanScene3dSwapchainResources? Resources,
    VulkanScene3dSwapchainStage Stage,
    Result? VulkanResult,
    uint RequestedWidth,
    uint RequestedHeight,
    string Message)
{
    public static VulkanScene3dSwapchainCreateResult Failed(
        VulkanScene3dSwapchainStage stage, Result? vkResult,
        uint w, uint h, string message) =>
        new(false, null, stage, vkResult, w, h, message);
}

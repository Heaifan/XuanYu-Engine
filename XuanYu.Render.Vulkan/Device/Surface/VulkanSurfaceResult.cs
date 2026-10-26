namespace XuanYu.Render.Vulkan;

// VK3-B2：Vulkan Surface 创建结果。Owner 非空表示创建成功。
// 仅携带创建结果元数据，不含 Device / Swapchain / Queue 概念。
public sealed record VulkanSurfaceResult(
    bool Success,
    VulkanSurfaceOwner? Owner,
    string ErrorType = "",
    string ErrorMessage = "");

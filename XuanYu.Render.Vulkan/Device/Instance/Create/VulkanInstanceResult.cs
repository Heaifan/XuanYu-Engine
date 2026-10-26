namespace XuanYu.Render.Vulkan;

// VK3-B1：Vulkan Instance 创建结果。Owner 非空表示创建成功。
// 仅携带创建结果元数据，不含 Surface / Device / Swapchain 概念。
public sealed record VulkanInstanceResult(
    bool Success,
    VulkanInstanceOwner? Owner,
    uint ApiVersion,
    string ErrorType = "",
    string ErrorMessage = "");

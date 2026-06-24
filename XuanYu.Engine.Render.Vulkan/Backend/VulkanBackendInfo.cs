namespace FluidWarfare.Render.Vulkan.Backend;

/// <summary>
/// 保存 Vulkan 后端探测结果。
/// </summary>
public sealed record VulkanBackendInfo(
    VulkanBackendStatus Status,
    string Message)
{
    public bool IsAvailable => Status == VulkanBackendStatus.Available;

    public static VulkanBackendInfo NotChecked { get; } =
        new(VulkanBackendStatus.NotChecked, "Vulkan 后端尚未探测。");
}

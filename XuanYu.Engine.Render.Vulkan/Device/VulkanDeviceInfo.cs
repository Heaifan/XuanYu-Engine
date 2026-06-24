namespace FluidWarfare.Render.Vulkan.Device;

/// <summary>
/// Vulkan PhysicalDevice / LogicalDevice 探测结果。
/// </summary>
public sealed record VulkanDeviceInfo(
    VulkanDeviceStatus Status,
    string Message,
    string PhysicalDeviceName,
    string PhysicalDeviceTypeText,
    int GraphicsQueueFamilyIndex,
    double ElapsedMilliseconds)
{
    public bool IsCreated => Status == VulkanDeviceStatus.Created;

    public static VulkanDeviceInfo NotChecked { get; } =
        new(
            VulkanDeviceStatus.NotChecked,
            "Vulkan Device 尚未创建。",
            "未知",
            "未知",
            -1,
            0);
}

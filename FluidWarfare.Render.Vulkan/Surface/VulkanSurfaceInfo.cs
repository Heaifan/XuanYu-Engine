namespace FluidWarfare.Render.Vulkan.Surface;

/// <summary>
/// Vulkan Surface 创建探测结果。
/// </summary>
public sealed record VulkanSurfaceInfo(
    VulkanSurfaceStatus Status,
    string Message,
    string PlatformText,
    bool HasNativeHandle,
    double ElapsedMilliseconds)
{
    public bool IsCreated => Status == VulkanSurfaceStatus.Created;

    public static VulkanSurfaceInfo NotChecked { get; } =
        new(
            VulkanSurfaceStatus.NotChecked,
            "Vulkan Surface 尚未创建。",
            "未知",
            false,
            0);
}

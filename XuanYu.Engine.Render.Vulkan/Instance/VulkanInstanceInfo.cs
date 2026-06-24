namespace XuanYu.Engine.Render.Vulkan.Instance;

/// <summary>
/// Vulkan Instance 创建探测结果。
/// </summary>
public sealed record VulkanInstanceInfo(
    VulkanInstanceStatus Status,
    string Message,
    string ApiVersionText,
    int ExtensionCount,
    double ElapsedMilliseconds)
{
    public bool IsCreated => Status == VulkanInstanceStatus.Created;

    public static VulkanInstanceInfo NotChecked { get; } =
        new(
            VulkanInstanceStatus.NotChecked,
            "Vulkan Instance 尚未创建。",
            "未知",
            0,
            0);
}

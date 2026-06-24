namespace XuanYu.Engine.Render.Vulkan.Clear;

/// <summary>
/// Vulkan 最小清屏结果模型。
/// </summary>
public sealed record VulkanClearInfo(
    VulkanClearStatus Status,
    string Message,
    string ClearColorText,
    uint Width,
    uint Height,
    double ElapsedMilliseconds)
{
    public bool IsSucceeded => Status == VulkanClearStatus.Succeeded;

    public static VulkanClearInfo NotChecked { get; } =
        new(VulkanClearStatus.NotChecked, "Vulkan 清屏尚未执行。", "未知", 0, 0, 0);
}

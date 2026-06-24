namespace XuanYu.Engine.Render.Vulkan.Validation;

/// <summary>
/// Vulkan Validation 状态信息。
/// </summary>
public sealed record VulkanValidationInfo(
    VulkanValidationStatus Status,
    string Message,
    int MessageCount)
{
    public bool IsEnabled => Status == VulkanValidationStatus.Enabled;

    public static VulkanValidationInfo Disabled { get; } =
        new(VulkanValidationStatus.Disabled, "Vulkan Validation：未启用。", 0);
}

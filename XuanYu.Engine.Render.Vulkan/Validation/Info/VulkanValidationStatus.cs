namespace XuanYu.Engine.Render.Vulkan.Validation;

/// <summary>
/// Vulkan Validation Layer 启用状态。
/// </summary>
public enum VulkanValidationStatus
{
    Disabled,
    Enabled,
    LayerMissing,
    ExtensionMissing,
    Failed
}

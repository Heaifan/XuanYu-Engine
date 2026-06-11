namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>
/// 一条 Vulkan Validation 消息。
/// </summary>
public sealed record VulkanValidationMessageInfo(
    string Severity,
    string Type,
    string Message);

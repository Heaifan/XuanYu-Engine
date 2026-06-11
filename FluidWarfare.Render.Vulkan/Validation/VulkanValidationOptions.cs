namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>
/// 从环境变量读取是否请求启用 Vulkan Validation Layer。
/// 不负责检测 Layer 是否存在。
/// </summary>
public sealed record VulkanValidationOptions(bool IsRequested)
{
    public static VulkanValidationOptions FromEnvironment()
    {
        var requested = string.Equals(
            Environment.GetEnvironmentVariable("FW_VULKAN_VALIDATION"),
            "1",
            StringComparison.Ordinal);

        return new VulkanValidationOptions(requested);
    }
}

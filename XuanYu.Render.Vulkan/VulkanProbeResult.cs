namespace XuanYu.Render.Vulkan;

public sealed record VulkanProbeResult(
    bool Success,
    uint InstanceVersion,
    IReadOnlyList<VulkanDeviceInfo> Devices,
    string ErrorType = "",
    string ErrorMessage = "");

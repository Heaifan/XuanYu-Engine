using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed class VulkanLabelUploadCompletionUnknownException(CommandBuffer command,
    VulkanLabelUploadResolution result)
    : InvalidOperationException($"Label upload completion unknown: QueueWaitIdle={result.QueueWait?.ToString() ?? "threw"}; " +
        $"DeviceWaitIdle={result.DeviceWait?.ToString() ?? "threw"}.", result.Error)
{
    internal CommandBuffer Command { get; } = command;
    internal VulkanLabelUploadResolution Resolution { get; } = result;
}

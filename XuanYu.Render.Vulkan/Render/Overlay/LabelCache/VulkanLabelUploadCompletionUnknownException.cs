using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed class VulkanLabelUploadCompletionUnknownException(
    CommandBuffer command, Result queueWait, Result deviceWait)
    : InvalidOperationException($"Label upload completion unknown: QueueWaitIdle={queueWait}; DeviceWaitIdle={deviceWait}.")
{
    internal CommandBuffer Command { get; } = command;
}

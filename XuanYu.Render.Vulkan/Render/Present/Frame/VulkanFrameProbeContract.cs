using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Frame;

public enum VulkanFenceWaitTarget
{
    FrameSlotGraphicsFence,
    AcquiredImageOwnerGraphicsFence,
    SwapchainImagePresentFence
}

public sealed record VulkanFrameIdentity(string? SessionId, long FrameId,
    long CameraRevision, int? FrameSlot, uint? ImageIndex,
    uint? SwapchainGeneration, int FramesInFlight);

public sealed record VulkanFenceWaitObservation(VulkanFrameIdentity Identity,
    VulkanFenceWaitTarget WaitTarget, long StartTimestamp, long EndTimestamp,
    long ClockFrequency, int StartThreadId, int EndThreadId, Result Result);

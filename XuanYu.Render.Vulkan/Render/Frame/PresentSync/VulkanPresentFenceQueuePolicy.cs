using Silk.NET.Vulkan;
using System;

namespace XuanYu.Render.Vulkan.Render;

public enum PresentQueueDisposition
{
    Enqueued,
    NotEnqueued,
    DeviceLost,
    Unknown
}

public static class VulkanPresentFenceQueuePolicy
{
    public static PresentQueueDisposition Classify(Result result) => result switch
    {
        Result.Success or Result.SuboptimalKhr or Result.ErrorOutOfDateKhr or Result.ErrorSurfaceLostKhr or Result.ErrorFullScreenExclusiveModeLostExt
            => PresentQueueDisposition.Enqueued,
        Result.ErrorOutOfHostMemory or Result.ErrorOutOfDeviceMemory
            => PresentQueueDisposition.NotEnqueued,
        Result.ErrorDeviceLost => PresentQueueDisposition.DeviceLost,
        _ => PresentQueueDisposition.Unknown
    };

    public static bool RequiresFenceWait(PresentQueueDisposition disposition) =>
        disposition == PresentQueueDisposition.Enqueued;

}

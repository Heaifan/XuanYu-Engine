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

    public static bool Complete(Result queueResult, Func<Result> waitFence,
        out PresentQueueDisposition disposition)
    {
        disposition = Classify(queueResult);
        return disposition switch
        {
            PresentQueueDisposition.Enqueued => waitFence() == Result.Success,
            PresentQueueDisposition.NotEnqueued or PresentQueueDisposition.DeviceLost => false,
            _ => false
        };
    }

    public static bool CanRelease(bool graphicsComplete, bool presentComplete,
        PresentQueueDisposition disposition) => graphicsComplete &&
        (!RequiresFenceWait(disposition) || presentComplete) && disposition != PresentQueueDisposition.DeviceLost;
}

using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Label;

internal enum VulkanLabelUploadState { NotSubmitted, Complete, DeviceLost, CompletionUnknown }

internal readonly record struct VulkanLabelUploadResolution(VulkanLabelUploadState State,
    Result? Submit, Result? QueueWait, Result? DeviceWait, Exception? Error = null);

internal static class VulkanMapLabelUploadCompletion
{
    internal static VulkanLabelUploadResolution Resolve(Result submitted,
        Func<Result> queueWait, Func<Result> deviceWait)
    {
        if (submitted == Result.ErrorDeviceLost)
            return new(VulkanLabelUploadState.DeviceLost, submitted, null, null);
        if (submitted != Result.Success)
            return new(VulkanLabelUploadState.NotSubmitted, submitted, null, null);
        Result queued;
        try { queued = queueWait(); }
        catch (Exception error) { return ResolveDeviceWait(submitted, null, deviceWait, error); }
        if (queued == Result.Success)
            return new(VulkanLabelUploadState.Complete, submitted, queued, null);
        if (queued == Result.ErrorDeviceLost)
            return new(VulkanLabelUploadState.DeviceLost, submitted, queued, null);
        return ResolveDeviceWait(submitted, queued, deviceWait, null);
    }

    static VulkanLabelUploadResolution ResolveDeviceWait(Result submitted, Result? queued,
        Func<Result> deviceWait, Exception? priorError)
    {
        Result idle;
        try { idle = deviceWait(); }
        catch (Exception error)
        {
            var failure = priorError is null ? error : new AggregateException(priorError, error);
            return new(VulkanLabelUploadState.CompletionUnknown, submitted, queued, null, failure);
        }
        var state = idle == Result.Success ? VulkanLabelUploadState.Complete :
            idle == Result.ErrorDeviceLost ? VulkanLabelUploadState.DeviceLost :
            VulkanLabelUploadState.CompletionUnknown;
        return new(state, submitted, queued, idle, priorError);
    }
}

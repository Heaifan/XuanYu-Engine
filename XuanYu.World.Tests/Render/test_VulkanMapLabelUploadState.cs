using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.Label;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanMapLabelUploadStateTests
{
    [Fact]
    public void Submit_failure_is_not_submitted_and_does_not_wait()
    {
        var waits = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.ErrorOutOfDeviceMemory,
            () => { waits++; return Result.Success; }, () => { waits++; return Result.Success; });
        Assert.Equal(VulkanLabelUploadState.NotSubmitted, state.State);
        Assert.Equal(0, waits);
    }

    [Fact]
    public void Submit_device_loss_is_terminal_without_waiting()
    {
        var waits = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.ErrorDeviceLost,
            () => { waits++; return Result.Success; }, () => { waits++; return Result.Success; });
        Assert.Equal(VulkanLabelUploadState.DeviceLost, state.State);
        Assert.Equal(0, waits);
    }

    [Fact]
    public void Successful_queue_wait_completes_without_device_wait()
    {
        var waits = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.Success,
            () => { waits++; return Result.Success; }, () => { waits++; return Result.Success; });
        Assert.Equal(VulkanLabelUploadState.Complete, state.State);
        Assert.Equal(1, waits);
    }

    [Theory]
    [InlineData(Result.ErrorDeviceLost, Result.Success, (int)VulkanLabelUploadState.DeviceLost, 1)]
    [InlineData(Result.ErrorUnknown, Result.Success, (int)VulkanLabelUploadState.Complete, 2)]
    [InlineData(Result.ErrorUnknown, Result.ErrorOutOfHostMemory, (int)VulkanLabelUploadState.CompletionUnknown, 2)]
    [InlineData(Result.ErrorUnknown, Result.ErrorDeviceLost, (int)VulkanLabelUploadState.DeviceLost, 2)]
    public void Wait_results_are_classified_once(Result queueWait, Result deviceWait,
        int expected, int calls)
    {
        var count = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.Success,
            () => { count++; return queueWait; }, () => { count++; return deviceWait; });
        Assert.Equal((VulkanLabelUploadState)expected, state.State);
        Assert.Equal(calls, count);
    }

    [Fact]
    public void Queue_wait_exception_uses_one_device_wait_fallback()
    {
        var waits = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.Success,
            () => { waits++; throw new InvalidOperationException("queue wait failed"); },
            () => { waits++; return Result.Success; });
        Assert.Equal(VulkanLabelUploadState.Complete, state.State);
        Assert.Equal(2, waits);
        Assert.NotNull(state.Error);
    }

    [Fact]
    public void Device_wait_exception_is_unknown_and_never_retried()
    {
        var waits = 0;
        var state = VulkanMapLabelUploadCompletion.Resolve(Result.Success,
            () => { waits++; return Result.ErrorUnknown; },
            () => { waits++; throw new InvalidOperationException("device wait failed"); });
        Assert.Equal(VulkanLabelUploadState.CompletionUnknown, state.State);
        Assert.Equal(2, waits);
        Assert.NotNull(state.Error);
    }

}

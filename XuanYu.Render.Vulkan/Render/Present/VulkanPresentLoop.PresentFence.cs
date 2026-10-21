using Silk.NET.Vulkan;
using System.Threading;
using XuanYu.Render.Vulkan.Render.Generation;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    VulkanPresentSyncGeneration? _syncGeneration;
    bool _presentPending;
    bool _presentUnknown;
    bool _deviceLost;
    bool _graphicsPending;
    uint _presentGeneration;
    uint _presentImage;
    long _presentFenceCompletionCount;

    bool ResetPresentFence() => _syncGeneration?.ResetPresentFence() == Result.Success;

    bool FinishPresent(Result result, SwapchainKHR swapchain, uint imageIndex)
    {
        var complete = VulkanPresentFenceQueuePolicy.Complete(result, () =>
        {
            _presentPending = true;
            _presentGeneration = _syncGeneration?.Generation ?? _swapchainOwner.ResourceGeneration;
            _presentImage = imageIndex;
            return WaitPresentFence() ? Result.Success : Result.Timeout;
        }, out var disposition);
        if (disposition == PresentQueueDisposition.DeviceLost) _deviceLost = true;
        if (disposition == PresentQueueDisposition.Unknown) _presentUnknown = true;
        return complete;
    }

    bool WaitPresentFence()
    {
        if (_syncGeneration is null) return false;
        var result = _syncGeneration.WaitPresentFence(FenceTimeoutNs);
        if (result == Result.Success)
        {
            _presentPending = false;
            var count = Interlocked.Increment(ref _presentFenceCompletionCount);
            if (count == 1 || count % 120 == 0)
                Log($"Present Fence complete: count={count}; generation={_presentGeneration}; image={_presentImage}; swapchain={_syncGeneration.Swapchain.Handle}");
            return true;
        }
        if (result == Result.ErrorDeviceLost) _deviceLost = true;
        return false;
    }

    bool CompleteForStop()
    {
        if (_deviceLost) return false;
        if (_graphicsPending && !WaitFence(_deviceOwner.LogicalDevice)) return false;
        if (_presentPending && !WaitPresentFence()) return false;
        return !_presentUnknown;
    }

    bool CanDestroySync => _deviceLost || (!_presentPending && !_presentUnknown && !_graphicsPending);
}

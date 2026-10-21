using Silk.NET.Vulkan;
using System.Threading;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    Fence _presentFence;
    bool _presentPending;
    bool _presentUnknown;
    bool _deviceLost;
    bool _graphicsPending;
    uint _presentGeneration;
    uint _presentImage;
    long _presentFenceCompletionCount;

    bool ResetPresentFence()
    {
        var fence = _presentFence;
        return _presentFence.Handle != 0 && _vk.ResetFences(_deviceOwner.LogicalDevice, 1, &fence) == Result.Success;
    }

    bool FinishPresent(Result result, SwapchainKHR swapchain, uint imageIndex)
    {
        var complete = VulkanPresentFenceQueuePolicy.Complete(result, () =>
        {
            _presentPending = true;
            _presentGeneration = _swapchainOwner.ResourceGeneration;
            _presentImage = imageIndex;
            return WaitPresentFence(swapchain) ? Result.Success : Result.Timeout;
        }, out var disposition);
        if (disposition == PresentQueueDisposition.DeviceLost) _deviceLost = true;
        if (disposition == PresentQueueDisposition.Unknown) _presentUnknown = true;
        return complete;
    }

    bool WaitPresentFence(SwapchainKHR swapchain)
    {
        var fence = _presentFence;
        var result = _vk.WaitForFences(_deviceOwner.LogicalDevice, 1, &fence, true, FenceTimeoutNs);
        if (result == Result.Success)
        {
            _presentPending = false;
            var count = Interlocked.Increment(ref _presentFenceCompletionCount);
            if (count == 1 || count % 120 == 0)
                Log($"Present Fence complete: count={count}; generation={_presentGeneration}; image={_presentImage}; swapchain={swapchain.Handle}");
            return true;
        }
        if (result == Result.ErrorDeviceLost) _deviceLost = true;
        return false;
    }

    bool CompleteForStop()
    {
        if (_deviceLost) { _graphicsPending = false; _presentPending = false; _presentUnknown = false; return true; }
        if (_graphicsPending && !WaitFence(_deviceOwner.LogicalDevice)) return false;
        if (_presentPending && !WaitPresentFence(_swapchainOwner.Swapchain)) return false;
        return !_presentUnknown;
    }

    bool CanDestroySync => _deviceLost || (!_presentPending && !_presentUnknown && !_graphicsPending);
}

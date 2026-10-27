using Silk.NET.Vulkan;
using System.Diagnostics;
using System.Threading;
using XuanYu.Render.Vulkan.Render.Generation;
using XuanYu.Render.Vulkan.Render.Frame;

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
    bool[] _presentPendingImages = [];
    long _submissionSerial;

    bool ResetPresentFence(uint imageIndex) => _syncGeneration?.ResetPresentFence(imageIndex) == Result.Success;

    bool FinishPresent(Result result, VulkanFrameContext frame, uint imageIndex)
    {
        var disposition = VulkanPresentFenceQueuePolicy.Classify(result);
        if (disposition == PresentQueueDisposition.Enqueued)
        {
            _presentPendingImages[imageIndex] = true;
            _presentPending = true;
            frame.PresentPending = true;
            _presentGeneration = _syncGeneration?.Generation ?? _swapchainOwner.ResourceGeneration;
            _presentImage = imageIndex;
            return true;
        }
        if (disposition == PresentQueueDisposition.DeviceLost) _deviceLost = true;
        if (disposition == PresentQueueDisposition.Unknown) _presentUnknown = true;
        return false;
    }

    bool WaitPresentFence(uint imageIndex, VulkanFrameContext? frame = null)
    {
        if (imageIndex >= _presentPendingImages.Length || !_presentPendingImages[imageIndex]) return true;
        if (_syncGeneration is null) return false;
        var start = _onFenceWait is null ? 0 : Stopwatch.GetTimestamp();
        var startThread = _onFenceWait is null ? 0 : Environment.CurrentManagedThreadId;
        var result = _syncGeneration.WaitPresentFence(imageIndex, FenceTimeoutNs);
        if (_onFenceWait is not null && frame is not null)
            PublishFenceWait(frame, VulkanFenceWaitTarget.SwapchainImagePresentFence,
                start, startThread, result);
        if (result == Result.Success)
        {
            _presentPendingImages[imageIndex] = false;
            _presentPending = _presentPendingImages.Any(x => x);
            foreach (var pendingFrame in _frames.Where(x => x.LastImageIndex == imageIndex))
                pendingFrame.PresentPending = false;
            var count = Interlocked.Increment(ref _presentFenceCompletionCount);
            if (count == 1 || count % 120 == 0)
                Log($"Present Fence complete: count={count}; generation={_presentGeneration}; image={_presentImage}; swapchain={_syncGeneration.Swapchain.Handle}");
            return true;
        }
        if (result == Result.ErrorDeviceLost) _deviceLost = true;
        else Fatal($"Present Fence 等待失败：image={imageIndex}; result={result}");
        return false;
    }

    VulkanFrameContext? FindPresentFrame(uint imageIndex)
        => _frames.FirstOrDefault(x => x.PresentPending && x.LastImageIndex == imageIndex);

    bool CompleteForStop()
    {
        if (_deviceLost) return true;
        if (!WaitFence(_deviceOwner.LogicalDevice)) return false;
        for (uint i = 0; i < _presentPendingImages.Length; i++)
            if (!WaitPresentFence(i, FindPresentFrame(i))) return false;
        return !_presentUnknown;
    }

    bool CanDestroySync => _deviceLost || (!_presentPending && !_presentUnknown && !_frames.Any(x => x.GraphicsPending));
}

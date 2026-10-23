using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    int _outOfDateRetries;

    AcquireDecision AcquireImage(Silk.NET.Vulkan.Device device, VulkanFrameContext frame,
        out SwapchainKHR swapchain, out uint index)
    {
        swapchain = _swapchainOwner.Swapchain;
        uint acquiredIndex;
        var result = _swapchainOwner.Khr.AcquireNextImage(device, swapchain, AcquireTimeoutNs,
            frame.ImageAvailable, default, &acquiredIndex);
        var decision = HandleAcquireResult(result);
        index = decision == AcquireDecision.Acquired ? acquiredIndex : 0;
        if (decision == AcquireDecision.Acquired) _outOfDateRetries = 0;
        if (decision == AcquireDecision.RetryAfterRecreate)
        {
            if (!VulkanAcquireResultPolicy.CanRetryRecovery(_outOfDateRetries))
            {
                Fatal($"AcquireNextImage 连续 {_outOfDateRetries} 次 OutOfDate，重试无进展。");
                return AcquireDecision.StopOrFatal;
            }
            _outOfDateRetries++;
        }
        if (decision != AcquireDecision.Acquired) return decision;

        var current = swapchain.Handle == _swapchainOwner.Swapchain.Handle;
        if (VulkanAcquireResultPolicy.HasValidImageIndex(index, _swapchainOwner.Images.Length, current))
            return decision;

        Fatal($"AcquireNextImage 返回无效或过期图像索引：index={index}；imageCount={_swapchainOwner.Images.Length}。");
        return AcquireDecision.StopOrFatal;
    }
}

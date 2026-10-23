using XuanYu.Render.Vulkan.Render.Generation;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    internal bool RetireGenerationForSwapchainReplacement()
    {
        if (_deviceLost || _presentUnknown) return false;
        if (!WaitFence(_deviceOwner.LogicalDevice)) return false;
        for (uint i = 0; i < _presentPendingImages.Length; i++)
            if (!WaitPresentFence(i, FindPresentFrame(i))) return false;
        if (_syncGeneration is null || !VulkanPresentGenerationPolicy.CanRetire(
                !_frames.Any(x => x.GraphicsPending), _presentPendingImages.All(x => !x),
                _presentUnknown, _deviceLost)) return false;
        var retiredGeneration = _syncGeneration.Generation;
        var retiredImageCount = _syncGeneration.ImageCount;
        var retiredSwapchain = _syncGeneration.Swapchain.Handle;
        _syncGeneration.Dispose();
        Log($"PresentSync generation retired: generation={retiredGeneration}; imageCount={retiredImageCount}; swapchain={retiredSwapchain}; graphics=complete; present=complete");
        _syncGeneration = null;
        _clearFrame.ReleaseSwapchainGeneration();
        return true;
    }
}

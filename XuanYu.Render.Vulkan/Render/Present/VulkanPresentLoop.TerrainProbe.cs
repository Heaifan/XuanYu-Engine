using Silk.NET.Vulkan;
using XuanYu.Core.Diagnostics;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool HandlePresent(KhrSwapchain khr, SwapchainKHR swapchain,
        uint imageIndex, long gpuFrameId, long cameraRevision)
    {
        var result = PresentFrame(khr, swapchain, imageIndex);
        if (result == Result.ErrorOutOfDateKhr)
        {
            return _onOutOfDate?.Invoke("QueuePresent") ?? true;
        }

        if (!Check(result, "QueuePresent", allowSuboptimal: true)) return false;
        if (TerrainReplayOptions.Enabled && ViewportProbe.IsCaptureTargetFrame(gpuFrameId, cameraRevision))
        {
            try
            {
                var format = _swapchainOwner.FormatName;
                if (format is not ("B8G8R8A8Unorm" or "B8G8R8A8Srgb" or
                    "R8G8B8A8Unorm" or "R8G8B8A8Srgb"))
                    ViewportProbe.Log("present", $"[GPU-PNG-UNSUPPORTED-FORMAT] FrameId={gpuFrameId};Format={format}");
                else if (WaitFence(_deviceOwner.LogicalDevice) &&
                    _clearFrame.TryReadbackPresentedImage(imageIndex, out var pixels))
                    ViewportProbe.WriteGpuCapture(gpuFrameId, imageIndex, cameraRevision,
                        (int)_swapchainOwner.Extent.Width, (int)_swapchainOwner.Extent.Height,
                        format, pixels);
                else ViewportProbe.Log("present", $"[GPU-PNG-UNAVAILABLE] FrameId={gpuFrameId};ImageIndex={imageIndex}");
            }
            catch (Exception ex)
            {
                ViewportProbe.Log("present", $"[GPU-PNG-FAILED] FrameId={gpuFrameId};Error={ex.Message}");
            }
        }
        if (TerrainReplayOptions.Enabled)
            ViewportProbe.FramePresented(gpuFrameId, imageIndex, cameraRevision);
        if (!_firstPresentLogged) LogFirstPresent(imageIndex);
        return true;
    }
}

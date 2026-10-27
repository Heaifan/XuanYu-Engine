using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Frame;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool HandlePresent(KhrSwapchain khr, SwapchainKHR swapchain, VulkanFrameContext frame,
        uint imageIndex)
    {
        if (!PresentFrame(khr, swapchain, imageIndex, out var result))
        { Fatal("Present Fence 重置失败；未提交 QueuePresent。"); return false; }
        if (!FinishPresent(result, frame, imageIndex))
        {
            PublishRendererEvent(RendererEventCodes.VulkanOperationFailed, RendererEventLevel.Error,
                result.ToString(), "PresentFenceCompletion");
            return false;
        }
        if (result == Result.ErrorOutOfDateKhr)
        {
            PublishRendererEvent(RendererEventCodes.PresentRecoveryRequested,
                RendererEventLevel.Warning, result.ToString(), "QueuePresent");
            _eventSession?.PublishFrameEvent(RendererEventCodes.QueuePresentOutOfDate,
                RendererEventLevel.Warning, frame.FrameId, imageIndex, frame.CameraRevision,
                frame.FrameIndex, _framesInFlight, frame.SwapchainGeneration);
            return _onOutOfDate?.Invoke("QueuePresent") ?? true;
        }
        if (result == Result.SuboptimalKhr)
            PublishRendererEvent(RendererEventCodes.PresentSuboptimal,
                RendererEventLevel.Warning, result.ToString());
        if (!Check(result, "QueuePresent", allowSuboptimal: true))
        {
            _eventSession?.PublishFrameEvent(RendererEventCodes.QueuePresentFailed,
                RendererEventLevel.Error, frame.FrameId, imageIndex, frame.CameraRevision,
                frame.FrameIndex, _framesInFlight, frame.SwapchainGeneration);
            return false;
        }
        CaptureReplayFrame(frame, imageIndex);
        if (TerrainReplayOptions.Enabled)
            ViewportProbe.FramePresented(frame.FrameId, imageIndex, frame.CameraRevision);
        PublishTerrainResources(frame, imageIndex);
        _eventSession?.PublishFrameEvent(RendererEventCodes.FramePresented,
            RendererEventLevel.Info, frame.FrameId, imageIndex, frame.CameraRevision,
            frame.FrameIndex, _framesInFlight, frame.SwapchainGeneration);
        if (!_firstPresentLogged) LogFirstPresent(imageIndex);
        return true;
    }

    void CaptureReplayFrame(VulkanFrameContext frame, uint imageIndex)
    {
        if (!TerrainReplayOptions.Enabled || !ViewportProbe.IsCaptureTargetFrame(frame.FrameId,
                frame.CameraRevision)) return;
        try
        {
            var format = _swapchainOwner.FormatName;
            if (format is not ("B8G8R8A8Unorm" or "B8G8R8A8Srgb" or "R8G8B8A8Unorm" or "R8G8B8A8Srgb"))
                ViewportProbe.Log("present", $"[GPU-PNG-UNSUPPORTED-FORMAT] FrameId={frame.FrameId};Format={format}");
            else if (WaitFence(_deviceOwner.LogicalDevice) &&
                _clearFrame.TryReadbackPresentedImage(imageIndex, out var pixels))
                ViewportProbe.WriteGpuCapture(frame.FrameId, imageIndex, frame.CameraRevision,
                    (int)_swapchainOwner.Extent.Width, (int)_swapchainOwner.Extent.Height, format, pixels);
            else ViewportProbe.Log("present", $"[GPU-PNG-UNAVAILABLE] FrameId={frame.FrameId};ImageIndex={imageIndex}");
        }
        catch (Exception ex)
        {
            ViewportProbe.Log("present", $"[GPU-PNG-FAILED] FrameId={frame.FrameId};Error={ex.Message}");
        }
    }
}

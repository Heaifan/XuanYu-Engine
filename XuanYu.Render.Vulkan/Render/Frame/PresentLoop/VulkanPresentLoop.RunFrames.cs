using System.Threading;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    void RunFrames()
    {
        var device = _deviceOwner.LogicalDevice;
        var khr = _swapchainOwner.Khr;
        var stage = Silk.NET.Vulkan.PipelineStageFlags.ColorAttachmentOutputBit;
        while (Volatile.Read(ref _stopRequested) == 0)
        {
            var frame = _frames[_nextFrameSlot];
            if (frame.GraphicsPending && !WaitFence(device, frame)) break;
            if (frame.PresentPending && !WaitPresentFence(frame.LastImageIndex, frame)) break;
            if (!EnsurePresentGeneration()) { Fatal("Swapchain generation synchronization could not be safely replaced."); break; }
            if (!_clearFrame.TryApplyPendingRenderProjection()) { Fatal("Render Projection CommandBuffer 重录失败。"); break; }
            if (!_clearFrame.HasRenderProjection) { Thread.Sleep(16); continue; }
            var decision = AcquireImage(device, frame, out var swapchain, out var imageIndex);
            switch (decision)
            {
                case AcquireDecision.Acquired: break;
                case AcquireDecision.TimeoutRetry:
                case AcquireDecision.RetryAfterRecreate: continue;
                default: return;
            }
            frame.SwapchainGeneration = _syncGeneration!.Generation;
            var owner = _imageOwnerSlots[imageIndex];
            if (owner >= 0 && _imageOwnerGenerations[imageIndex] != frame.SwapchainGeneration)
            { Fatal("Swapchain image ownership belongs to a retired generation."); break; }
            frame.FrameId = NextGpuFrameId();
            frame.CameraRevision = _clearFrame.RenderProjection.Camera.Revision;
            frame.LastImageIndex = imageIndex;
            if (owner >= 0 && owner != frame.FrameIndex && _frames[owner].GraphicsPending &&
                !WaitFence(device, _frames[owner], VulkanFenceWaitTarget.AcquiredImageOwnerGraphicsFence, frame)) break;
            if (!VulkanFrameInFlightPolicy.CanUseImage(imageIndex, (uint)_swapchainOwner.Images.Length,
                    frame.SwapchainGeneration, _syncGeneration.Generation, true))
            { Fatal("Swapchain image ownership validation failed."); break; }
            if (!WaitPresentFence(imageIndex, frame)) break;
            _clearFrame.BeginPresentFrame(frame);
            if (!_clearFrame.PreparePresentCommandBuffer(frame.FrameIndex, imageIndex))
            { Fatal("Present CommandBuffer 录制失败。"); break; }
            if (!ResetFence(device, frame) || !SubmitFrame(device, frame, imageIndex, stage)) break;
            _imageOwnerSlots[imageIndex] = frame.FrameIndex;
            _imageOwnerGenerations[imageIndex] = frame.SwapchainGeneration;
            if (TerrainReplayOptions.Enabled) ViewportProbe.FrameSubmitted(frame.FrameId, imageIndex, frame.CameraRevision);
            if (!HandlePresent(khr, swapchain, frame, imageIndex)) break;
            _nextFrameSlot = VulkanFrameInFlightPolicy.NextFrameIndex(_nextFrameSlot, _framesInFlight);
        }
    }
}

using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool SubmitFrame(Silk.NET.Vulkan.Device device, VulkanFrameContext frame, uint idx, PipelineStageFlags stage)
    {
        var command = _clearFrame.GetPresentCommandBuffer(frame.FrameIndex, idx);
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo };
        var imageAvailable = frame.ImageAvailable;
        if (_syncGeneration is null || !_syncGeneration.TryGetImageSemaphore(idx,
                _swapchainOwner.Swapchain, out var renderFinished))
        {
            Fatal("Acquire image does not belong to the active synchronization generation.");
            return false;
        }
        var fence = frame.GraphicsFence;
        submit.WaitSemaphoreCount = 1;
        submit.PWaitSemaphores = &imageAvailable;
        submit.PWaitDstStageMask = &stage;
        submit.CommandBufferCount = 1;
        submit.PCommandBuffers = &command;
        submit.SignalSemaphoreCount = 1;
        submit.PSignalSemaphores = &renderFinished;
        var success = Check(_vk.QueueSubmit(_deviceOwner.GraphicsQueue, 1, &submit, fence), "QueueSubmit");
        if (success)
        {
            frame.GraphicsPending = true;
            frame.SubmissionSerial = Interlocked.Increment(ref _submissionSerial);
            _graphicsPending = true;
        }
        _eventSession?.PublishFrameEvent(success ? RendererEventCodes.FrameSubmitted :
            RendererEventCodes.QueueSubmitFailed, success ? RendererEventLevel.Info :
            RendererEventLevel.Error, frame.FrameId, idx, frame.CameraRevision, frame.FrameIndex,
            _framesInFlight, frame.SwapchainGeneration);
        return success;
    }
}

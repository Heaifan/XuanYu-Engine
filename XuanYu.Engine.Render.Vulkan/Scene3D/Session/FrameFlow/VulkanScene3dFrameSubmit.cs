using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// QueueSubmit 调用。
/// </summary>
partial class VulkanScene3dSession
{
    private unsafe Result SubmitFrame()
    {
        _vk!.GetDeviceQueue(_device, _queueIndex, 0, out _queue);
        var ws = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes!.SemAvail };
        var wst = stackalloc Silk.NET.Vulkan.PipelineStageFlags[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var cb = stackalloc Silk.NET.Vulkan.CommandBuffer[] { _swapchainRes.CommandBuffer };
        var ss = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes.SemFin };
        var si = new SubmitInfo
        {
            SType = StructureType.SubmitInfo, WaitSemaphoreCount = 1, PWaitSemaphores = ws,
            PWaitDstStageMask = wst, CommandBufferCount = 1, PCommandBuffers = cb,
            SignalSemaphoreCount = 1, PSignalSemaphores = ss
        };
        return _vk.QueueSubmit(_queue, 1, &si, _swapchainRes.Fence);
    }
}

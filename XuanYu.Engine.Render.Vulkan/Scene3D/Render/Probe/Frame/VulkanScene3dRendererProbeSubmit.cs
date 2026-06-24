using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 QueueSubmit 阶段。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static bool ProbeSubmitFrame(VulkanScene3dRenderResources r, uint qi)
    {
        var queue = default(Queue); r.Vk!.GetDeviceQueue(r.Device, qi, 0, out queue);
        var waitSem = stackalloc[] { r.SemAvail }; var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var sigSem = stackalloc[] { r.SemFin }; var cBufs = stackalloc[] { r.CommandBuffer };
        var submitInfo = new SubmitInfo { SType = StructureType.SubmitInfo, WaitSemaphoreCount = 1, PWaitSemaphores = waitSem, PWaitDstStageMask = waitStage, CommandBufferCount = 1, PCommandBuffers = cBufs, SignalSemaphoreCount = 1, PSignalSemaphores = sigSem };
        return r.Vk.QueueSubmit(queue, 1, &submitInfo, r.Fence) == Result.Success;
    }
}

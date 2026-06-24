using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>Overlay TriangleList 录制。最后叠加显示。</summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    internal static void RecordDrawOverlay(Vk vk, CommandBuffer cmdBuf,
        PipelineLayout? overlayPipelineLayout,
        Silk.NET.Vulkan.Buffer? overlayBuffer, int overlayVertexCount,
        Pipeline? overlayPipeline, uint overlayViewportWidth, uint overlayViewportHeight,
        ref int drawCalls)
    {
        if (!overlayBuffer.HasValue || overlayBuffer.Value.Handle == 0 ||
            overlayVertexCount <= 0 || !overlayPipeline.HasValue || overlayPipeline.Value.Handle == 0)
            return;

        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, overlayPipeline.Value);
        var pc = stackalloc float[2]; pc[0] = overlayViewportWidth; pc[1] = overlayViewportHeight;
        vk.CmdPushConstants(cmdBuf, overlayPipelineLayout!.Value, ShaderStageFlags.VertexBit, 0, 8, pc);
        var buf = stackalloc[] { overlayBuffer.Value }; var off = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, buf, off);
        vk.CmdDraw(cmdBuf, (uint)overlayVertexCount, 1, 0, 0);
        drawCalls++;
    }
}

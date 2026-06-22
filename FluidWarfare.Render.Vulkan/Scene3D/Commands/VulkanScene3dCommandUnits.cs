using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Unit TriangleList + Overlay 录制阶段。
/// 每单位单独 PushConstants（MVP + Tint）。Overlay 最后叠加。
/// stackalloc/fixed 在方法内立即使用。
/// </summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    internal static void RecordDrawUnits(Vk vk, CommandBuffer cmdBuf,
        PipelineLayout pipelineLayout, Pipeline unitPipeline,
        Silk.NET.Vulkan.Buffer unitBuffer, int unitVertexCount,
        UnitDrawData[] unitDrawData, ref int drawCalls)
    {
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, unitPipeline);
        var buf = stackalloc[] { unitBuffer }; var off = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, buf, off);

        foreach (var draw in unitDrawData)
        {
            var mvp = draw.Mvp; var tint = draw.Tint;
            fixed (float* mvpPtr = mvp) fixed (float* tintPtr = tint)
            {
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.MvpOffset,
                    (uint)VulkanScene3dPushConstants.MvpByteSize, mvpPtr);
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.TintOffset,
                    (uint)VulkanScene3dPushConstants.TintByteSize, tintPtr);
            }
            vk.CmdDraw(cmdBuf, (uint)unitVertexCount, 1, 0, 0);
            drawCalls++;
        }
    }

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

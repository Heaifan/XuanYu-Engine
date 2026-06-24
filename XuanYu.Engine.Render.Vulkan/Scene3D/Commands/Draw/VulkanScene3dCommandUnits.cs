using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>Unit TriangleList 录制。每单位单独 PushConstants（MVP + Tint）。</summary>
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
}

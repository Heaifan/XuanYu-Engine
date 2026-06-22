using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>Grid LineList 录制。使用 Grid Pipeline + VP-only MVP。</summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    internal static void RecordDrawGrid(Vk vk, CommandBuffer cmdBuf,
        PipelineLayout pipelineLayout, Pipeline gridPipeline, float[] vpMvp,
        Silk.NET.Vulkan.Buffer gridBuffer, int gridVertexCount, ref int drawCalls)
    {
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, gridPipeline);
        var pushConstants = stackalloc float[VulkanScene3dPushConstants.FloatCount];
        for (var i = 0; i < 16; i++) pushConstants[i] = vpMvp[i];
        for (var i = 0; i < 4; i++) pushConstants[16 + i] = VulkanScene3dPushConstants.GridTint[i];
        vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
            0, (uint)VulkanScene3dPushConstants.ByteSize, pushConstants);
        var buf = stackalloc[] { gridBuffer }; var off = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, buf, off);
        vk.CmdDraw(cmdBuf, (uint)gridVertexCount, 1, 0, 0);
        drawCalls++;
    }
}

using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>GroundCursor LineList 录制。共享 Grid Pipeline。</summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    internal static void RecordDrawGroundCursor(Vk vk, CommandBuffer cmdBuf,
        PipelineLayout pipelineLayout, Pipeline gridPipeline,
        GroundCursorDrawData? groundCursor, ref int drawCalls)
    {
        if (groundCursor is null || groundCursor.CursorVertexCount <= 0 ||
            groundCursor.CursorBuffer.Handle == 0 || groundCursor.CursorModelMvp is null)
            return;

        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, gridPipeline);
        var mvp = groundCursor.CursorModelMvp;
        fixed (float* mvpPtr = mvp)
        {
            vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                (uint)VulkanScene3dPushConstants.MvpOffset,
                (uint)VulkanScene3dPushConstants.MvpByteSize, mvpPtr);
            vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                (uint)VulkanScene3dPushConstants.TintOffset,
                (uint)VulkanScene3dPushConstants.TintByteSize,
                VulkanScene3dPushConstants.GridTint);
        }
        var buf = stackalloc[] { groundCursor.CursorBuffer }; var off = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, buf, off);
        vk.CmdDraw(cmdBuf, (uint)groundCursor.CursorVertexCount, 1, 0, 0);
        drawCalls++;
    }
}

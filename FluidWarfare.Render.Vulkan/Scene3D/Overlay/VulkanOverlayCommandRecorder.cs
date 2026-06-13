using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>
/// 录制 Overlay CommandBuffer。在世界绘制完成后执行。
/// 使用 TriangleList，Depth=off，Blend=on。
/// </summary>
public static unsafe class VulkanOverlayCommandRecorder
{
    /// <summary>
    /// 在已开始的 RenderPass 中录制 Overlay DrawCall。
    /// </summary>
    public static bool Record(
        Vk vk, CommandBuffer cmdBuf,
        Pipeline pipeline, PipelineLayout pipelineLayout,
        uint viewportWidth, uint viewportHeight,
        Silk.NET.Vulkan.Buffer vertexBuffer, int vertexCount,
        out string? error)
    {
        error = null;

        if (vertexCount <= 0) return true; // nothing to draw
        if (pipeline.Handle == 0) { error = "Overlay Pipeline 未创建。"; return false; }
        if (vertexBuffer.Handle == 0) { error = "Overlay VertexBuffer 未创建。"; return false; }

        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, pipeline);

        // Push constant: viewportWidth, viewportHeight
        var pc = stackalloc float[2];
        pc[0] = viewportWidth;
        pc[1] = viewportHeight;
        vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
            0, VulkanOverlayPipelineLayout.PushConstantByteSize, pc);

        var buf = stackalloc[] { vertexBuffer };
        var offsets = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, buf, offsets);
        vk.CmdDraw(cmdBuf, (uint)vertexCount, 1, 0, 0);

        return true;
    }
}

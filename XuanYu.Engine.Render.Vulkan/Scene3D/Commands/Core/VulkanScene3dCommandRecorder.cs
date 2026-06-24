using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>CommandBuffer 录制编排器。各录制阶段委托到专用辅助。</summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    public sealed record UnitDrawData(float[] Mvp, float[] Tint);

    public sealed record GroundCursorDrawData(
        Silk.NET.Vulkan.Buffer CursorBuffer, int CursorVertexCount, float[]? CursorModelMvp);

    public static bool Record(
        Vk vk, CommandBuffer cmdBuf,
        RenderPass renderPass, Framebuffer framebuffer, Extent2D extent,
        Pipeline gridPipeline, Pipeline unitPipeline, PipelineLayout pipelineLayout,
        float[] vpMvp,
        Silk.NET.Vulkan.Buffer gridBuffer, int gridVertexCount,
        Silk.NET.Vulkan.Buffer unitBuffer, int unitVertexCount,
        UnitDrawData[] unitDrawData,
        GroundCursorDrawData? groundCursor,
        Silk.NET.Vulkan.Buffer? overlayBuffer, int overlayVertexCount,
        Pipeline? overlayPipeline, PipelineLayout? overlayPipelineLayout,
        uint overlayViewportWidth, uint overlayViewportHeight,
        out int drawCalls, out string errorMessage)
    {
        drawCalls = 0; errorMessage = string.Empty;
        if (gridVertexCount <= 0) { errorMessage = "Grid vertex count <= 0。"; return false; }
        if (unitVertexCount <= 0) { errorMessage = "Unit vertex count <= 0。"; return false; }
        if (gridPipeline.Handle == 0) { errorMessage = "GridPipeline 未创建。"; return false; }
        if (unitPipeline.Handle == 0) { errorMessage = "UnitPipeline 未创建。"; return false; }
        if (gridBuffer.Handle == 0 || unitBuffer.Handle == 0) { errorMessage = "VertexBuffer 未创建。"; return false; }

        if (!RecordBegin(vk, cmdBuf, out errorMessage)) return false;
        RecordBeginRenderPass(vk, cmdBuf, renderPass, framebuffer, extent);
        RecordDrawGrid(vk, cmdBuf, pipelineLayout, gridPipeline, vpMvp, gridBuffer, gridVertexCount, ref drawCalls);
        RecordDrawGroundCursor(vk, cmdBuf, pipelineLayout, gridPipeline, groundCursor, ref drawCalls);
        RecordDrawUnits(vk, cmdBuf, pipelineLayout, unitPipeline, unitBuffer, unitVertexCount, unitDrawData, ref drawCalls);
        RecordDrawOverlay(vk, cmdBuf, overlayPipelineLayout, overlayBuffer, overlayVertexCount,
            overlayPipeline, overlayViewportWidth, overlayViewportHeight, ref drawCalls);
        if (!RecordEnd(vk, cmdBuf, out errorMessage)) return false;
        return true;
    }
}


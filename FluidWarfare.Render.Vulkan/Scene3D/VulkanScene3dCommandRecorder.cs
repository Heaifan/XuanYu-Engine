using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 录制 Scene3D CommandBuffer。
/// 录制顺序：Begin → ClearColor+Depth → BindPipeline(Grid) → PushConstants(VP) → Draw(Grid)
///   → BindPipeline(Unit) → BindVBs → for each object: PushConstants(MVP) → Draw
///   → EndRenderPass → End
/// </summary>
public static unsafe class VulkanScene3dCommandRecorder
{
    public static bool Record(
        Vk vk, CommandBuffer cmdBuf,
        RenderPass renderPass, Framebuffer framebuffer,
        Extent2D extent,
        Pipeline gridPipeline, Pipeline unitPipeline,
        PipelineLayout pipelineLayout,
        float[] vpMvp,
        Silk.NET.Vulkan.Buffer gridBuffer, int gridVertexCount,
        Silk.NET.Vulkan.Buffer unitBuffer, int unitVertexCount,
        float[][] unitMvpArray,
        out int drawCalls,
        out string errorMessage)
    {
        drawCalls = 0;
        errorMessage = string.Empty;

        if (gridVertexCount <= 0)
        {
            errorMessage = "Scene3D CommandRecorder：Grid vertex count <= 0。";
            return false;
        }
        if (unitVertexCount <= 0)
        {
            errorMessage = "Scene3D CommandRecorder：Unit vertex count <= 0。";
            return false;
        }
        if (gridPipeline.Handle == 0)
        {
            errorMessage = "Scene3D CommandRecorder：GridPipeline 未创建。";
            return false;
        }
        if (unitPipeline.Handle == 0)
        {
            errorMessage = "Scene3D CommandRecorder：UnitPipeline 未创建。";
            return false;
        }
        if (gridBuffer.Handle == 0 || unitBuffer.Handle == 0)
        {
            errorMessage = "Scene3D CommandRecorder：VertexBuffer 未创建。";
            return false;
        }

        vk.ResetCommandBuffer(cmdBuf, CommandBufferResetFlags.None);
        var beginInfo = new CommandBufferBeginInfo
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit
        };
        if (vk.BeginCommandBuffer(cmdBuf, &beginInfo) != Result.Success)
        {
            errorMessage = "Scene3D CommandRecorder：BeginCommandBuffer 失败。";
            return false;
        }

        // Clear: color + depth
        const float clearR = 0.03f, clearG = 0.08f, clearB = 0.18f, clearA = 1.0f;
        var clearVals = stackalloc ClearValue[2];
        clearVals[0] = new ClearValue
        {
            Color = new ClearColorValue { Float32_0 = clearR, Float32_1 = clearG, Float32_2 = clearB, Float32_3 = clearA }
        };
        clearVals[1] = new ClearValue
        {
            DepthStencil = new ClearDepthStencilValue { Depth = 1.0f, Stencil = 0 }
        };

        var rpBegin = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = renderPass,
            Framebuffer = framebuffer,
            RenderArea = new Rect2D(new Offset2D(0, 0), extent),
            ClearValueCount = 2,
            PClearValues = clearVals
        };
        vk.CmdBeginRenderPass(cmdBuf, &rpBegin, SubpassContents.Inline);

        // Draw grid (LineList) — uses VP-only MVP
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, gridPipeline);
        fixed (float* mvpPtr = vpMvp)
        {
            vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit, 0, 64, mvpPtr);
        }
        var gridBufs = stackalloc[] { gridBuffer };
        var offsets = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, gridBufs, offsets);
        vk.CmdDraw(cmdBuf, (uint)gridVertexCount, 1, 0, 0);
        drawCalls++;

        // Draw units (shared buffer, per-object MVP)
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, unitPipeline);
        var unitBufs = stackalloc[] { unitBuffer };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, unitBufs, offsets);

        foreach (var unitMvp in unitMvpArray)
        {
            fixed (float* mvpPtr = unitMvp)
            {
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit, 0, 64, mvpPtr);
            }
            vk.CmdDraw(cmdBuf, (uint)unitVertexCount, 1, 0, 0);
            drawCalls++;
        }

        vk.CmdEndRenderPass(cmdBuf);

        if (vk.EndCommandBuffer(cmdBuf) != Result.Success)
        {
            errorMessage = "Scene3D CommandRecorder：EndCommandBuffer 失败。";
            return false;
        }

        return true;
    }
}

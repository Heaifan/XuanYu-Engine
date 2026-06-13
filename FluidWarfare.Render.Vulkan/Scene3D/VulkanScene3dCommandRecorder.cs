using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 录制 Scene3D CommandBuffer。
/// Push Constant 布局：MVP (mat4, 64 字节) + Tint (vec4, 16 字节) = 80 字节。
/// Grid 使用无色 Tint，单位使用普通色或选中高亮色。
/// </summary>
public static unsafe class VulkanScene3dCommandRecorder
{
    /// <summary>
    /// 每个单位对象的绘制信息。
    /// </summary>
    /// <param name="Mvp">列优先 MVP 矩阵 (16 floats)。</param>
    /// <param name="Tint">颜色覆盖 (rgba, alpha=0 使用顶点色, alpha=1 使用覆盖色)。</param>
    public sealed record UnitDrawData(float[] Mvp, float[] Tint);

    /// <summary>
    /// Ground Cursor 绘制参数。不绘制时 CursorVertexCount = 0。
    /// </summary>
    /// <param name="CursorBuffer">Ground Cursor VertexBuffer。</param>
    /// <param name="CursorVertexCount">顶点数量（0 = 不绘制）。</param>
    /// <param name="CursorModelMvp">Ground Cursor 的 Model*VP 矩阵（16 floats）。</param>
    public sealed record GroundCursorDrawData(
        Silk.NET.Vulkan.Buffer CursorBuffer,
        int CursorVertexCount,
        float[]? CursorModelMvp);

    public static bool Record(
        Vk vk, CommandBuffer cmdBuf,
        RenderPass renderPass, Framebuffer framebuffer,
        Extent2D extent,
        Pipeline gridPipeline, Pipeline unitPipeline,
        PipelineLayout pipelineLayout,
        float[] vpMvp,
        Silk.NET.Vulkan.Buffer gridBuffer, int gridVertexCount,
        Silk.NET.Vulkan.Buffer unitBuffer, int unitVertexCount,
        UnitDrawData[] unitDrawData,
        GroundCursorDrawData? groundCursor,
        // Overlay (optional, rendered last)
        Silk.NET.Vulkan.Buffer? overlayBuffer, int overlayVertexCount,
        Pipeline? overlayPipeline, PipelineLayout? overlayPipelineLayout,
        uint overlayViewportWidth, uint overlayViewportHeight,
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

        // Draw grid (LineList) — VP-only MVP, grid tint
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, gridPipeline);
        var pushConstants = stackalloc float[VulkanScene3dPushConstants.FloatCount];
        for (var i = 0; i < 16; i++) pushConstants[i] = vpMvp[i];
        for (var i = 0; i < 4; i++) pushConstants[16 + i] = VulkanScene3dPushConstants.GridTint[i];
        vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
            0, (uint)VulkanScene3dPushConstants.ByteSize, pushConstants);
        var gridBufs = stackalloc[] { gridBuffer };
        var offsets = stackalloc[] { 0ul };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, gridBufs, offsets);
        vk.CmdDraw(cmdBuf, (uint)gridVertexCount, 1, 0, 0);
        drawCalls++;

        // Draw ground cursor (LineList, same Grid pipeline) — if visible
        if (groundCursor is not null && groundCursor.CursorVertexCount > 0 &&
            groundCursor.CursorBuffer.Handle != 0 && groundCursor.CursorModelMvp is not null)
        {
            vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, gridPipeline);
            var cursorMvp = groundCursor.CursorModelMvp;
            fixed (float* mvpPtr = cursorMvp)
            {
                // MVP (64 bytes)
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.MvpOffset,
                    (uint)VulkanScene3dPushConstants.MvpByteSize, mvpPtr);
                // Tint (16 bytes) — ground cursor uses vertex color, so alpha = 0
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.TintOffset,
                    (uint)VulkanScene3dPushConstants.TintByteSize,
                    VulkanScene3dPushConstants.GridTint);
            }
            var cursorBufs = stackalloc[] { groundCursor.CursorBuffer };
            vk.CmdBindVertexBuffers(cmdBuf, 0, 1, cursorBufs, offsets);
            vk.CmdDraw(cmdBuf, (uint)groundCursor.CursorVertexCount, 1, 0, 0);
            drawCalls++;
        }

        // Draw units (shared buffer, per-object MVP + Tint)
        vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, unitPipeline);
        var unitBufs = stackalloc[] { unitBuffer };
        vk.CmdBindVertexBuffers(cmdBuf, 0, 1, unitBufs, offsets);

        foreach (var draw in unitDrawData)
        {
            var mvp = draw.Mvp;
            var tint = draw.Tint;
            fixed (float* mvpPtr = mvp)
            fixed (float* tintPtr = tint)
            {
                // MVP (64 bytes)
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.MvpOffset,
                    (uint)VulkanScene3dPushConstants.MvpByteSize, mvpPtr);
                // Tint (16 bytes)
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit,
                    (uint)VulkanScene3dPushConstants.TintOffset,
                    (uint)VulkanScene3dPushConstants.TintByteSize, tintPtr);
            }
            vk.CmdDraw(cmdBuf, (uint)unitVertexCount, 1, 0, 0);
            drawCalls++;
        }

        // Draw overlay (TriangleList, Depth=off, Blend=on) — last on top of everything
        if (overlayBuffer.HasValue && overlayBuffer.Value.Handle != 0 &&
            overlayVertexCount > 0 && overlayPipeline.HasValue && overlayPipeline.Value.Handle != 0)
        {
            vk.CmdBindPipeline(cmdBuf, PipelineBindPoint.Graphics, overlayPipeline.Value);
            var pc2 = stackalloc float[2];
            pc2[0] = overlayViewportWidth;
            pc2[1] = overlayViewportHeight;
            vk.CmdPushConstants(cmdBuf, overlayPipelineLayout!.Value, ShaderStageFlags.VertexBit,
                0, 8, pc2);
            var olBufs = stackalloc[] { overlayBuffer.Value };
            vk.CmdBindVertexBuffers(cmdBuf, 0, 1, olBufs, offsets);
            vk.CmdDraw(cmdBuf, (uint)overlayVertexCount, 1, 0, 0);
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

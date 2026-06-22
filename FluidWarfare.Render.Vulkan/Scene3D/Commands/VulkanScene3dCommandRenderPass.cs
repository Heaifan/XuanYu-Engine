using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// CommandBuffer Begin + RenderPass Begin/End 阶段。
/// 所有 stackalloc 在方法内创建并立即使用，不返回指针。
/// </summary>
public static unsafe partial class VulkanScene3dCommandRecorder
{
    internal static bool RecordBegin(Vk vk, CommandBuffer cmdBuf, out string error)
    {
        error = string.Empty;
        vk.ResetCommandBuffer(cmdBuf, CommandBufferResetFlags.None);
        var beginInfo = new CommandBufferBeginInfo
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit
        };
        if (vk.BeginCommandBuffer(cmdBuf, &beginInfo) != Result.Success)
        { error = "BeginCommandBuffer 失败。"; return false; }
        return true;
    }

    internal static void RecordBeginRenderPass(Vk vk, CommandBuffer cmdBuf,
        RenderPass renderPass, Framebuffer framebuffer, Extent2D extent)
    {
        var clearVals = stackalloc ClearValue[2];
        clearVals[0] = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.03f, Float32_1 = 0.08f, Float32_2 = 0.18f, Float32_3 = 1.0f } };
        clearVals[1] = new ClearValue { DepthStencil = new ClearDepthStencilValue { Depth = 1.0f, Stencil = 0 } };
        var rpBegin = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = renderPass, Framebuffer = framebuffer,
            RenderArea = new Rect2D(new Offset2D(0, 0), extent),
            ClearValueCount = 2, PClearValues = clearVals
        };
        vk.CmdBeginRenderPass(cmdBuf, &rpBegin, SubpassContents.Inline);
    }

    internal static bool RecordEnd(Vk vk, CommandBuffer cmdBuf, out string error)
    {
        error = string.Empty;
        vk.CmdEndRenderPass(cmdBuf);
        if (vk.EndCommandBuffer(cmdBuf) != Result.Success)
        { error = "EndCommandBuffer 失败。"; return false; }
        return true;
    }
}

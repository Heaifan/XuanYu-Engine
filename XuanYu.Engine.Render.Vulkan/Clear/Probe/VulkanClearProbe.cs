using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Clear;

public static unsafe class VulkanClearProbe
{
    const float ClearR = 0.03f, ClearG = 0.08f, ClearB = 0.18f, ClearA = 1.0f;

    public static VulkanClearInfo ProbeWindows(nint hinstance, nint hwnd, uint reqW, uint reqH)
    {
        var sw = Stopwatch.StartNew();
        if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", sw);
        using var ctx = new VulkanClearProbeContextScope();
        if (!ctx.CreateInstance()) return Fail("Instance 创建失败。", sw);
        if (!ctx.CreateSurface(hinstance, hwnd)) return Fail("Surface 创建失败。", sw);
        var sel = new VulkanClearProbeDeviceSelector();
        if (!sel.TrySelect(ctx.Vk, ctx.Instance, ctx.Surface, ctx.FnSurfaceSupport, out var pd, out var qi, out _))
            return Fail("未找到 Graphics+Present 队列。", sw);
        if (!ctx.CreateDevice(pd, qi)) return Fail("Device 创建失败。", sw);
        var query = new VulkanClearProbeSurfaceQuery();
        var caps = query.QueryCaps(pd, ctx.Surface, ctx.FnGetCaps);
        var formats = query.QueryFormats(pd, ctx.Surface, ctx.FnGetFmts);
        if (formats.Length == 0) return Fail("无可用 Surface 格式。", sw);
        var cf = query.ChooseFormat(formats);
        var cm = query.ChoosePresentMode(query.QueryModes(pd, ctx.Surface, ctx.FnGetModes));
        var ic = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);
        var extent = query.ChooseExtent(caps, reqW, reqH);
        using var target = new VulkanClearProbeRenderTargetScope(ctx.Vk, ctx.Device);
        if (!target.CreateSwapchain(ctx.Surface, cf.Format, cf.ColorSpace, caps, extent, ic, cm,
                ctx.FnCreateSwapchain, ctx.FnDestroySwapchain, ctx.FnGetImages))
            return Fail("Swapchain 创建失败。", sw);
        if (!target.CreateRenderPass()) return Fail("RenderPass 创建失败。", sw);
        using var submit = new VulkanClearProbeRenderSubmitScope(ctx.Vk, ctx.Device);
        if (!submit.CreateCommandPool(qi)) return Fail("CommandPool 创建失败。", sw);
        if (!submit.CreateSync()) return Fail("同步对象创建失败。", sw);

        var fence = submit.Fence;
        ctx.Vk.WaitForFences(ctx.Device, 1, ref fence, Vk.True, ulong.MaxValue);
        ctx.Vk.ResetFences(ctx.Device, 1, ref fence);
        uint imgIndex = 0;
        var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(ctx.FnAcquire);
        var acqRes = acquireFn(ctx.Device, target.Swapchain, ulong.MaxValue, submit.SemAvail, default, &imgIndex);
        if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", sw);
        if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr)
            return Fail($"AcquireNextImage 失败：{acqRes}。", sw);

        ctx.Vk.ResetCommandBuffer(submit.CmdBuf, CommandBufferResetFlags.None);
        var beginInfo = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
        ctx.Vk.BeginCommandBuffer(submit.CmdBuf, &beginInfo);
        var cv = new ClearValue { Color = new ClearColorValue { Float32_0 = ClearR, Float32_1 = ClearG, Float32_2 = ClearB, Float32_3 = ClearA } };
        var rpBegin = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo, RenderPass = target.RenderPass,
            Framebuffer = target.Framebuffers[imgIndex],
            RenderArea = new Rect2D(new Offset2D(0, 0), extent),
            ClearValueCount = 1, PClearValues = &cv
        };
        ctx.Vk.CmdBeginRenderPass(submit.CmdBuf, &rpBegin, SubpassContents.Inline);
        ctx.Vk.CmdEndRenderPass(submit.CmdBuf);
        ctx.Vk.EndCommandBuffer(submit.CmdBuf);

        ctx.Vk.GetDeviceQueue(ctx.Device, qi, 0, out var queue);
        var wSems = stackalloc[] { submit.SemAvail };
        var wStages = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var sSems = stackalloc[] { submit.SemFin };
        var bufs = stackalloc[] { submit.CmdBuf };
        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo, WaitSemaphoreCount = 1, PWaitSemaphores = wSems,
            PWaitDstStageMask = wStages, CommandBufferCount = 1, PCommandBuffers = bufs,
            SignalSemaphoreCount = 1, PSignalSemaphores = sSems
        };
        if (ctx.Vk.QueueSubmit(queue, 1, &submitInfo, submit.Fence) != Result.Success)
            return Fail("QueueSubmit 失败。", sw);
        var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(ctx.FnQueuePresent);
        var scs = stackalloc[] { target.Swapchain };
        var idxs = stackalloc[] { imgIndex };
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr, WaitSemaphoreCount = 1, PWaitSemaphores = sSems,
            SwapchainCount = 1, PSwapchains = scs, PImageIndices = idxs
        };
        var pr = presentFn(queue, &presentInfo);
        if (pr != Result.Success && pr != Result.SuboptimalKhr)
            return Fail($"QueuePresent 失败：{pr}。", sw);
        ctx.Vk.DeviceWaitIdle(ctx.Device);
        sw.Stop();
        return new VulkanClearInfo(VulkanClearStatus.Succeeded, "最小 Vulkan 清屏成功。",
            $"rgba({ClearR:F2}, {ClearG:F2}, {ClearB:F2}, {ClearA:F2})",
            extent.Width, extent.Height, sw.Elapsed.TotalMilliseconds);
    }

    static VulkanClearInfo Fail(string msg, Stopwatch sw) { sw.Stop(); return new VulkanClearInfo(VulkanClearStatus.Failed, msg, $"rgba({ClearR:F2}, {ClearG:F2}, {ClearB:F2}, {ClearA:F2})", 0, 0, sw.Elapsed.TotalMilliseconds); }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}

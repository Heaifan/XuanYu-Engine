using XuanYu.Engine.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Vulkan 资源创建编排器。顺序：Instance→Surface→Device→Swapchain→Resources。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    internal static bool ProbeCreateSession(VulkanScene3dRenderResources r,
        nint hinstance, nint hwnd, uint reqW, uint reqH,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        out uint qi, out nint fnAcquire, out nint fnQueuePresent,
        out Extent2D extent, out Format chosenFmt, out Image[] images,
        out uint imgCount, out int gVc, out int uVc, out string error)
    {
        qi = 0; fnAcquire = 0; fnQueuePresent = 0; extent = default; chosenFmt = default; images = null!; imgCount = 0; gVc = 0; uVc = 0; error = string.Empty;

        // Instance → Surface → Device
        if (!CreateInstance(r.Vk!, out r.Instance)) { error = "Instance 创建失败。"; return false; }
        r.InstOk = true; r.FnDestroySurface = LoadProc(r.Vk!, r.Instance, "vkDestroySurfaceKHR");
        if (!CreateSurface(r.Vk!, r.Instance, hinstance, hwnd, out r.Surface)) { error = "Surface 创建失败。"; return false; }
        r.SurfOk = true;
        if (!SelectDevice(r.Vk!, r.Instance, r.Surface, out var pd, out qi, out _)) { error = "未找到 Graphics+Present 队列。"; return false; }
        if (!CreateDevice(r.Vk!, pd, qi, out r.Device)) { error = "Device 创建失败。"; return false; }
        r.DevOk = true;

        // 函数指针加载
        var fnGetCaps = LoadProc(r.Vk!, r.Instance, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        var fnGetFmts = LoadProc(r.Vk!, r.Instance, "vkGetPhysicalDeviceSurfaceFormatsKHR");
        var fnGetModes = LoadProc(r.Vk!, r.Instance, "vkGetPhysicalDeviceSurfacePresentModesKHR");
        if (fnGetCaps == 0 || fnGetFmts == 0 || fnGetModes == 0) { error = "无法加载 Surface 查询函数。"; return false; }
        r.FnDestroySwapchain = LoadDeviceProc(r.Vk!, r.Device, "vkDestroySwapchainKHR");
        var fnCreateSwapchain = LoadDeviceProc(r.Vk!, r.Device, "vkCreateSwapchainKHR");
        var fnGetImages = LoadDeviceProc(r.Vk!, r.Device, "vkGetSwapchainImagesKHR");
        fnAcquire = LoadDeviceProc(r.Vk!, r.Device, "vkAcquireNextImageKHR");
        fnQueuePresent = LoadDeviceProc(r.Vk!, r.Device, "vkQueuePresentKHR");
        if (fnCreateSwapchain == 0 || r.FnDestroySwapchain == 0 || fnGetImages == 0 || fnAcquire == 0 || fnQueuePresent == 0) { error = "无法加载 Swapchain 设备扩展函数。"; return false; }

        // Swapchain
        if (!ProbeCreateSwapchain(r, pd, fnGetCaps, fnGetFmts, fnGetModes, fnCreateSwapchain, fnGetImages, reqW, reqH, out extent, out chosenFmt, out images, out imgCount, out error)) return false;

        // ImageViews
        if (!ProbeCreateImageViews(r, chosenFmt, imgCount, images, out error)) return false;

        // Depth
        var depthInfo = VulkanScene3dDepthFormatSelector.Select(r.Vk!, pd);
        if (!depthInfo.IsSupported) { error = depthInfo.Message; return false; }
        r.DepthFormat = depthInfo.ChosenFormat; r.DepthAttachmentCount = (int)imgCount;
        r.DepthImages = new Image[imgCount]; r.DepthMemories = new DeviceMemory[imgCount]; r.DepthViews = new ImageView[imgCount];
        if (!VulkanScene3dDepthAttachments.Create(r.Vk!, pd, r.Device, extent, depthInfo.ChosenFormat, imgCount, r.DepthImages, r.DepthMemories, r.DepthViews, out var depthErr)) { error = depthErr; return false; }
        r.DepthOk = true;

        // RenderPass
        if (!CreateRenderPass(r, chosenFmt, r.DepthFormat)) { error = "RenderPass 创建失败。"; return false; }

        // Shaders / PipelineLayout / Pipelines / VertexBuffers
        if (!VulkanScene3dShaderModules.Create(r.Vk!, r.Device, out r.VertModule, out r.FragModule, out var shaderErr)) { error = shaderErr; return false; }
        r.VertModOk = true; r.FragModOk = true;
        if (!VulkanScene3dPipelineLayout.Create(r.Vk!, r.Device, pd, out r.PipelineLayout, out var layoutErr)) { error = layoutErr; return false; }
        r.LayoutOk = true;
        if (!VulkanScene3dPipelines.Create(r.Vk!, r.Device, r.RenderPass, r.PipelineLayout, r.VertModule, r.FragModule, extent.Width, extent.Height, out r.GridPipeline, out r.UnitPipeline, out var pipeErr)) { error = pipeErr; return false; }
        r.GridPipeOk = true; r.UnitPipeOk = true;
        if (!VulkanScene3dVertexBuffers.Create(r.Vk!, pd, r.Device, gridVertices, unitVertices, out r.GridBuffer, out r.GridMemory, out r.UnitBuffer, out r.UnitMemory, out gVc, out uVc, out var bufErr)) { error = bufErr; return false; }
        r.GridBufOk = true; r.UnitBufOk = true;

        // Framebuffers
        if (!ProbeCreateFramebuffers(r, imgCount, extent, out error)) return false;

        // CommandPool / CommandBuffer / Sync
        if (!ProbeCreateSyncObjects(r, qi, out error)) return false;

        return true;
    }

    static bool CreateRenderPass(VulkanScene3dRenderResources r, Format colorFmt, Format depthFmt)
    {
        var colorAtt = new AttachmentDescription { Format = colorFmt, Samples = SampleCountFlags.Count1Bit, LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store, StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare, InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr };
        var depthAtt = new AttachmentDescription { Format = depthFmt, Samples = SampleCountFlags.Count1Bit, LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.DontCare, StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare, InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.DepthStencilAttachmentOptimal };
        var attachments = stackalloc[] { colorAtt, depthAtt };
        var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var depthRef = new AttachmentReference { Attachment = 1, Layout = ImageLayout.DepthStencilAttachmentOptimal };
        var subpass = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &colorRef, PDepthStencilAttachment = &depthRef };
        var rpCI = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 2, PAttachments = attachments, SubpassCount = 1, PSubpasses = &subpass };
        return r.Vk!.CreateRenderPass(r.Device, &rpCI, null, out r.RenderPass) == Result.Success;
    }
}

using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

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

        if (!CreateInstance(r.Vk!, out r.Instance)) { error = "Instance 创建失败。"; return false; }
        r.InstOk = true; r.FnDestroySurface = LoadProc(r.Vk!, r.Instance, "vkDestroySurfaceKHR");
        if (!CreateSurface(r.Vk!, r.Instance, hinstance, hwnd, out r.Surface)) { error = "Surface 创建失败。"; return false; }
        r.SurfOk = true;
        if (!SelectDevice(r.Vk!, r.Instance, r.Surface, out var pd, out qi, out _)) { error = "未找到 Graphics+Present 队列。"; return false; }
        if (!CreateDevice(r.Vk!, pd, qi, out r.Device)) { error = "Device 创建失败。"; return false; }
        r.DevOk = true;
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

        var caps = QueryCaps(pd, r.Surface, fnGetCaps);
        var formats = QueryFormats(pd, r.Surface, fnGetFmts);
        if (formats.Length == 0) { error = "无可用 Surface 格式。"; return false; }
        chosenFmt = ChooseFormat(formats).Format;
        extent = ChooseExtent(caps, reqW, reqH);
        var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);
        var scCI = new SwapchainCreateInfoKHR { SType = StructureType.SwapchainCreateInfoKhr, Surface = r.Surface, MinImageCount = imageCount, ImageFormat = chosenFmt, ImageColorSpace = ChooseFormat(formats).ColorSpace, ImageExtent = extent, ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit, ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform, CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr, PresentMode = ChoosePresentMode(QueryModes(pd, r.Surface, fnGetModes)), Clipped = Vk.True };
        var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
        SwapchainKHR sc;
        if (createScFn(r.Device, &scCI, null, &sc) != Result.Success) { error = "Swapchain 创建失败。"; return false; }
        r.Swapchain = sc; r.ScOk = true;
        var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
        var localImgCount = 0u; getImgsFn(r.Device, r.Swapchain, &localImgCount, null);
        if (localImgCount == 0) { error = "Swapchain 图像数为 0。"; return false; }
        images = new Image[localImgCount];
        fixed (Image* ip = images) getImgsFn(r.Device, r.Swapchain, &localImgCount, ip);
        imgCount = localImgCount;

        r.ImageViews = new ImageView[localImgCount];
        for (var i = 0; i < localImgCount; i++)
        { var ivCI = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo, Image = images[i], ViewType = ImageViewType.Type2D, Format = chosenFmt, Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity }, SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 } }; if (r.Vk.CreateImageView(r.Device, &ivCI, null, out r.ImageViews[i]) != Result.Success) { error = $"ImageView {i} 创建失败。"; return false; } }
        var depthInfo = VulkanScene3dDepthFormatSelector.Select(r.Vk!, pd);
        if (!depthInfo.IsSupported) { error = depthInfo.Message; return false; }
        r.DepthFormat = depthInfo.ChosenFormat; r.DepthAttachmentCount = (int)localImgCount;
        r.DepthImages = new Image[localImgCount]; r.DepthMemories = new DeviceMemory[localImgCount]; r.DepthViews = new ImageView[localImgCount];
        if (!VulkanScene3dDepthAttachments.Create(r.Vk!, pd, r.Device, extent, depthInfo.ChosenFormat, localImgCount, r.DepthImages, r.DepthMemories, r.DepthViews, out var depthErr)) { error = depthErr; return false; }
        r.DepthOk = true;
        if (!CreateRenderPass(r, chosenFmt, r.DepthFormat)) { error = "RenderPass 创建失败。"; return false; }
        if (!VulkanScene3dShaderModules.Create(r.Vk!, r.Device, out r.VertModule, out r.FragModule, out var shaderErr)) { error = shaderErr; return false; }
        r.VertModOk = true; r.FragModOk = true;
        if (!VulkanScene3dPipelineLayout.Create(r.Vk!, r.Device, pd, out r.PipelineLayout, out var layoutErr)) { error = layoutErr; return false; }
        r.LayoutOk = true;
        if (!VulkanScene3dPipelines.Create(r.Vk!, r.Device, r.RenderPass, r.PipelineLayout, r.VertModule, r.FragModule, extent.Width, extent.Height, out r.GridPipeline, out r.UnitPipeline, out var pipeErr)) { error = pipeErr; return false; }
        r.GridPipeOk = true; r.UnitPipeOk = true;
        if (!VulkanScene3dVertexBuffers.Create(r.Vk!, pd, r.Device, gridVertices, unitVertices, out r.GridBuffer, out r.GridMemory, out r.UnitBuffer, out r.UnitMemory, out gVc, out uVc, out var bufErr)) { error = bufErr; return false; }
        r.GridBufOk = true; r.UnitBufOk = true;
        r.Framebuffers = new Framebuffer[localImgCount];
        var fba = stackalloc ImageView[2];
        for (var i = 0; i < localImgCount; i++)
        { fba[0] = r.ImageViews[i]; fba[1] = r.DepthViews[i]; var fbCI = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = r.RenderPass, AttachmentCount = 2, PAttachments = fba, Width = extent.Width, Height = extent.Height, Layers = 1 }; if (r.Vk.CreateFramebuffer(r.Device, &fbCI, null, out r.Framebuffers[i]) != Result.Success) { error = $"Framebuffer {i} 创建失败。"; return false; } }
        var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
        if (r.Vk.CreateCommandPool(r.Device, &poolCI, null, out r.CommandPool) != Result.Success) { error = "CommandPool 创建失败。"; return false; }
        r.PoolOk = true;
        var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = r.CommandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        if (r.Vk.AllocateCommandBuffers(r.Device, &allocCI, out r.CommandBuffer) != Result.Success) { error = "CommandBuffer 创建失败。"; return false; }
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo }; var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        if (r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemAvail) != Result.Success || r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemFin) != Result.Success || r.Vk.CreateFence(r.Device, &fenceCI, null, out r.Fence) != Result.Success) { error = "同步对象创建失败。"; return false; }
        r.SyncOk = true;
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
    } }

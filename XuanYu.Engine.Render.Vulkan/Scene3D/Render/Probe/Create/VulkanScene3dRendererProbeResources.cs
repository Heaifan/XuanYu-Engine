using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 ImageView / Framebuffer / CommandPool / CommandBuffer / 同步对象创建。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static bool ProbeCreateImageViews(VulkanScene3dRenderResources r, Format chosenFmt, uint imgCount, Image[] images, out string error)
    {
        error = string.Empty;
        r.ImageViews = new ImageView[imgCount];
        for (var i = 0; i < imgCount; i++)
        { var ivCI = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo, Image = images[i], ViewType = ImageViewType.Type2D, Format = chosenFmt, Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity }, SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 } }; if (r.Vk!.CreateImageView(r.Device, &ivCI, null, out r.ImageViews[i]) != Result.Success) { error = $"ImageView {i} 创建失败。"; return false; } }
        return true;
    }

    static bool ProbeCreateFramebuffers(VulkanScene3dRenderResources r, uint imgCount, Extent2D extent, out string error)
    {
        error = string.Empty;
        r.Framebuffers = new Framebuffer[imgCount];
        var fba = stackalloc ImageView[2];
        for (var i = 0; i < imgCount; i++)
        { fba[0] = r.ImageViews[i]; fba[1] = r.DepthViews[i]; var fbCI = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = r.RenderPass, AttachmentCount = 2, PAttachments = fba, Width = extent.Width, Height = extent.Height, Layers = 1 }; if (r.Vk!.CreateFramebuffer(r.Device, &fbCI, null, out r.Framebuffers[i]) != Result.Success) { error = $"Framebuffer {i} 创建失败。"; return false; } }
        return true;
    }

    static bool ProbeCreateSyncObjects(VulkanScene3dRenderResources r, uint qi, out string error)
    {
        error = string.Empty;
        var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
        if (r.Vk!.CreateCommandPool(r.Device, &poolCI, null, out r.CommandPool) != Result.Success) { error = "CommandPool 创建失败。"; return false; }
        r.PoolOk = true;
        var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = r.CommandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        if (r.Vk.AllocateCommandBuffers(r.Device, &allocCI, out r.CommandBuffer) != Result.Success) { error = "CommandBuffer 创建失败。"; return false; }
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo }; var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        if (r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemAvail) != Result.Success || r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemFin) != Result.Success || r.Vk.CreateFence(r.Device, &fenceCI, null, out r.Fence) != Result.Success) { error = "同步对象创建失败。"; return false; }
        r.SyncOk = true;
        return true;
    }
}

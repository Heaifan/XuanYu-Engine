using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Depth;

/// <summary>为每个 Swapchain Image 创建 Depth Image、DeviceMemory 和 ImageView。</summary>
public static unsafe class VulkanScene3dDepthAttachments
{
    public static bool Create(Vk vk, Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        Silk.NET.Vulkan.Device device, Extent2D extent, Format format, uint imageCount,
        Image[] images, DeviceMemory[] memories, ImageView[] views,
        out string errorMessage)
    {
        errorMessage = string.Empty;
        var imageCI = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D, Format = format,
            Extent = new Extent3D(extent.Width, extent.Height, 1), MipLevels = 1, ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.DepthStencilAttachmentBit, SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined
        };
        var aspect = VulkanScene3dDepthFormatSelector.HasStencilComponent(format)
            ? ImageAspectFlags.DepthBit | ImageAspectFlags.StencilBit : ImageAspectFlags.DepthBit;
        for (var i = 0; i < imageCount; i++)
        {
            if (vk.CreateImage(device, &imageCI, null, out images[i]) != Result.Success)
            { errorMessage = $"Scene3D Depth：Image[{i}] 创建失败。"; return false; }
            vk.GetImageMemoryRequirements(device, images[i], out var memReq);
            vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out var memProps);
            var memType = FindDepthMemoryType(memProps, memReq.MemoryTypeBits);
            if (memType == uint.MaxValue)
            { errorMessage = $"Scene3D Depth：Image[{i}] 无 DeviceLocal 内存。"; return false; }
            var allocCI = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = memReq.Size, MemoryTypeIndex = memType };
            if (vk.AllocateMemory(device, &allocCI, null, out memories[i]) != Result.Success)
            { errorMessage = $"Scene3D Depth：Memory[{i}] 分配失败。"; return false; }
            if (vk.BindImageMemory(device, images[i], memories[i], 0) != Result.Success)
            { errorMessage = $"Scene3D Depth：BindImageMemory[{i}] 失败。"; return false; }
            if (!CreateDepthImageView(vk, device, images[i], format, aspect, out views[i]))
            { errorMessage = $"Scene3D Depth：ImageView[{i}] 创建失败。"; return false; }
        }
        return true;
    }

    static uint FindDepthMemoryType(PhysicalDeviceMemoryProperties memProps, uint typeBits)
    {
        for (var t = 0; t < memProps.MemoryTypeCount; t++)
            if ((typeBits & (1u << t)) != 0 && (memProps.MemoryTypes[t].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) == MemoryPropertyFlags.DeviceLocalBit)
                return (uint)t;
        return uint.MaxValue;
    }

    static bool CreateDepthImageView(Vk vk, Silk.NET.Vulkan.Device device, Image image, Format format, ImageAspectFlags aspect, out ImageView view)
    {
        view = default;
        var viewCI = new ImageViewCreateInfo
        {
            SType = StructureType.ImageViewCreateInfo, Image = image, ViewType = ImageViewType.Type2D, Format = format,
            Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
            SubresourceRange = new ImageSubresourceRange { AspectMask = aspect, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
        };
        return vk.CreateImageView(device, &viewCI, null, out view) == Result.Success;
    }
}

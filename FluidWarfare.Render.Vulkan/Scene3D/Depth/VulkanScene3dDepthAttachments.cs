using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Depth;

/// <summary>
/// 为每个 Swapchain Image 创建对应的 Depth Image、DeviceMemory 和 ImageView。
/// 不负责 Pipeline，不持有持久资源，调用者负责将结果存入 RenderResources。
/// </summary>
public static unsafe class VulkanScene3dDepthAttachments
{
    /// <summary>
    /// 为所有 Swapchain Image 创建深度附件（需要 PhysicalDevice 用于 DeviceLocal 内存选择）。
    /// </summary>
    public static bool Create(
        Vk vk, Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        Silk.NET.Vulkan.Device device,
        Extent2D extent, Format format, uint imageCount,
        Image[] images, DeviceMemory[] memories, ImageView[] views,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        var imageCI = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Format = format,
            Extent = new Extent3D(extent.Width, extent.Height, 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.DepthStencilAttachmentBit,
            SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined
        };

        var aspect = VulkanScene3dDepthFormatSelector.HasStencilComponent(format)
            ? ImageAspectFlags.DepthBit | ImageAspectFlags.StencilBit
            : ImageAspectFlags.DepthBit;

        for (var i = 0; i < imageCount; i++)
        {
            // Create Image
            if (vk.CreateImage(device, &imageCI, null, out images[i]) != Result.Success)
            {
                errorMessage = $"Scene3D Depth：Image[{i}] 创建失败。";
                return false;
            }

            // Get memory requirements
            vk.GetImageMemoryRequirements(device, images[i], out var memReq);

            // Find DeviceLocal memory type
            vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out var memProps);
            uint memTypeIndex = uint.MaxValue;
            for (var t = 0; t < memProps.MemoryTypeCount; t++)
            {
                if ((memReq.MemoryTypeBits & (1u << t)) != 0 &&
                    (memProps.MemoryTypes[t].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) == MemoryPropertyFlags.DeviceLocalBit)
                {
                    memTypeIndex = (uint)t;
                    break;
                }
            }

            if (memTypeIndex == uint.MaxValue)
            {
                errorMessage = $"Scene3D Depth：Image[{i}] 无 DeviceLocal 内存类型。";
                return false;
            }

            var allocCI = new MemoryAllocateInfo
            {
                SType = StructureType.MemoryAllocateInfo,
                AllocationSize = memReq.Size,
                MemoryTypeIndex = memTypeIndex
            };

            if (vk.AllocateMemory(device, &allocCI, null, out memories[i]) != Result.Success)
            {
                errorMessage = $"Scene3D Depth：Memory[{i}] 分配失败。";
                return false;
            }

            if (vk.BindImageMemory(device, images[i], memories[i], 0) != Result.Success)
            {
                errorMessage = $"Scene3D Depth：BindImageMemory[{i}] 失败。";
                return false;
            }

            // Create ImageView
            var viewCI = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = images[i],
                ViewType = ImageViewType.Type2D,
                Format = format,
                Components = new ComponentMapping
                {
                    R = ComponentSwizzle.Identity,
                    G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity,
                    A = ComponentSwizzle.Identity
                },
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = aspect,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            if (vk.CreateImageView(device, &viewCI, null, out views[i]) != Result.Success)
            {
                errorMessage = $"Scene3D Depth：ImageView[{i}] 创建失败。";
                return false;
            }
        }

        return true;
    }
}

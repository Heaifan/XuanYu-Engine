using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache
{
    VulkanMapLabelTexture CreateTexture(RenderLabelBitmap bitmap)
    {
        var image = CreateImage(bitmap, out var memory);
        var viewInfo = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo,
            Image = image, ViewType = ImageViewType.Type2D, Format = Format.B8G8R8A8Unorm,
            SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, LevelCount = 1, LayerCount = 1 } };
        if (_vk.CreateImageView(_device.LogicalDevice, &viewInfo, null, out var view) != Result.Success)
            throw new InvalidOperationException("CreateImageView");
        try
        {
            using var staging = VulkanStaticModelBuffer.Create(_vk, _device, bitmap.Pixels,
                BufferUsageFlags.TransferSrcBit, out var error);
            if (staging is null) throw new InvalidOperationException(error);
            Upload(staging.Buffer, image, (uint)bitmap.Width, (uint)bitmap.Height, (uint)(bitmap.Stride / 4));
            var descriptor = AllocateDescriptor(view);
            return new(_vk, _device, image, memory, view, descriptor);
        }
        catch
        {
            _vk.DestroyImageView(_device.LogicalDevice, view, null);
            _vk.DestroyImage(_device.LogicalDevice, image, null);
            _vk.FreeMemory(_device.LogicalDevice, memory, null);
            throw;
        }
    }

    Image CreateImage(RenderLabelBitmap bitmap, out DeviceMemory memory)
    {
        var info = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D, Format = Format.B8G8R8A8Unorm,
            Extent = new Extent3D((uint)bitmap.Width, (uint)bitmap.Height, 1), MipLevels = 1,
            ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit, SharingMode = SharingMode.Exclusive };
        Check(_vk.CreateImage(_device.LogicalDevice, &info, null, out var image), "CreateImage");
        _vk.GetImageMemoryRequirements(_device.LogicalDevice, image, out var requirements);
        var type = FindMemoryType(requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit);
        if (type < 0) throw new InvalidOperationException("DeviceLocal memory type");
        var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size, MemoryTypeIndex = (uint)type };
        Check(_vk.AllocateMemory(_device.LogicalDevice, &allocate, null, out memory), "AllocateImageMemory");
        Check(_vk.BindImageMemory(_device.LogicalDevice, image, memory, 0), "BindImageMemory");
        return image;
    }

    int FindMemoryType(uint bits, MemoryPropertyFlags flags)
    {
        _vk.GetPhysicalDeviceMemoryProperties(_device.PhysicalDevice, out var props);
        for (var i = 0; i < props.MemoryTypeCount; i++)
            if ((bits & (1u << i)) != 0 && (props.MemoryTypes[i].PropertyFlags & flags) == flags) return i;
        return -1;
    }

    static void Check(Result result, string operation)
    { if (result != Result.Success) throw new InvalidOperationException($"{operation}: {result}"); }
}

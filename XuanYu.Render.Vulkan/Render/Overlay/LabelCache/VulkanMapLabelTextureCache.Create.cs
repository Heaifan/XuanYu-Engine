using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache
{
    VulkanMapLabelTexture CreateTexture(RenderLabelBitmap bitmap)
    {
        Image image = default; DeviceMemory memory = default; ImageView view = default;
        VulkanStaticModelBuffer? staging = null;
        try
        {
            image = CreateImage(bitmap, out memory);
            var viewInfo = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo,
                Image = image, ViewType = ImageViewType.Type2D, Format = Format.B8G8R8A8Unorm,
                SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, LevelCount = 1, LayerCount = 1 } };
            Check(_vk.CreateImageView(_device.LogicalDevice, &viewInfo, null, out view), "CreateImageView");
            staging = VulkanStaticModelBuffer.Create(_vk, _device, bitmap.Pixels,
                BufferUsageFlags.TransferSrcBit, out var error);
            if (staging is null) throw new InvalidOperationException(error);
            Upload(staging.Buffer, image, (uint)bitmap.Width, (uint)bitmap.Height, (uint)(bitmap.Stride / 4));
            var descriptor = AllocateDescriptor(view);
            var texture = new VulkanMapLabelTexture(_vk, _device, image, memory,
                view, descriptor, FreeDescriptor);
            image = default; memory = default; view = default;
            return texture;
        }
        catch (VulkanLabelUploadCompletionUnknownException error)
        {
            if (staging is null) throw;
            _uploadQuarantines.Add(new(error.Command, error.Resolution, staging, view, image, memory));
            staging = null; view = default; image = default; memory = default;
            throw;
        }
        catch
        {
            if (view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, view, null);
            if (image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, image, null);
            if (memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, memory, null);
            throw;
        }
        finally { staging?.Dispose(); }
    }

    Image CreateImage(RenderLabelBitmap bitmap, out DeviceMemory memory)
    {
        Image image = default; memory = default;
        var info = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D, Format = Format.B8G8R8A8Unorm,
            Extent = new Extent3D((uint)bitmap.Width, (uint)bitmap.Height, 1), MipLevels = 1,
            ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit, SharingMode = SharingMode.Exclusive };
        try
        {
            Check(_vk.CreateImage(_device.LogicalDevice, &info, null, out image), "CreateImage");
            _vk.GetImageMemoryRequirements(_device.LogicalDevice, image, out var requirements);
            var type = FindMemoryType(requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit);
            if (type < 0) throw new InvalidOperationException("DeviceLocal memory type");
            var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
                AllocationSize = requirements.Size, MemoryTypeIndex = (uint)type };
            Check(_vk.AllocateMemory(_device.LogicalDevice, &allocate, null, out memory), "AllocateImageMemory");
            Check(_vk.BindImageMemory(_device.LogicalDevice, image, memory, 0), "BindImageMemory");
            return image;
        }
        catch
        {
            if (image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, image, null);
            if (memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, memory, null);
            throw;
        }
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

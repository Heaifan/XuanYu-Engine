using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render;

sealed unsafe class VulkanDepthAttachment : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device;
    Image _image; DeviceMemory _memory; ImageView _view;
    public const Format DepthFormat = Format.D32Sfloat;
    VulkanDepthAttachment(Vk vk, VulkanDeviceOwner device, Image image, DeviceMemory memory, ImageView view) =>
        (_vk, _device, _image, _memory, _view) = (vk, device, image, memory, view);
    public ImageView View => _view;

    public static VulkanDepthAttachment? Create(Vk vk, VulkanDeviceOwner device, Extent2D extent)
    {
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D, Format = DepthFormat,
            Extent = new Extent3D { Width = extent.Width, Height = extent.Height, Depth = 1 },
            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal, Usage = ImageUsageFlags.DepthStencilAttachmentBit,
            SharingMode = SharingMode.Exclusive, InitialLayout = ImageLayout.Undefined
        };
        if (vk.CreateImage(device.LogicalDevice, &imageInfo, null, out var image) != Result.Success) return null;
        if (!Allocate(vk, device, image, out var memory))
        { vk.DestroyImage(device.LogicalDevice, image, null); return null; }
        if (vk.BindImageMemory(device.LogicalDevice, image, memory, 0) != Result.Success)
        { vk.FreeMemory(device.LogicalDevice, memory, null); vk.DestroyImage(device.LogicalDevice, image, null); return null; }
        var range = new ImageSubresourceRange { AspectMask = ImageAspectFlags.DepthBit, LevelCount = 1, LayerCount = 1 };
        var viewInfo = new ImageViewCreateInfo
        {
            SType = StructureType.ImageViewCreateInfo, Image = image, ViewType = ImageViewType.Type2D,
            Format = DepthFormat, SubresourceRange = range
        };
        if (vk.CreateImageView(device.LogicalDevice, &viewInfo, null, out var view) == Result.Success)
            return new VulkanDepthAttachment(vk, device, image, memory, view);
        vk.FreeMemory(device.LogicalDevice, memory, null);
        vk.DestroyImage(device.LogicalDevice, image, null);
        return null;
    }

    static bool Allocate(Vk vk, VulkanDeviceOwner d, Image image, out DeviceMemory mem)
    {
        mem = default;
        vk.GetImageMemoryRequirements(d.LogicalDevice, image, out var req);
        var type = FindMemoryType(vk, d, req.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit);
        if (type < 0) return false;
        var info = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = req.Size, MemoryTypeIndex = (uint)type };
        return vk.AllocateMemory(d.LogicalDevice, &info, null, out mem) == Result.Success;
    }

    static int FindMemoryType(Vk vk, VulkanDeviceOwner d, uint bits, MemoryPropertyFlags flags)
    {
        vk.GetPhysicalDeviceMemoryProperties(d.PhysicalDevice, out var props);
        for (var i = 0; i < props.MemoryTypeCount; i++)
            if ((bits & (1u << i)) != 0 && (props.MemoryTypes[i].PropertyFlags & flags) == flags) return i;
        return -1;
    }

    public void Dispose()
    {
        if (_view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, _view, null);
        if (_image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, _image, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
        _view = default; _image = default; _memory = default;
    }
}

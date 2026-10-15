using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Readback;

internal sealed unsafe class VulkanOffscreenColorTarget : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _device;
    Image _image;
    DeviceMemory _memory;
    ImageView _view;

    VulkanOffscreenColorTarget(Vk vk, VulkanDeviceOwner device, Image image,
        DeviceMemory memory, ImageView view, Extent2D extent, Format format) =>
        (_vk, _device, _image, _memory, _view, Extent, Format) =
        (vk, device, image, memory, view, extent, format);

    public Extent2D Extent { get; }
    public Format Format { get; }
    public Image Image => _image;
    public ImageView View => _view;

    public static VulkanOffscreenColorTarget Create(Vk vk, VulkanDeviceOwner device,
        uint width, uint height)
    {
        if (width == 0 || height == 0) throw new ArgumentOutOfRangeException(nameof(width));
        const Format format = Format.R8G8B8A8Unorm;
        vk.GetPhysicalDeviceFormatProperties(device.PhysicalDevice, format, out var support);
        var required = FormatFeatureFlags.ColorAttachmentBit | FormatFeatureFlags.TransferSrcBit;
        if ((support.OptimalTilingFeatures & required) != required)
            throw new NotSupportedException($"离屏格式 {format} 不支持 ColorAttachment + TransferSrc");

        var extent = new Extent2D(width, height);
        var imageInfo = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D, Format = format, Extent = new Extent3D(width, height, 1),
            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal, Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferSrcBit,
            SharingMode = SharingMode.Exclusive, InitialLayout = ImageLayout.Undefined };
        Check(vk.CreateImage(device.LogicalDevice, &imageInfo, null, out var image), "CreateImage");
        DeviceMemory memory = default;
        ImageView view = default;
        try
        {
            vk.GetImageMemoryRequirements(device.LogicalDevice, image, out var requirements);
            var type = FindDeviceLocalMemoryType(vk, device, requirements.MemoryTypeBits);
            if (type < 0) throw new NotSupportedException("离屏图像没有 DeviceLocal 内存类型");
            var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
                AllocationSize = requirements.Size, MemoryTypeIndex = (uint)type };
            Check(vk.AllocateMemory(device.LogicalDevice, &allocation, null, out memory), "AllocateMemory");
            Check(vk.BindImageMemory(device.LogicalDevice, image, memory, 0), "BindImageMemory");
            var range = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit,
                LevelCount = 1, LayerCount = 1 };
            var viewInfo = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo,
                Image = image, ViewType = ImageViewType.Type2D, Format = format, SubresourceRange = range };
            Check(vk.CreateImageView(device.LogicalDevice, &viewInfo, null, out view), "CreateImageView");
            return new VulkanOffscreenColorTarget(vk, device, image, memory, view, extent, format);
        }
        catch
        {
            if (view.Handle != 0) vk.DestroyImageView(device.LogicalDevice, view, null);
            if (memory.Handle != 0) vk.FreeMemory(device.LogicalDevice, memory, null);
            vk.DestroyImage(device.LogicalDevice, image, null);
            throw;
        }
    }

    static int FindDeviceLocalMemoryType(Vk vk, VulkanDeviceOwner device, uint bits)
    {
        vk.GetPhysicalDeviceMemoryProperties(device.PhysicalDevice, out var properties);
        for (var i = 0; i < properties.MemoryTypeCount; i++)
            if ((bits & (1u << i)) != 0 &&
                properties.MemoryTypes[i].PropertyFlags.HasFlag(MemoryPropertyFlags.DeviceLocalBit)) return i;
        return -1;
    }

    static void Check(Result result, string operation)
    {
        if (result != Result.Success) throw new InvalidOperationException($"Vulkan {operation}: {result}");
    }

    public void Dispose()
    {
        if (_view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, _view, null);
        if (_image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, _image, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
        _view = default; _image = default; _memory = default;
    }
}

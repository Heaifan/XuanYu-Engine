using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Viewport.CompositionSpike;

unsafe sealed class VulkanImage : IDisposable
{
    readonly VulkanContext _context; readonly DeviceMemory _memory; readonly ImageView _view;
    public Image Handle { get; } public PixelSize Size { get; } public ulong MemorySize { get; }

    public VulkanImage(VulkanContext context, PixelSize size)
    {
        _context = context; Size = size; var external = new ExternalMemoryImageCreateInfo { SType = StructureType.ExternalMemoryImageCreateInfo, HandleTypes = ExternalMemoryHandleTypeFlags.OpaqueWin32Bit };
        var usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferDstBit | ImageUsageFlags.TransferSrcBit | ImageUsageFlags.SampledBit;
        var create = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, PNext = &external, ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm, Extent = new Extent3D((uint)size.Width, (uint)size.Height, 1), MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal, Usage = usage, SharingMode = SharingMode.Exclusive, InitialLayout = ImageLayout.Undefined, Flags = ImageCreateFlags.CreateMutableFormatBit };
        context.Api.CreateImage(context.Device, in create, null, out var image).Ensure(); Handle = image; context.Api.GetImageMemoryRequirements(context.Device, image, out var requirements);
        var dedicated = new MemoryDedicatedAllocateInfoKHR { SType = StructureType.MemoryDedicatedAllocateInfoKhr, Image = image }; var export = new ExportMemoryAllocateInfo { SType = StructureType.ExportMemoryAllocateInfo, PNext = &dedicated, HandleTypes = ExternalMemoryHandleTypeFlags.OpaqueWin32Bit };
        var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, PNext = &export, AllocationSize = requirements.Size, MemoryTypeIndex = FindMemoryType(requirements.MemoryTypeBits) }; context.Api.AllocateMemory(context.Device, in allocate, null, out _memory).Ensure(); MemorySize = requirements.Size; context.Api.BindImageMemory(context.Device, image, _memory, 0).Ensure();
        var range = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1); var viewInfo = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo, Image = image, ViewType = ImageViewType.Type2D, Format = Format.R8G8B8A8Unorm, SubresourceRange = range }; context.Api.CreateImageView(context.Device, in viewInfo, null, out _view).Ensure();
    }

    uint FindMemoryType(uint bits)
    { PhysicalDeviceMemoryProperties props = default; _context.Api.GetPhysicalDeviceMemoryProperties(_context.PhysicalDevice, out props); for (uint i = 0; i < props.MemoryTypeCount; i++) if ((bits & (1u << (int)i)) != 0 && props.MemoryTypes[(int)i].PropertyFlags.HasFlag(MemoryPropertyFlags.DeviceLocalBit)) return i; throw new InvalidOperationException("Vulkan image has no device-local memory type"); }

    public IPlatformHandle Export()
    { if (!_context.Api.TryGetDeviceExtension<KhrExternalMemoryWin32>(_context.Instance, _context.Device, out var ext)) throw new InvalidOperationException("缺 VK_KHR_external_memory_win32"); var info = new MemoryGetWin32HandleInfoKHR { SType = StructureType.MemoryGetWin32HandleInfoKhr, Memory = _memory, HandleType = ExternalMemoryHandleTypeFlags.OpaqueWin32Bit }; ext.GetMemoryWin32Handle(_context.Device, in info, out var handle).Ensure(); return new PlatformHandle(handle, KnownPlatformGraphicsExternalImageHandleTypes.VulkanOpaqueNtHandle); }

    public void Dispose() { _context.Api.DestroyImageView(_context.Device, _view, null); _context.Api.DestroyImage(_context.Device, Handle, null); _context.Api.FreeMemory(_context.Device, _memory, null); }
}

using System.Runtime.InteropServices;
using Avalonia.Platform;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Viewport.CompositionSpike;

unsafe sealed class VulkanSemaphorePair : IDisposable
{
    readonly VulkanContext _context;
    public VkSemaphore Available { get; private set; }
    public VkSemaphore Complete { get; private set; }

    public VulkanSemaphorePair(VulkanContext context)
    {
        _context = context;
        var export = new ExportSemaphoreCreateInfo { SType = StructureType.ExportSemaphoreCreateInfo, HandleTypes = ExternalSemaphoreHandleTypeFlags.OpaqueWin32Bit };
        var create = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo, PNext = &export };
        context.Api.CreateSemaphore(context.Device, in create, null, out var available).Ensure();
        context.Api.CreateSemaphore(context.Device, in create, null, out var complete).Ensure();
        Available = available; Complete = complete;
    }

    public IPlatformHandle Export(bool complete)
    {
        if (!_context.Api.TryGetDeviceExtension<KhrExternalSemaphoreWin32>(_context.Instance, _context.Device, out var ext))
            throw new InvalidOperationException("缺 VK_KHR_external_semaphore_win32");
        var info = new SemaphoreGetWin32HandleInfoKHR { SType = StructureType.SemaphoreGetWin32HandleInfoKhr, Semaphore = complete ? Complete : Available, HandleType = ExternalSemaphoreHandleTypeFlags.OpaqueWin32Bit };
        ext.GetSemaphoreWin32Handle(_context.Device, in info, out var handle).Ensure();
        return new PlatformHandle(handle, KnownPlatformGraphicsExternalSemaphoreHandleTypes.VulkanOpaqueNtHandle);
    }

    public void Dispose()
    { _context.Api.DestroySemaphore(_context.Device, Available, null); _context.Api.DestroySemaphore(_context.Device, Complete, null); Available = default; Complete = default; }
}

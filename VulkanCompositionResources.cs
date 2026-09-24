using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Vulkan;

namespace XuanYu.Viewport.CompositionSpike;

sealed class VulkanCompositionResources : IAsyncDisposable
{
    readonly VulkanContext _context;
    readonly VulkanImage _image;
    readonly VulkanSemaphorePair _semaphores;
    readonly ICompositionImportedGpuImage _imported;
    readonly ICompositionImportedGpuSemaphore _available;
    readonly ICompositionImportedGpuSemaphore _complete;
    public string DeviceInfo { get; }

    VulkanCompositionResources(VulkanContext context, VulkanImage image, VulkanSemaphorePair semaphores,
        ICompositionImportedGpuImage imported, ICompositionImportedGpuSemaphore available, ICompositionImportedGpuSemaphore complete)
    { _context = context; _image = image; _semaphores = semaphores; _imported = imported; _available = available; _complete = complete; DeviceInfo = $"GPU={context.DeviceName}; Driver={context.Driver}; Layout=TransferSrc; Sync=VulkanSemaphore"; }

    public static VulkanCompositionResources Create(ICompositionGpuInterop interop, PixelSize size)
    {
        var result = VulkanContext.Create(interop);
        if (result.Context is null) throw new InvalidOperationException(result.Info);
        var image = new VulkanImage(result.Context, size);
        var semaphores = new VulkanSemaphorePair(result.Context);
        result.Context.Commands.Clear(image.Handle, Silk.NET.Vulkan.ImageLayout.Undefined, Silk.NET.Vulkan.ImageLayout.TransferSrcOptimal, semaphores.Complete);
        var imported = interop.ImportImage(image.Export(), new PlatformGraphicsExternalImageProperties
        { Format = PlatformGraphicsExternalImageFormat.R8G8B8A8UNorm, Width = size.Width, Height = size.Height, MemorySize = image.MemorySize,
          MemoryOffset = 0, TopLeftOrigin = true,
          VulkanProperties = new PlatformGraphicsExternalImageVulkanProperties { Layout = (int)Silk.NET.Vulkan.ImageLayout.TransferSrcOptimal } });
        var available = interop.ImportSemaphore(semaphores.Export(false));
        var complete = interop.ImportSemaphore(semaphores.Export(true));
        Console.WriteLine($"[A1.5] ExternalMemory=VulkanOpaqueNtHandle; MemorySize={image.MemorySize}; ImageSize={size}; ImageLayout=TransferSrcOptimal; Sync=VulkanOpaqueNtHandle");
        return new VulkanCompositionResources(result.Context, image, semaphores, imported, available, complete);
    }

    public async Task PresentAsync(CompositionDrawingSurface surface) => await surface.UpdateWithSemaphoresAsync(_imported, _complete, _available);

    public async ValueTask DisposeAsync()
    { await _imported.DisposeAsync(); await _available.DisposeAsync(); await _complete.DisposeAsync(); _semaphores.Dispose(); _image.Dispose(); _context.Dispose(); }
}

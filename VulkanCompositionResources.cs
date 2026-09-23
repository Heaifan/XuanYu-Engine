using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;

namespace XuanYu.Viewport.CompositionSpike;

sealed class VulkanCompositionResources : IAsyncDisposable
{
    readonly VulkanContext _context;
    readonly VulkanImage _image;
    readonly ICompositionImportedGpuImage _imported;
    public string DeviceInfo { get; }

    VulkanCompositionResources(VulkanContext context, VulkanImage image, ICompositionImportedGpuImage imported)
    { _context = context; _image = image; _imported = imported; DeviceInfo = $"GPU={context.DeviceName}; Driver={context.Driver}; Layout=TransferSrc; Sync=QueueWaitIdle+UpdateAsync"; }

    public static VulkanCompositionResources Create(ICompositionGpuInterop interop, PixelSize size)
    {
        var result = VulkanContext.Create(interop);
        if (result.Context is null) throw new InvalidOperationException(result.Info);
        var image = new VulkanImage(result.Context, size);
        result.Context.Commands.Clear(image.Handle, Silk.NET.Vulkan.ImageLayout.Undefined, Silk.NET.Vulkan.ImageLayout.TransferSrcOptimal);
        var imported = interop.ImportImage(image.Export(), new PlatformGraphicsExternalImageProperties
        { Format = PlatformGraphicsExternalImageFormat.R8G8B8A8UNorm, Width = size.Width, Height = size.Height, MemorySize = image.MemorySize });
        return new VulkanCompositionResources(result.Context, image, imported);
    }

    public async Task PresentAsync(CompositionDrawingSurface surface) => await surface.UpdateAsync(_imported);

    public async ValueTask DisposeAsync()
    { await _imported.DisposeAsync(); _image.Dispose(); _context.Dispose(); }
}

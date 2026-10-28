using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain;

public sealed unsafe partial class VulkanSwapchainOwner
{
    RendererEventSession? _events;
    internal RendererEventSession? EventSession => _events;

    internal void AttachEventSession(RendererEventSession? events)
    {
        _events = events;
        PublishSwapchain(RendererEventCodes.SwapchainCreated, RendererEventLevel.Info);
    }

    internal void PublishSkipped(int width, int height, string reason) =>
        PublishSwapchain(RendererEventCodes.SwapchainSkipped, RendererEventLevel.Warning,
            width, height, reason);

    internal void PublishRecreated(int width, int height) =>
        PublishSwapchain(RendererEventCodes.SwapchainRecreated, RendererEventLevel.Info,
            width, height);

    internal void PublishFailure(int width, int height, string reason) =>
        PublishSwapchain(RendererEventCodes.SwapchainFailed, RendererEventLevel.Error,
            width, height, reason);

    internal void PublishReleased() =>
        PublishSwapchain(RendererEventCodes.SwapchainReleased, RendererEventLevel.Info);

    void PublishSwapchain(string code, RendererEventLevel level, int? width = null,
        int? height = null, string? reason = null) => _events?.PublishLifecycleEvent(code, level,
            _resourceGeneration, width, height, _extent.Width, _extent.Height, _imageViews.Length,
            reason: reason);

    public Format Format => _format;
    public Extent2D Extent => _extent;
    public uint ResourceGeneration => _resourceGeneration;
    public ReadOnlySpan<ImageView> ImageViews => _imageViews;
    public ReadOnlySpan<Image> Images => _images;
    public bool ReadbackEnabled => _readbackEnabled;
    public string FormatName => _format.ToString();
    public SwapchainKHR Swapchain => _swapchain;
    public bool HasUsableSwapchain => _swapchain.Handle != 0 && !_disposed;
    public KhrSwapchain Khr => _khr!;

    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}

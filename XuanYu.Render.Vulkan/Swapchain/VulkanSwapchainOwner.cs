using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Swapchain.Diagnostics;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain;
public sealed unsafe partial class VulkanSwapchainOwner : IDisposable
{
    readonly Vk _vk;
    readonly Instance _instance;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly SurfaceKHR _surface;
    readonly PhysicalDevice _physicalDevice;
    readonly Action<string>? _log;
    KhrSwapchain? _khr;
    SwapchainKHR _swapchain;
    Image[] _images = [];
    ImageView[] _imageViews = [];
    Format _format;
    Extent2D _extent;
    uint _resourceGeneration;
    bool _readbackEnabled;
    bool _disposed;
    public const string DeviceExtensionName = "VK_KHR_swapchain"; // VK4-C：LogicalDevice 创建时必须启用的设备扩展
    internal VulkanSwapchainOwner(Vk vk, Instance instance, VulkanDeviceOwner deviceOwner, SurfaceKHR surface,
        PhysicalDevice physicalDevice, KhrSwapchain khr, SwapchainKHR swapchain, Image[] images, ImageView[] views, Format format, Extent2D extent, bool readbackEnabled, Action<string>? log)
    {
        _vk = vk; _instance = instance; _deviceOwner = deviceOwner; _surface = surface;
        _physicalDevice = physicalDevice; _khr = khr; _swapchain = swapchain; _images = images; _imageViews = views; _format = format; _extent = extent; _readbackEnabled = readbackEnabled; _log = log;
    }
    public bool Recreate(int width, int height, uint generation = 0, Func<bool>? retireCurrentGeneration = null)
    {
        if (_khr is null) return false;
        if (width <= 0 || height <= 0)
        { PublishSkipped(width, height, "尺寸为零"); Log(_log, VulkanSwapchainLogFormatter.Skipped($"0 尺寸跳过重建（{width}x{height}）")); return false; }
        if (_swapchain.Handle != 0 && _extent.Width == (uint)width && _extent.Height == (uint)height)
        { PublishSkipped(width, height, "请求尺寸未变化"); Log(_log, VulkanSwapchainLogFormatter.Skipped($"同尺寸跳过重建（{width}x{height}）")); return true; }
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 重建开始", $"请求尺寸={width}x{height}"));
        PublishSwapchain(XuanYu.Render.Abstractions.Diagnostics.RendererEventCodes.SwapchainCreating,
            XuanYu.Render.Abstractions.Diagnostics.RendererEventLevel.Info, width, height);
        try
        {
            if (retireCurrentGeneration is not null && !retireCurrentGeneration())
            { PublishFailure(width, height, "旧 generation 未满足安全退休条件"); return false; }
            var (swapchain, images, views, format, extent, ok, readbackEnabled) = VulkanSwapchainBuilder.Build(_vk, _instance, _physicalDevice, _surface, _khr, _deviceOwner.LogicalDevice, width, height, _log, _swapchain, _events);
            if (!ok) { PublishFailure(width, height, "构建器未能创建 Swapchain"); return false; }
            DestroyImagesAndViews();
            _swapchain = swapchain; _images = images; _imageViews = views; _format = format; _extent = extent; _readbackEnabled = readbackEnabled;
            _resourceGeneration++;
            PublishRecreated(width, height);
            Log(_log, VulkanSwapchainLogFormatter.Recreated(_extent, views.Length));
            _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 重建完成", $"物理尺寸={extent.Width}x{extent.Height}"));
            return true;
        }
        catch (Exception ex) { PublishFailure(width, height, ex.Message); Log(_log, VulkanSwapchainLogFormatter.Failed($"重建异常：{ex.Message}")); return false; }
    }
    public bool TryRecreateToCurrent(out Extent2D newExtent, out bool rebuilt, uint generation = 0, Func<bool>? retireCurrentGeneration = null)
    {
        newExtent = _extent;
        rebuilt = false;
        if (_khr is null) return false;
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 自愈查询", $"旧物理尺寸={_extent.Width}x{_extent.Height}；查询 Surface"));
        var caps = VulkanSwapchainCapabilities.Query(_vk, _instance, _physicalDevice, _surface, (int)_extent.Width, (int)_extent.Height, _log);
        if (!caps.Success || caps.Caps is null || caps.Caps.Value.Extent.Width == 0 || caps.Caps.Value.Extent.Height == 0 || caps.Caps.Value.Extent.Width == uint.MaxValue) return false;
        if (caps.Caps.Value.Extent.Width == _extent.Width && caps.Caps.Value.Extent.Height == _extent.Height) { newExtent = _extent; Log(_log, VulkanSwapchainLogFormatter.Skipped($"同尺寸跳过自愈重建（{_extent.Width}x{_extent.Height}）")); return true; }
        if (!Recreate((int)caps.Caps.Value.Extent.Width, (int)caps.Caps.Value.Extent.Height, generation, retireCurrentGeneration)) return false;
        newExtent = _extent;
        rebuilt = true;
        return true;
    }

    void DestroyImagesAndViews()
    {
        foreach (var v in _imageViews) if (v.Handle != 0) _vk.DestroyImageView(_deviceOwner.LogicalDevice, v, null);
        if (_swapchain.Handle != 0) _khr?.DestroySwapchain(_deviceOwner.LogicalDevice, _swapchain, null);
        _imageViews = []; _images = []; _swapchain = default;
    }
    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        DestroyImagesAndViews();
        PublishReleased();
        Log(_log, VulkanSwapchainLogFormatter.Disposed());
    }
}

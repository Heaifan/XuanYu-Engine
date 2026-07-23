using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 持有者（创建/重建/释放）。RZ-VK5-D-R1：Recreate 内部加 T+ 阶段日志。
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
    bool _disposed;

    public const string DeviceExtensionName = "VK_KHR_swapchain"; // VK4-C：LogicalDevice 创建时必须启用的设备扩展

    VulkanSwapchainOwner(Vk vk, Instance instance, VulkanDeviceOwner deviceOwner, SurfaceKHR surface,
        PhysicalDevice physicalDevice, KhrSwapchain khr, SwapchainKHR swapchain, Image[] images, ImageView[] views, Format format, Extent2D extent, Action<string>? log)
    {
        _vk = vk; _instance = instance; _deviceOwner = deviceOwner; _surface = surface;
        _physicalDevice = physicalDevice; _khr = khr; _swapchain = swapchain; _images = images; _imageViews = views; _format = format; _extent = extent; _log = log;
    }

    public static VulkanSwapchainOwner? Create(Vk vk, Instance instance, VulkanDeviceOwner? deviceOwner,
        SurfaceKHR surface, PhysicalDevice physicalDevice, int width, int height, Action<string>? log)
    {
        if (deviceOwner is null) { Log(log, VulkanSwapchainLogFormatter.Skipped("LogicalDevice 不可用")); return null; }
        if (!vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice, out KhrSwapchain? khr) || khr is null)
        { Log(log, VulkanSwapchainLogFormatter.Failed("缺 VK_KHR_swapchain 设备扩展")); return null; }
        try
        {
        var (swapchain, images, views, format, extent, ok) = VulkanSwapchainBuilder.Build(vk, instance, physicalDevice, surface, khr, deviceOwner.LogicalDevice, width, height, log);
        if (!ok) return null;
        Log(log, VulkanSwapchainLogFormatter.Created(extent, views.Length));
        return new VulkanSwapchainOwner(vk, instance, deviceOwner, surface, physicalDevice, khr, swapchain, images, views, format, extent, log);
        }
        catch (Exception ex) { Log(log, VulkanSwapchainLogFormatter.Failed($"创建异常：{ex.Message}")); return null; }
    }

    public bool Recreate(int width, int height, uint generation = 0)
    {
        if (_khr is null) return false;
        if (width <= 0 || height <= 0) { Log(_log, VulkanSwapchainLogFormatter.Skipped($"0 尺寸跳过重建（{width}x{height}）")); return false; }
        if (_swapchain.Handle != 0 && _extent.Width == (uint)width && _extent.Height == (uint)height) { Log(_log, VulkanSwapchainLogFormatter.Skipped($"同尺寸跳过重建（{width}x{height}）")); return true; }
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 重建开始", $"请求尺寸={width}x{height}"));
        try
        {
            var (swapchain, images, views, format, extent, ok) = VulkanSwapchainBuilder.Build(_vk, _instance, _physicalDevice, _surface, _khr, _deviceOwner.LogicalDevice, width, height, _log, _swapchain);
            if (!ok) return false;
            DestroyImagesAndViews();
            _swapchain = swapchain; _images = images; _imageViews = views; _format = format; _extent = extent;
            _resourceGeneration++;
            Log(_log, VulkanSwapchainLogFormatter.Recreated(_extent, views.Length));
            _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 重建完成", $"物理尺寸={extent.Width}x{extent.Height}"));
            return true;
        }
        catch (Exception ex) { Log(_log, VulkanSwapchainLogFormatter.Failed($"重建异常：{ex.Message}")); return false; }
    }

    public bool TryRecreateToCurrent(out Extent2D newExtent, out bool rebuilt, uint generation = 0)
    {
        newExtent = _extent;
        rebuilt = false;
        if (_khr is null) return false;
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Swapchain 自愈查询", $"旧物理尺寸={_extent.Width}x{_extent.Height}；查询 Surface"));
        var caps = VulkanSwapchainCapabilities.Query(_vk, _instance, _physicalDevice, _surface, (int)_extent.Width, (int)_extent.Height, _log);
        if (!caps.Success || caps.Caps is null || caps.Caps.Value.Extent.Width == 0 || caps.Caps.Value.Extent.Height == 0 || caps.Caps.Value.Extent.Width == uint.MaxValue) return false;
        if (caps.Caps.Value.Extent.Width == _extent.Width && caps.Caps.Value.Extent.Height == _extent.Height) { newExtent = _extent; Log(_log, VulkanSwapchainLogFormatter.Skipped($"同尺寸跳过自愈重建（{_extent.Width}x{_extent.Height}）")); return true; }
        if (!Recreate((int)caps.Caps.Value.Extent.Width, (int)caps.Caps.Value.Extent.Height, generation)) return false;
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
        Log(_log, VulkanSwapchainLogFormatter.Disposed());
    }
}

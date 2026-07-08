using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain + Images + ImageViews 持有者。仅创建/重建/释放；
// 不建 RenderPass / Framebuffer / CommandPool / CommandBuffer，不清屏 / Present。
public sealed unsafe class VulkanSwapchainOwner : IDisposable
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
    bool _disposed;

    VulkanSwapchainOwner(Vk vk, Instance instance, VulkanDeviceOwner deviceOwner, SurfaceKHR surface,
        PhysicalDevice physicalDevice, KhrSwapchain khr, SwapchainKHR swapchain, Image[] images, ImageView[] views, Action<string>? log)
    {
        _vk = vk; _instance = instance; _deviceOwner = deviceOwner; _surface = surface;
        _physicalDevice = physicalDevice; _khr = khr; _swapchain = swapchain; _images = images; _imageViews = views; _log = log;
    }

    public static VulkanSwapchainOwner? Create(Vk vk, Instance instance, VulkanDeviceOwner? deviceOwner,
        SurfaceKHR surface, PhysicalDevice physicalDevice, int width, int height, Action<string>? log)
    {
        if (deviceOwner is null) { Log(log, VulkanSwapchainLogFormatter.Skipped("LogicalDevice 不可用")); return null; }
        if (!vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice, out KhrSwapchain? khr) || khr is null)
        { Log(log, VulkanSwapchainLogFormatter.Failed("缺 VK_KHR_swapchain 设备扩展")); return null; }
        try
        {
            var (swapchain, images, views, ok) = VulkanSwapchainBuilder.Build(vk, instance, physicalDevice, surface, khr, deviceOwner.LogicalDevice, width, height, log);
            if (!ok) return null;
            Log(log, VulkanSwapchainLogFormatter.Created(views.Length));
            return new VulkanSwapchainOwner(vk, instance, deviceOwner, surface, physicalDevice, khr, swapchain, images, views, log);
        }
        catch (Exception ex) { Log(log, VulkanSwapchainLogFormatter.Failed($"创建异常：{ex.Message}")); return null; }
    }

    public void Recreate(int width, int height)
    {
        if (_khr is null) return;
        try
        {
            var (swapchain, images, views, ok) = VulkanSwapchainBuilder.Build(_vk, _instance, _physicalDevice, _surface, _khr, _deviceOwner.LogicalDevice, width, height, _log);
            if (!ok) return;
            DestroyImagesAndViews();
            _swapchain = swapchain; _images = images; _imageViews = views;
            Log(_log, VulkanSwapchainLogFormatter.Recreated((uint)width, (uint)height, views.Length));
        }
        catch (Exception ex) { Log(_log, VulkanSwapchainLogFormatter.Failed($"重建异常：{ex.Message}")); }
    }

    void DestroyImagesAndViews()
    {
        foreach (var v in _imageViews) if (v.Handle != 0) _vk.DestroyImageView(_deviceOwner.LogicalDevice, v, null);
        if (_swapchain.Handle != 0) _khr?.DestroySwapchain(_deviceOwner.LogicalDevice, _swapchain, null);
        _imageViews = []; _images = []; _swapchain = default;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DestroyImagesAndViews();
        Log(_log, VulkanSwapchainLogFormatter.Disposed());
    }

    static void Log(Action<string>? log, string m) { log?.Invoke(m); Console.WriteLine(m); }
}

using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Session;

// VK4-D：薄组合根。只装配 ClearFrame + PresentLoop，不写 Vulkan 细节。
// Bridge 仅委托 Attach/Resize/Detach；Detach 顺序 ClearFrame → Swapchain → ...（由 Bridge 保证后续）。
public sealed class VulkanRenderSession : IDisposable
{
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly VulkanClearFrameOwner _clearFrame;
    readonly VulkanPresentLoop _presentLoop;
    readonly Action<string>? _log;
    bool _disposed;

    VulkanRenderSession(VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, VulkanPresentLoop presentLoop, Action<string>? log)
    {
        _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _clearFrame = clearFrame; _presentLoop = presentLoop; _log = log;
    }

    public static VulkanRenderSession? Create(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection, Action<string>? log)
    {
        if (deviceOwner is null || swapchainOwner is null || selection is null || !selection.Success || selection.Queue is null)
        { log?.Invoke(VulkanClearFrameLogFormatter.Skipped("设备/Swapchain/选择不可用")); return null; }
        try
        {
            var clear = new VulkanClearFrameOwner(vk, deviceOwner, swapchainOwner, selection.Queue!.GraphicsFamily, log);
            var loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear, log);
            loop.Start();
            return new VulkanRenderSession(deviceOwner, swapchainOwner, clear, loop, log);
        }
        catch (Exception ex) { log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"创建异常：{ex.Message}")); return null; }
    }

    public void Resize(int width, int height)
    {
        if (_disposed) return;
        _presentLoop.Stop();
        _swapchainOwner.Recreate(width, height);
        _clearFrame.RebuildFramebuffers();
        _log?.Invoke(VulkanClearFrameLogFormatter.Rebuilt(_swapchainOwner.Extent, (uint)_clearFrame.CommandBuffers.Length));
        _presentLoop.Start();
    }

    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        _presentLoop.Stop();
        _presentLoop.Dispose();
        _clearFrame.Dispose();
        _log?.Invoke(VulkanClearFrameLogFormatter.Disposed());
    }
}

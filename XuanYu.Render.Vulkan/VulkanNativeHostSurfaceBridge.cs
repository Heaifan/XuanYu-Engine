using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Bridge;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan;

// VK-LIFE-1：Attach 全成功后才写入字段；失败按现有释放顺序回滚。
public sealed partial class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    readonly Action<string>? _log;
    readonly IRenderProjectionSource? _projectionSource;
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    VulkanDeviceOwner? _deviceOwner;
    VulkanSwapchainOwner? _swapchainOwner;
    VulkanRenderSession? _renderSession;
    RendererEventSession? _rendererEvents;
    bool _disposed;
    bool _failed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;
    public VulkanNativeHostSurfaceBridge(Action<string>? log = null, IRenderProjectionSource? projectionSource = null)
    {
        _log = log;
        _projectionSource = projectionSource;
        if (_projectionSource is not null) _projectionSource.RenderProjectionChanged += OnRenderProjectionChanged;
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null && _rendererEvents is null)
        { Emit(VulkanBridgeLogFormatter.DetachedSkipped()); return; }
        if (_renderSession is not null && !_renderSession.TryDispose())
        {
            _failed = true;
            Emit(VulkanBridgeLogFormatter.DetachBlocked());
            return;
        }
        _renderSession = null;
        _swapchainOwner?.Dispose(); _swapchainOwner = null;
        _deviceOwner?.Dispose(); _deviceOwner = null;
        _surfaceOwner?.Dispose(); _surfaceOwner = null;
        Emit(VulkanBridgeLogFormatter.SurfaceDisposed());
        _instanceOwner?.Dispose(); _instanceOwner = null;
        Emit(VulkanBridgeLogFormatter.InstanceDisposed());
        CloseDiagnostics(_rendererEvents);
        _rendererEvents = null;
        _failed = false;
        Emit(VulkanBridgeLogFormatter.Detached());
    }

}

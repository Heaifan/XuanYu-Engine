using System;
using Silk.NET.Vulkan;
using XuanYu.Core.Scene;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Render;

// VK-LIFE-1：创建失败必须显式失败，交给 Session 逆序回滚。
public sealed unsafe partial class VulkanClearFrameOwner : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly Action<string>? _log;
    RenderPass _renderPass;
    CommandPool _commandPool;
    CommandBuffer[] _commandBuffers = [];
    Framebuffer[] _framebuffers = [];
    ImageView[] _views = [];
    Silk.NET.Vulkan.Pipeline _pipeline = default;
    SceneRenderSnapshot _sceneSnapshot = SceneRenderSnapshot.TestEntityAtOrigin;
    Extent2D _extent;
    bool _disposed;

    public VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanSwapchainOwner swapchainOwner, int graphicsFamily, Action<string>? log)
    {
        _vk = vk;
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _log = log;
        try
        {
            BuildRenderPass();
            CreateCommandPool(graphicsFamily);
            if (!RebuildFramebuffers()) throw new InvalidOperationException("Framebuffer 创建失败");
            Log(VulkanClearFrameLogFormatter.Created());
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public CommandBuffer[] CommandBuffers => _commandBuffers;
    public Extent2D Extent => _extent;
    public RenderPass RenderPass => _renderPass;

    public void SetPipeline(Silk.NET.Vulkan.Pipeline pipeline)
    {
        _pipeline = pipeline;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }

    public void SetSceneSnapshot(SceneRenderSnapshot snapshot)
    {
        _sceneSnapshot = snapshot;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Scene snapshot 注入后 CommandBuffer 重录失败");
    }

    public bool RebuildFramebuffers(uint generation = 0, bool force = false)
    {
        if (!force && _framebuffers.Length > 0 && _extent.Width == _swapchainOwner.Extent.Width && _extent.Height == _swapchainOwner.Extent.Height)
        {
            Log(VulkanClearFrameLogFormatter.Skipped($"同尺寸跳过帧缓冲重建（{_extent.Width}x{_extent.Height}）"));
            return true;
        }
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Framebuffer.Rebuild", "开始..."));
        DestroyFramebuffers();
        _extent = _swapchainOwner.Extent;
        _views = _swapchainOwner.ImageViews.ToArray();
        _framebuffers = new Framebuffer[_views.Length];
        if (!CreateFramebuffers()) return false;
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "Framebuffer.Rebuild完成", $"{_extent.Width}x{_extent.Height}；{_framebuffers.Length}张FB+重录CB"));
        return RecordCommandBuffers(_views);
    }
}

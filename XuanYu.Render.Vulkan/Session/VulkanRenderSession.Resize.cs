using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    public bool Resize(int width, int height)
    {
        if (_disposed || IsFailed) return false;
        lock (_rebuildLock)
        {
            if (IsSameSize(width, height))
            {
                _presentLoop.PublishLifecycle(RendererEventCodes.ResizeSkipped,
                    RendererEventLevel.Info, _swapchainOwner.ResourceGeneration,
                    width, height, _swapchainOwner.Extent.Width, _swapchainOwner.Extent.Height,
                    "请求尺寸与当前 Swapchain 一致");
                _log?.Invoke(VulkanClearFrameLogFormatter.ResizeFastSkipped(_generation, width, height));
                return true;
            }
            _resizeStopping = true;
        }
        VulkanResizeTracer.StartTrace();
        _presentLoop.PublishLifecycle(RendererEventCodes.ResizeStarted, RendererEventLevel.Info,
            _swapchainOwner.ResourceGeneration, width, height,
            _swapchainOwner.Extent.Width, _swapchainOwner.Extent.Height);
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize 开始", $"请求物理尺寸={width}x{height}"));
        if (!_presentLoop.Stop()) return FailResize("Present 泵停止失败", width, height);
        lock (_rebuildLock)
        {
            _resizeStopping = false;
            if (IsSameSize(width, height)) return RestartAfterResizeSkip(width, height);
            var old = _swapchainOwner.Extent;
            var oldResourceGen = _swapchainOwner.ResourceGeneration;
            if (!_swapchainOwner.TryRecreateToCurrent(out _, out var currentRebuilt, _generation, _presentLoop.RetireGenerationForSwapchainReplacement)) return FailResize("Swapchain 重建失败", width, height);
            var rebuilt = oldResourceGen != _swapchainOwner.ResourceGeneration;
            var next = _swapchainOwner.Extent;
            if (!currentRebuilt && !rebuilt)
            {
                _log?.Invoke(VulkanClearFrameLogFormatter.ResizeSkipped("UI合并Resize", _generation, next, width, height, "Present自愈已完成目标尺寸"));
                _presentLoop.PublishLifecycle(RendererEventCodes.ResizeSkipped,
                    RendererEventLevel.Info, _swapchainOwner.ResourceGeneration,
                    width, height, next.Width, next.Height, "Present 自愈已完成目标尺寸");
                return RestartAfterResizeSkip(width, height);
            }
            if (rebuilt || old.Width != _swapchainOwner.Extent.Width || old.Height != _swapchainOwner.Extent.Height) _generation++;
            _log?.Invoke(VulkanClearFrameLogFormatter.SwapchainGeneration("UI合并Resize", oldResourceGen, _swapchainOwner.ResourceGeneration, rebuilt, old, next, "必须重建FB并重录CB"));
            if (!_clearFrame.RebuildFramebuffers(_generation, rebuilt)) return FailResize("Framebuffer 重建失败", width, height);
            _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize 完成", $"物理尺寸={_swapchainOwner.Extent.Width}x{_swapchainOwner.Extent.Height}；命令缓冲={_clearFrame.CommandBuffers.Length} 张"));
        }
        if (!_presentLoop.Start()) return FailResize("Present 泵重启失败", width, height);
        _presentLoop.PublishLifecycle(RendererEventCodes.ResizeCompleted, RendererEventLevel.Info,
            _swapchainOwner.ResourceGeneration, width, height,
            _swapchainOwner.Extent.Width, _swapchainOwner.Extent.Height);
        return true;
    }

    bool IsSameSize(int width, int height)
        => _swapchainOwner.Extent.Width == (uint)width && _swapchainOwner.Extent.Height == (uint)height;

    public void UpdateRenderProjection(RenderProjectionResult projection)
    {
        if (_disposed || IsFailed) return;
        _clearFrame.QueueRenderProjection(projection);
    }
}

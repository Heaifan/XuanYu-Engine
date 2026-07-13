using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    public bool Resize(int width, int height)
    {
        if (_disposed || _failed) return false;
        lock (_rebuildLock)
        {
            if (IsSameSize(width, height))
            {
                _log?.Invoke(VulkanClearFrameLogFormatter.ResizeFastSkipped(_generation, width, height));
                return true;
            }
            _resizeStopping = true;
        }
        VulkanResizeTracer.StartTrace();
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize开始", $"请求逻辑尺寸={width}x{height}"));
        if (!_presentLoop.Stop()) return FailResize("Present 泵停止失败");
        lock (_rebuildLock)
        {
            _resizeStopping = false;
            if (IsSameSize(width, height)) return RestartAfterResizeSkip(width, height);
            var old = _swapchainOwner.Extent;
            if (!_swapchainOwner.Recreate(width, height, _generation)) return FailResize("Swapchain 重建失败");
            if (!_clearFrame.RebuildFramebuffers(_generation)) return FailResize("Framebuffer 重建失败");
            if (old.Width != _swapchainOwner.Extent.Width || old.Height != _swapchainOwner.Extent.Height) _generation++;
            _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize完成", $"{_swapchainOwner.Extent.Width}x{_swapchainOwner.Extent.Height}；{_clearFrame.CommandBuffers.Length} 张 CB"));
        }
        if (!_presentLoop.Start()) return FailResize("Present 泵重启失败");
        return true;
    }

    bool IsSameSize(int width, int height)
        => _swapchainOwner.Extent.Width == (uint)width && _swapchainOwner.Extent.Height == (uint)height;
}

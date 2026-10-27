using XuanYu.Render.Vulkan.Diagnostic;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    public bool RebuildFramebuffers(uint generation = 0, bool force = false)
    {
        var nextExtent = _offscreenTarget?.Extent ?? _swapchainOwner!.Extent;
        if (!force && _framebuffers.Length > 0 &&
            _extent.Width == nextExtent.Width && _extent.Height == nextExtent.Height)
        {
            Log(VulkanClearFrameLogFormatter.Skipped($"同尺寸跳过帧缓冲重建（{_extent.Width}x{_extent.Height}）"));
            return true;
        }

        _log?.Invoke(VulkanResizeTracer.Stage(generation, "帧缓冲重建开始", "开始"));
        DestroyFramebuffers();
        _extent = nextExtent;
        _views = _offscreenTarget is null ? _swapchainOwner!.ImageViews.ToArray() : [_offscreenTarget.View];
        try
        {
            RebuildReadbackBuffers();
            if (!CreateFramebuffers()) return FailClosedFrameTargets();
            _framebufferGeneration++;
            if (!RecordCommandBuffers(_views)) return FailClosedFrameTargets();
            _log?.Invoke(VulkanResizeTracer.Stage(generation, "帧缓冲重建完成", $"物理尺寸={_extent.Width}x{_extent.Height}；帧缓冲={_framebuffers.Length} 张；命令缓冲已重录"));
            return true;
        }
        catch
        {
            FailClosedFrameTargets();
            return false;
        }
    }
    void Initialize(int graphicsFamily)
    {
        try
        {
            BuildRenderPass();
            CreateCommandPool(graphicsFamily);
            InitializeMapLabelTextures();
            if (!RebuildFramebuffers()) throw new InvalidOperationException("Framebuffer 创建失败");
            Log(VulkanClearFrameLogFormatter.Created());
        }
        catch
        {
            Dispose();
            throw;
        }
    }
}

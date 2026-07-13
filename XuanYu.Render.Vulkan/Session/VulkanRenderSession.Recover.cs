using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    bool RecoverFromOutOfDate(string source)
    {
        if (_disposed || IsFailed) return false;
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Present.OutOfDate", $"来源={source}（进入自愈）"));
        lock (_rebuildLock)
        {
            if (_resizeStopping) return false;
            var old = _swapchainOwner.Extent;
            var oldResourceGen = _swapchainOwner.ResourceGeneration;
            _log?.Invoke(VulkanResizeTracer.HealStage(_generation, source, $"{old.Width}x{old.Height}", "查询中..."));
            if (!_swapchainOwner.TryRecreateToCurrent(out _, out var rebuilt, _generation)) return RetryOrStop(source, old);
            if (!rebuilt) return true;
            _generation++;
            if (!_clearFrame.RebuildFramebuffers(_generation, true)) return false;
            _recoverTries = 0;
            var next = _swapchainOwner.Extent;
            _log?.Invoke(VulkanClearFrameLogFormatter.SwapchainGeneration($"QueuePresent自愈/{source}", oldResourceGen, _swapchainOwner.ResourceGeneration, true, old, next, "必须重建FB并重录CB"));
            _log?.Invoke(VulkanResizeTracer.HealStage(_generation, source, $"{old.Width}x{old.Height}", $"{next.Width}x{next.Height}", "已恢复 Present"));
            return true;
        }
    }

    bool RetryOrStop(string source, Extent2D old)
    {
        var next = _swapchainOwner.Extent;
        var dpi = _surfaceHandle?.DpiScale ?? 1.0;
        _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateProbe(source, old, next, _generation, dpi));
        if (_recoverTries >= MaxRecoverTries)
        {
            _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateRecoverFailed($"连续 {MaxRecoverTries} 次重建失败"));
            MarkFailed("Swapchain 自愈连续失败");
            return false;
        }
        _recoverTries++;
        return true;
    }
}

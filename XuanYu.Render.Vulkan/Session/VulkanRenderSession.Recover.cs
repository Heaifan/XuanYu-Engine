using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    bool RecoverFromOutOfDate(string source)
    {
        if (_disposed) return false;
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Present.OutOfDate", $"来源={source}（进入自愈）"));
        lock (_rebuildLock)
        {
            var old = _swapchainOwner.Extent;
            _log?.Invoke(VulkanResizeTracer.HealStage(_generation, source, $"{old.Width}x{old.Height}", "查询中..."));
            if (!_swapchainOwner.TryRecreateToCurrent(out _, _generation)) return RetryOrStop(source, old);
            if (!_clearFrame.RebuildFramebuffers(_generation)) return false;
            _generation++;
            _recoverTries = 0;
            var next = _swapchainOwner.Extent;
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
            return false;
        }
        _recoverTries++;
        return true;
    }
}

using XuanYu.Render.Vulkan.Render;
using System.Threading;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    bool FailResize(string reason)
    {
        _log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"Resize 失败，RenderSession 将释放：{reason}"));
        _resizeStopping = false;
        MarkFailed($"Resize 失败：{reason}");
        TryDispose();
        return false;
    }

    bool RestartAfterResizeSkip(int width, int height)
    {
        _resizeStopping = false;
        _log?.Invoke(VulkanClearFrameLogFormatter.ResizeFastSkipped(_generation, width, height));
        return _presentLoop.Start() || FailResize("Present 泵重启失败");
    }

    void MarkFailed(string reason)
    {
        if (Interlocked.CompareExchange(ref _failureReason, reason, null) is not null) return;
        Volatile.Write(ref _failed, 1);
        _log?.Invoke(VulkanClearFrameLogFormatter.PresentFatal(reason));
    }

    public bool TryDispose()
    {
        if (_disposed) return true;
        if (!_presentLoop.Stop()) return false;
        _disposed = true;
        _presentLoop.Dispose();
        _pipeline?.Dispose();
        _clearFrame.Dispose();
        _log?.Invoke(VulkanClearFrameLogFormatter.SessionDisposed());
        return true;
    }

    public void Dispose() => TryDispose();
}

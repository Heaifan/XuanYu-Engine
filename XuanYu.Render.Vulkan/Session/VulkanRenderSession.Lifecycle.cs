using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    bool FailResize(string reason)
    {
        _log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"Resize 失败，RenderSession 将释放：{reason}"));
        TryDispose();
        return false;
    }

    public bool TryDispose()
    {
        if (_disposed) return true;
        if (!_presentLoop.Stop()) return false;
        _disposed = true;
        _presentLoop.Dispose();
        _pipeline?.Dispose();
        _clearFrame.Dispose();
        _log?.Invoke(VulkanClearFrameLogFormatter.Disposed());
        return true;
    }

    public void Dispose() => TryDispose();
}

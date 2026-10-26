using XuanYu.Render.Vulkan.Bridge;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    public void Dispose()
    {
        if (_disposed) return;
        if (_projectionSource is not null) _projectionSource.RenderProjectionChanged -= OnRenderProjectionChanged;
        Detach();
        if (_renderSession is not null) return;
        _vk?.Dispose(); _vk = null;
        _disposed = true;
    }

    void Emit(string message) => VulkanBridgeLogFormatter.Emit(_log, message);
}

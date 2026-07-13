using XuanYu.Render.Vulkan.Bridge;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Detach();
        if (_renderSession is null) { _vk?.Dispose(); _vk = null; }
    }

    void Emit(string message) => VulkanBridgeLogFormatter.Emit(_log, message);
}

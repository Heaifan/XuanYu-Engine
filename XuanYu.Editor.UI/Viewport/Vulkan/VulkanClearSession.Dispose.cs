using Silk.NET.Vulkan;

namespace XuanYu.Editor.UI;

public sealed unsafe partial class VulkanClearSession
{
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_device.Handle != 0) _vk.DeviceWaitIdle(_device);
        DestroySwapchain();
        if (_device.Handle != 0) _vk.DestroyDevice(_device, null);
        if (_surface.Handle != 0) _khrSurface?.DestroySurface(_instance, _surface, null);
        if (_instance.Handle != 0) _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}

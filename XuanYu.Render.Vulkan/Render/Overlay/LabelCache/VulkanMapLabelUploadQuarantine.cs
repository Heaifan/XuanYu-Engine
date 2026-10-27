using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe class VulkanMapLabelUploadQuarantine : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly CommandPool _pool;
    readonly CommandBuffer _command; readonly VulkanStaticModelBuffer _staging;
    readonly ImageView _view; readonly Image _image; readonly DeviceMemory _memory;
    bool _disposed;
    internal CommandBuffer Command => _command;

    internal VulkanMapLabelUploadQuarantine(Vk vk, VulkanDeviceOwner device, CommandPool pool,
        CommandBuffer command, VulkanStaticModelBuffer staging, ImageView view, Image image, DeviceMemory memory)
    {
        _vk = vk; _device = device; _pool = pool; _command = command;
        _staging = staging; _view = view; _image = image; _memory = memory;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _vk.FreeCommandBuffers(_device.LogicalDevice, _pool, 1, in _command);
        _staging.Dispose();
        if (_view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, _view, null);
        if (_image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, _image, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
    }
}

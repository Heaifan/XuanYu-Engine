using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe class VulkanMapLabelTexture : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<DescriptorSet> _releaseDescriptor;
    Image _image; DeviceMemory _memory; ImageView _view; DescriptorSet _descriptor;

    internal VulkanMapLabelTexture(Vk vk, VulkanDeviceOwner device, Image image,
        DeviceMemory memory, ImageView view, DescriptorSet descriptor, Action<DescriptorSet> releaseDescriptor)
    {
        _vk = vk; _device = device; _image = image; _memory = memory;
        _view = view; _descriptor = descriptor; _releaseDescriptor = releaseDescriptor;
    }

    public DescriptorSet DescriptorSet => _descriptor;

    public void Dispose()
    {
        if (_descriptor.Handle != 0) _releaseDescriptor(_descriptor);
        _descriptor = default;
        if (_view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, _view, null);
        _view = default;
        if (_image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, _image, null);
        _image = default;
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
        _memory = default;
    }
}

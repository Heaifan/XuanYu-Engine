using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe class VulkanMapLabelTexture(Vk vk, VulkanDeviceOwner device,
    Image image, DeviceMemory memory, ImageView view, DescriptorSet descriptor) : IDisposable
{
    readonly Vk _vk = vk;
    readonly VulkanDeviceOwner _device = device;
    public DescriptorSet DescriptorSet { get; } = descriptor;

    public void Dispose()
    {
        if (view.Handle != 0) _vk.DestroyImageView(_device.LogicalDevice, view, null);
        if (image.Handle != 0) _vk.DestroyImage(_device.LogicalDevice, image, null);
        if (memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, memory, null);
    }
}

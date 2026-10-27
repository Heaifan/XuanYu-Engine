using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed class VulkanMapLabelUploadQuarantine
{
    internal CommandBuffer Command { get; }
    internal VulkanLabelUploadResolution Resolution { get; }
    internal VulkanStaticModelBuffer Staging { get; }
    internal ImageView View { get; }
    internal Image Image { get; }
    internal DeviceMemory Memory { get; }

    internal VulkanMapLabelUploadQuarantine(CommandBuffer command,
        VulkanLabelUploadResolution resolution, VulkanStaticModelBuffer staging,
        ImageView view, Image image, DeviceMemory memory)
    {
        Command = command; Resolution = resolution; Staging = staging;
        View = view; Image = image; Memory = memory;
    }
}

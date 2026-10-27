using Silk.NET.Vulkan;
using VkDevice = global::Silk.NET.Vulkan.Device;
using VkBuffer = Silk.NET.Vulkan.Buffer;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

internal static class VulkanStaticModelBufferMapFailureCalls
{
    internal static void DestroyBuffer(Vk vk, VkDevice device, VkBuffer buffer)
    { unsafe { vk.DestroyBuffer(device, buffer, null); } }

    internal static void FreeMemory(Vk vk, VkDevice device, DeviceMemory memory)
    { unsafe { vk.FreeMemory(device, memory, null); } }
}

using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

internal static unsafe class VulkanInstanceExtensionAvailability
{
    public static bool Supports(Vk vk, string requested)
    {
        uint count = 0;
        if (vk.EnumerateInstanceExtensionProperties((byte*)null, &count, null) != Result.Success) return false;
        var properties = stackalloc ExtensionProperties[(int)count];
        if (vk.EnumerateInstanceExtensionProperties((byte*)null, &count, properties) != Result.Success) return false;
        for (var i = 0; i < count; i++)
        {
            var name = properties[i].ExtensionName;
            if (Marshal.PtrToStringAnsi((nint)name) == requested) return true;
        }
        return false;
    }
}

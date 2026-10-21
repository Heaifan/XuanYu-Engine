using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Device;

public static unsafe class VulkanPresentFenceCapability
{
    public const string DeviceExtension = "VK_EXT_swapchain_maintenance1";
    public static bool IsSupported(Vk vk, Instance instance, PhysicalDevice physical,
        bool properties2Enabled, bool surfaceMaintenance1Enabled, Action<string>? log)
    {
        if (!properties2Enabled || !surfaceMaintenance1Enabled ||
            !HasDeviceExtension(vk, physical, DeviceExtension))
        { log?.Invoke("Present Fence unsupported: required properties2/surface maintenance1/maintenance1 extension missing"); return false; }
        if (!vk.TryGetInstanceExtension(instance, out KhrGetPhysicalDeviceProperties2 properties2))
        { log?.Invoke("Present Fence unsupported: Silk.NET properties2 entry point missing"); return false; }
        var feature = new PhysicalDeviceSwapchainMaintenance1FeaturesEXT
        { SType = StructureType.PhysicalDeviceSwapchainMaintenance1FeaturesExt };
        var features = new PhysicalDeviceFeatures2 { SType = StructureType.PhysicalDeviceFeatures2, PNext = &feature };
        properties2.GetPhysicalDeviceFeatures2(physical, &features);
        var supported = (bool)feature.SwapchainMaintenance1;
        log?.Invoke($"Present Fence hardware feature swapchainMaintenance1={supported}");
        return supported;
    }

    static bool HasDeviceExtension(Vk vk, PhysicalDevice physical, string requested)
    {
        uint count = 0;
        if (vk.EnumerateDeviceExtensionProperties(physical, (byte*)null, &count, null) != Result.Success) return false;
        var properties = stackalloc ExtensionProperties[(int)count];
        if (vk.EnumerateDeviceExtensionProperties(physical, (byte*)null, &count, properties) != Result.Success) return false;
        for (var i = 0; i < count; i++)
        {
            var name = properties[i].ExtensionName;
            if (Marshal.PtrToStringAnsi((nint)name) == requested) return true;
        }
        return false;
    }
}

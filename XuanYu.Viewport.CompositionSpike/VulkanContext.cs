using System.Runtime.InteropServices;
using Avalonia.Rendering.Composition;
using Avalonia.Platform;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Viewport.CompositionSpike;

unsafe sealed class VulkanContext : IDisposable
{
    public Vk Api { get; }
    public Instance Instance { get; }
    public PhysicalDevice PhysicalDevice { get; }
    public Device Device { get; }
    public Queue Queue { get; }
    public uint QueueFamily { get; }
    public VulkanCommandPool Commands { get; }
    public string DeviceName { get; }
    public string Driver { get; }

    VulkanContext(Vk api, Instance instance, PhysicalDevice physical, Device device,
        Queue queue, uint family, VulkanCommandPool commands, string name, string driver)
    { Api = api; Instance = instance; PhysicalDevice = physical; Device = device; Queue = queue;
        QueueFamily = family; Commands = commands; DeviceName = name; Driver = driver; }

    public static (VulkanContext? Context, string Info) Create(ICompositionGpuInterop interop)
    {
        if (!interop.SupportedImageHandleTypes.Contains(KnownPlatformGraphicsExternalImageHandleTypes.VulkanOpaqueNtHandle))
            return (null, "Avalonia 不支持 VulkanOpaqueNtHandle");
        var api = Vk.GetApi();
        using var app = new VulkanStrings(new[] { "A1.5" });
        var extensions = new VulkanStrings(new[] { "VK_KHR_get_physical_device_properties2", "VK_KHR_external_memory_capabilities", "VK_KHR_external_semaphore_capabilities" });
        var info = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = app.First, PEngineName = app.First, ApiVersion = new Version32(1, 1, 0) };
        var create = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &info, PpEnabledExtensionNames = extensions.Pointer, EnabledExtensionCount = extensions.Count };
        api.CreateInstance(in create, null, out var instance).Ensure();
        uint count = 0; api.EnumeratePhysicalDevices(instance, ref count, null).Ensure();
        var physicals = new PhysicalDevice[count]; fixed (PhysicalDevice* p = physicals) api.EnumeratePhysicalDevices(instance, ref count, p).Ensure();
        foreach (var physical in physicals)
        {
            uint families = 0; api.GetPhysicalDeviceQueueFamilyProperties(physical, ref families, null);
            var props = new QueueFamilyProperties[families]; fixed (QueueFamilyProperties* p = props) api.GetPhysicalDeviceQueueFamilyProperties(physical, ref families, p);
            for (uint family = 0; family < families; family++)
            {
                if (!props[family].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) continue;
                var required = new[] { "VK_KHR_external_memory", "VK_KHR_external_semaphore", "VK_KHR_external_memory_win32", "VK_KHR_external_semaphore_win32", "VK_KHR_dedicated_allocation", "VK_KHR_get_memory_requirements2" };
                if (required.Any(x => !api.IsDeviceExtensionPresent(physical, x))) continue;
                var priority = 1f; var queue = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = family, QueueCount = 1, PQueuePriorities = &priority };
                using var deviceExt = new VulkanStrings(required);
                var deviceInfo = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &queue, PpEnabledExtensionNames = deviceExt.Pointer, EnabledExtensionCount = deviceExt.Count };
                api.CreateDevice(physical, in deviceInfo, null, out var device).Ensure(); api.GetDeviceQueue(device, family, 0, out var mainQueue);
                var props2 = new PhysicalDeviceProperties(); api.GetPhysicalDeviceProperties(physical, out props2);
                var name = Marshal.PtrToStringAnsi((nint)props2.DeviceName) ?? "unknown";
                var driver = $"{props2.DriverVersion} / Vulkan {props2.ApiVersion}";
                return (new VulkanContext(api, instance, physical, device, mainQueue, family, new VulkanCommandPool(api, device, mainQueue, family), name, driver), $"GPU={name}; Driver={driver}");
            }
        }
        api.DestroyInstance(instance, null); return (null, "未找到支持外部内存的 Vulkan graphics queue");
    }

    public void Dispose() { Api.DeviceWaitIdle(Device); Commands.Dispose(); Api.DestroyDevice(Device, null); Api.DestroyInstance(Instance, null); }
}

using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Device;

/// <summary>
/// 创建 Vulkan Instance，选择支持 Graphics Queue 的设备，创建并释放 LogicalDevice。
/// </summary>
public static unsafe class VulkanDeviceProbe
{
    public static VulkanDeviceInfo Probe()
    {
        var stopwatch = Stopwatch.StartNew();
        var instance = default(Silk.NET.Vulkan.Instance);
        var device = default(Silk.NET.Vulkan.Device);
        var instanceCreated = false;
        var deviceCreated = false;
        Vk? vk = null;

        try
        {
            vk = Vk.GetApi();
            var createInstanceResult = CreateInstance(vk, out instance);
            if (createInstanceResult != Result.Success)
            {
                stopwatch.Stop();
                return Failed($"Vulkan Device 创建失败：Instance 创建失败：{createInstanceResult}。", stopwatch);
            }

            instanceCreated = true;

            var physicalDevicesResult = ReadPhysicalDevices(vk, instance);
            if (physicalDevicesResult.Result != Result.Success)
            {
                stopwatch.Stop();
                return Failed($"Vulkan Device 创建失败：枚举 PhysicalDevice 失败：{physicalDevicesResult.Result}。", stopwatch);
            }

            if (physicalDevicesResult.PhysicalDevices.Count == 0)
            {
                stopwatch.Stop();
                return Failed("Vulkan Device 创建失败：未找到物理设备。", stopwatch);
            }

            var selectedDevice = SelectPhysicalDevice(vk, physicalDevicesResult.PhysicalDevices);
            if (selectedDevice is null)
            {
                stopwatch.Stop();
                return Failed("Vulkan Device 创建失败：未找到支持 Graphics Queue 的物理设备。", stopwatch);
            }

            var queuePriority = 1.0f;
            var queueCreateInfo = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = (uint)selectedDevice.GraphicsQueueFamilyIndex,
                QueueCount = 1,
                PQueuePriorities = &queuePriority
            };

            var deviceCreateInfo = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &queueCreateInfo,
                EnabledLayerCount = 0,
                PpEnabledLayerNames = null,
                EnabledExtensionCount = 0,
                PpEnabledExtensionNames = null,
                PEnabledFeatures = null
            };

            var createDeviceResult = vk.CreateDevice(
                selectedDevice.PhysicalDevice,
                &deviceCreateInfo,
                null,
                out device);

            if (createDeviceResult != Result.Success)
            {
                stopwatch.Stop();
                return new VulkanDeviceInfo(
                    VulkanDeviceStatus.Failed,
                    $"Vulkan Device 创建失败：{createDeviceResult}。",
                    selectedDevice.Name,
                    selectedDevice.DeviceTypeText,
                    selectedDevice.GraphicsQueueFamilyIndex,
                    stopwatch.Elapsed.TotalMilliseconds);
            }

            deviceCreated = true;
            vk.GetDeviceQueue(device, (uint)selectedDevice.GraphicsQueueFamilyIndex, 0, out _);

            stopwatch.Stop();
            return new VulkanDeviceInfo(
                VulkanDeviceStatus.Created,
                $"创建成功，显卡：{selectedDevice.Name}，类型：{selectedDevice.DeviceTypeText}，图形队列族：{selectedDevice.GraphicsQueueFamilyIndex}。",
                selectedDevice.Name,
                selectedDevice.DeviceTypeText,
                selectedDevice.GraphicsQueueFamilyIndex,
                stopwatch.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            stopwatch.Stop();
            return Failed($"Vulkan Device 创建失败：{ex.Message}", stopwatch);
        }
        finally
        {
            if (vk is not null && deviceCreated)
            {
                vk.DestroyDevice(device, null);
            }

            if (vk is not null && instanceCreated)
            {
                vk.DestroyInstance(instance, null);
            }
        }
    }

    private static Result CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance instance)
    {
        instance = default;

        var apiVersion = PackApiVersion(1, 0, 0);
        if (vk.EnumerateInstanceVersion(ref apiVersion) != Result.Success)
        {
            apiVersion = PackApiVersion(1, 0, 0);
        }

        var appName = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var engineName = Marshal.StringToHGlobalAnsi("FluidWarfare");

        try
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)appName,
                ApplicationVersion = PackApiVersion(0, 0, 1),
                PEngineName = (byte*)engineName,
                EngineVersion = PackApiVersion(0, 0, 1),
                ApiVersion = apiVersion
            };

            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledLayerCount = 0,
                PpEnabledLayerNames = null,
                EnabledExtensionCount = 0,
                PpEnabledExtensionNames = null
            };

            return vk.CreateInstance(&createInfo, null, out instance);
        }
        finally
        {
            Marshal.FreeHGlobal(appName);
            Marshal.FreeHGlobal(engineName);
        }
    }

    private static PhysicalDevicesReadResult ReadPhysicalDevices(Vk vk, Silk.NET.Vulkan.Instance instance)
    {
        uint physicalDeviceCount = 0;
        var countResult = vk.EnumeratePhysicalDevices(instance, ref physicalDeviceCount, null);
        if (countResult != Result.Success)
        {
            return new PhysicalDevicesReadResult(countResult, []);
        }

        if (physicalDeviceCount == 0)
        {
            return new PhysicalDevicesReadResult(Result.Success, []);
        }

        var physicalDevices = new Silk.NET.Vulkan.PhysicalDevice[physicalDeviceCount];
        fixed (Silk.NET.Vulkan.PhysicalDevice* physicalDevicesPtr = physicalDevices)
        {
            var listResult = vk.EnumeratePhysicalDevices(instance, ref physicalDeviceCount, physicalDevicesPtr);
            return new PhysicalDevicesReadResult(listResult, physicalDevices);
        }
    }

    private static SelectedPhysicalDevice? SelectPhysicalDevice(
        Vk vk,
        IReadOnlyList<Silk.NET.Vulkan.PhysicalDevice> physicalDevices)
    {
        var candidates = physicalDevices
            .Select(device => TryCreateCandidate(vk, device))
            .Where(candidate => candidate is not null)
            .Select(candidate => candidate!)
            .ToArray();

        return candidates
            .OrderByDescending(candidate => candidate.DeviceType == PhysicalDeviceType.DiscreteGpu)
            .FirstOrDefault();
    }

    private static SelectedPhysicalDevice? TryCreateCandidate(
        Vk vk,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice)
    {
        vk.GetPhysicalDeviceProperties(physicalDevice, out var properties);

        var queueFamilyIndex = FindGraphicsQueueFamilyIndex(vk, physicalDevice);
        if (queueFamilyIndex < 0)
        {
            return null;
        }

        return new SelectedPhysicalDevice(
            physicalDevice,
            ReadDeviceName(properties),
            properties.DeviceType,
            properties.DeviceType.ToString(),
            queueFamilyIndex);
    }

    private static int FindGraphicsQueueFamilyIndex(
        Vk vk,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice)
    {
        uint queueFamilyCount = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilyCount, null);

        if (queueFamilyCount == 0)
        {
            return -1;
        }

        var queueFamilies = new QueueFamilyProperties[queueFamilyCount];
        fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
        {
            vk.GetPhysicalDeviceQueueFamilyProperties(
                physicalDevice,
                ref queueFamilyCount,
                queueFamiliesPtr);
        }

        for (var i = 0; i < queueFamilies.Length; i++)
        {
            if (queueFamilies[i].QueueCount > 0 &&
                queueFamilies[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
            {
                return i;
            }
        }

        return -1;
    }

    private static string ReadDeviceName(PhysicalDeviceProperties properties)
    {
        return Marshal.PtrToStringAnsi((nint)properties.DeviceName) ?? "未知";
    }

    private static VulkanDeviceInfo Failed(string message, Stopwatch stopwatch)
    {
        return new VulkanDeviceInfo(
            VulkanDeviceStatus.Failed,
            message,
            "未知",
            "未知",
            -1,
            stopwatch.Elapsed.TotalMilliseconds);
    }

    private static uint PackApiVersion(uint major, uint minor, uint patch)
    {
        return (major << 22) | (minor << 12) | patch;
    }

    private sealed record PhysicalDevicesReadResult(
        Result Result,
        IReadOnlyList<Silk.NET.Vulkan.PhysicalDevice> PhysicalDevices);

    private sealed record SelectedPhysicalDevice(
        Silk.NET.Vulkan.PhysicalDevice PhysicalDevice,
        string Name,
        PhysicalDeviceType DeviceType,
        string DeviceTypeText,
        int GraphicsQueueFamilyIndex);
}

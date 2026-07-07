using System.Text;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

public static unsafe class VulkanApiProbe
{
    public static VulkanProbeResult Probe()
    {
        Vk vk;
        try { vk = Vk.GetApi(); }
        catch (Exception ex) { return Fail(0, ex); }

        uint instanceVersion = Vk.Version10;
        try
        {
            var version = 0u;
            vk.EnumerateInstanceVersion(&version);
            instanceVersion = version;
        }
        catch { }

        Instance instance = default;
        try
        {
            CreateInstance(vk, instanceVersion, out instance);
            var devices = EnumerateDevices(vk, instance);
            return devices.Count == 0 ? Fail(instanceVersion, new InvalidOperationException("未枚举到物理设备")) : new(true, instanceVersion, devices);
        }
        catch (Exception ex)
        {
            return Fail(instanceVersion, ex);
        }
        finally
        {
            if (instance.Handle != 0) vk.DestroyInstance(instance, null);
            vk.Dispose();
        }
    }

    static VulkanProbeResult Fail(uint instanceVersion, Exception ex) =>
        new(false, instanceVersion, [], ex.GetType().Name, ex.Message);

    static void CreateInstance(Vk vk, uint instanceVersion, out Instance instance)
    {
        var appBytes = Encoding.UTF8.GetBytes("XuanYu Vulkan Probe\0");
        fixed (byte* app = appBytes)
        {
            var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = app, PEngineName = app, ApiVersion = instanceVersion };
            var createInfo = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo };
            Check(vk.CreateInstance(&createInfo, null, out instance), "创建 Vulkan Instance 失败");
        }
    }

    static IReadOnlyList<VulkanDeviceInfo> EnumerateDevices(Vk vk, Instance instance)
    {
        uint count = 0;
        Check(vk.EnumeratePhysicalDevices(instance, &count, null), "枚举物理设备失败");
        if (count == 0) return [];
        var devices = stackalloc PhysicalDevice[(int)count];
        Check(vk.EnumeratePhysicalDevices(instance, &count, devices), "枚举物理设备失败");
        var list = new List<VulkanDeviceInfo>((int)count);
        for (var i = 0; i < count; i++)
        {
            vk.GetPhysicalDeviceProperties(devices[i], out var props);
            list.Add(new VulkanDeviceInfo(GetName(props), props.DeviceType, props.ApiVersion));
        }
        return list;
    }

    static string GetName(PhysicalDeviceProperties props)
    {
        byte* name = props.DeviceName;
        return Encoding.UTF8.GetString(name, 256).TrimEnd('\0');
    }

    static void Check(Result result, string message)
    {
        if (result != Result.Success) throw new InvalidOperationException($"{message}: {result}");
    }
}

using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Device;

/// <summary>临时 VkInstance 生命周期（无扩展，仅用于设备探测）。供 VulkanDeviceProbe 内部使用。</summary>
sealed unsafe class VulkanDeviceInstanceScope : IDisposable
{
    readonly Vk _vk;
    Silk.NET.Vulkan.Instance _instance;
    bool _instanceCreated;

    public VulkanDeviceInstanceScope() => _vk = Vk.GetApi();

    public Vk Vk => _vk;
    public Silk.NET.Vulkan.Instance Instance => _instance;

    public Result CreateInstance()
    {
        var apiVersion = PackApiVersion(1, 0, 0);
        if (_vk.EnumerateInstanceVersion(ref apiVersion) != Result.Success)
            apiVersion = PackApiVersion(1, 0, 0);

        var appName = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var engineName = Marshal.StringToHGlobalAnsi("XuanYu Engine");

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
            var ci = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo
            };
            var r = _vk.CreateInstance(&ci, null, out _instance);
            if (r == Result.Success) _instanceCreated = true;
            return r;
        }
        finally
        {
            Marshal.FreeHGlobal(appName);
            Marshal.FreeHGlobal(engineName);
        }
    }

    public void Dispose()
    {
        if (_instanceCreated) _vk.DestroyInstance(_instance, null);
    }

    static uint PackApiVersion(uint major, uint minor, uint patch) =>
        (major << 22) | (minor << 12) | patch;
}

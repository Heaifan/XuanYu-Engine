using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Instance;

/// <summary>创建并立即释放 Vulkan Instance，验证 Vulkan API 调用链路。</summary>
public static unsafe class VulkanInstanceProbe
{
    public static VulkanInstanceInfo Probe()
    {
        var sw = Stopwatch.StartNew();
        var apiVersionText = "未知"; var extensionCount = 0;
        var instance = default(Silk.NET.Vulkan.Instance); var instanceCreated = false;
        try
        {
            var vk = Vk.GetApi();
            var apiVersion = PackApiVersion(1, 0, 0);
            var apiVersionResult = vk.EnumerateInstanceVersion(ref apiVersion);
            if (apiVersionResult != Result.Success) apiVersion = PackApiVersion(1, 0, 0);
            apiVersionText = FormatApiVersion(apiVersion);
            extensionCount = ReadInstanceExtensionCount(vk);
            var appName = Marshal.StringToHGlobalAnsi("FluidWarfare");
            var engineName = Marshal.StringToHGlobalAnsi("FluidWarfare");
            try
            {
                var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)appName, ApplicationVersion = PackApiVersion(0, 0, 1), PEngineName = (byte*)engineName, EngineVersion = PackApiVersion(0, 0, 1), ApiVersion = apiVersion };
                var createInfo = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo, EnabledLayerCount = 0, PpEnabledLayerNames = null, EnabledExtensionCount = 0, PpEnabledExtensionNames = null };
                var createResult = vk.CreateInstance(&createInfo, null, out instance);
                if (createResult != Result.Success) return Fail(sw, apiVersionText, extensionCount, $"Vulkan Instance 创建失败：{createResult}。");
                instanceCreated = true; sw.Stop();
                return new VulkanInstanceInfo(VulkanInstanceStatus.Created, $"创建成功，API 版本：{apiVersionText}，扩展数量：{extensionCount}。", apiVersionText, extensionCount, sw.Elapsed.TotalMilliseconds);
            }
            finally { if (instanceCreated) vk.DestroyInstance(instance, null); Marshal.FreeHGlobal(appName); Marshal.FreeHGlobal(engineName); }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { return Fail(sw, apiVersionText, extensionCount, $"Vulkan Instance 创建失败：{ex.Message}"); }
    }

    static int ReadInstanceExtensionCount(Vk vk) { uint c = 0; return vk.EnumerateInstanceExtensionProperties((byte*)null, ref c, null) == Result.Success ? (int)c : 0; }
    static uint PackApiVersion(uint major, uint minor, uint patch) => (major << 22) | (minor << 12) | patch;
    static string FormatApiVersion(uint v) => $"{v >> 22}.{(v >> 12) & 0x3ff}.{v & 0xfff}";
    static VulkanInstanceInfo Fail(Stopwatch sw, string v, int c, string m) { sw.Stop(); return new VulkanInstanceInfo(VulkanInstanceStatus.Failed, m, v, c, sw.Elapsed.TotalMilliseconds); }
}

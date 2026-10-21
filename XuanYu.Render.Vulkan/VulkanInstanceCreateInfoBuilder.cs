using System.Text;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

// VK3-B1：Instance 创建信息构造辅助。仅构造 InstanceCreateInfo（含最小扩展集），不直接调用 Vulkan。
public static unsafe class VulkanInstanceCreateInfoBuilder
{
    public static readonly uint ApiVersion = Vk.Version10;

    // 在 fixed 作用域内构造 InstanceCreateInfo 并交给 create 回调，
    // 确保扩展名指针在创建调用期间保持有效。
    public static void BuildAndUse(Action<InstanceCreateInfo> create)
        => BuildAndUse(create, false, false, false);

    public static void BuildAndUse(Action<InstanceCreateInfo> create,
        bool enableProperties2, bool enableSurfaceCapabilities2, bool enableSurfaceMaintenance1)
    {
        var appName = Encoding.UTF8.GetBytes("XuanYu Engine\0");
        var surfaceExt = Encoding.UTF8.GetBytes(VulkanInstanceExtensions.Surface);
        var win32Ext = Encoding.UTF8.GetBytes(VulkanInstanceExtensions.Win32Surface);
        var properties2Ext = Encoding.ASCII.GetBytes(VulkanInstanceExtensions.Properties2 + "\0");
        var surfaceMaintenanceExt = Encoding.ASCII.GetBytes(VulkanInstanceExtensions.SurfaceMaintenance1 + "\0");
        var surfaceCapabilities2Ext = Encoding.ASCII.GetBytes(VulkanInstanceExtensions.SurfaceCapabilities2 + "\0");
        fixed (byte* pApp = appName, pSurface = surfaceExt, pWin32 = win32Ext,
            pProperties2 = properties2Ext, pSurfaceMaintenance = surfaceMaintenanceExt,
            pSurfaceCapabilities2 = surfaceCapabilities2Ext)
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = pApp,
                PEngineName = pApp,
                ApiVersion = ApiVersion
            };
            byte** extPtrs = stackalloc byte*[5];
            extPtrs[0] = pSurface;
            extPtrs[1] = pWin32;
            var count = 2u;
            if (enableSurfaceCapabilities2) extPtrs[count++] = pSurfaceCapabilities2;
            if (enableSurfaceMaintenance1) extPtrs[count++] = pSurfaceMaintenance;
            if (enableProperties2) extPtrs[count++] = pProperties2;
            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = count,
                PpEnabledExtensionNames = extPtrs
            };
            create(createInfo);
        }
    }

    public static void BuildOffscreenAndUse(Action<InstanceCreateInfo> create)
    {
        var appName = Encoding.UTF8.GetBytes("XuanYu Engine Offscreen\0");
        fixed (byte* pApp = appName)
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = pApp,
                PEngineName = pApp,
                ApiVersion = ApiVersion
            };
            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = 0,
                PpEnabledExtensionNames = null
            };
            create(createInfo);
        }
    }
}

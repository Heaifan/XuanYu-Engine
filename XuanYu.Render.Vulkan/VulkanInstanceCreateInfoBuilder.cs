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
    {
        var appName = Encoding.UTF8.GetBytes("XuanYu Engine\0");
        var surfaceExt = Encoding.UTF8.GetBytes(VulkanInstanceExtensions.Surface);
        var win32Ext = Encoding.UTF8.GetBytes(VulkanInstanceExtensions.Win32Surface);
        fixed (byte* pApp = appName, pSurface = surfaceExt, pWin32 = win32Ext)
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = pApp,
                PEngineName = pApp,
                ApiVersion = ApiVersion
            };
            byte** extPtrs = stackalloc byte*[2];
            extPtrs[0] = pSurface;
            extPtrs[1] = pWin32;
            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = 2,
                PpEnabledExtensionNames = extPtrs
            };
            create(createInfo);
        }
    }
}

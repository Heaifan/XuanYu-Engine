using System.Runtime.InteropServices;

namespace FluidWarfare.Render.Vulkan.Backend;

/// <summary>
/// Vulkan 后端探测器。
/// 当前在 Windows 下检测 vulkan-1.dll 是否可加载。
/// 不创建 Vulkan Instance/Device/Surface/Swapchain，不写日志，不依赖 Editor。
/// </summary>
public static class VulkanBackendProbe
{
    /// <summary>
    /// 执行最小 Vulkan 后端探测。
    /// </summary>
    public static VulkanBackendInfo Probe()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ProbeWindowsLoader();
        }

        return new VulkanBackendInfo(
            VulkanBackendStatus.Unavailable,
            "当前平台暂未实现 Vulkan Loader 探测。");
    }

    private static VulkanBackendInfo ProbeWindowsLoader()
    {
        if (NativeLibrary.TryLoad("vulkan-1.dll", out var handle))
        {
            NativeLibrary.Free(handle);
            return new VulkanBackendInfo(
                VulkanBackendStatus.Available,
                "已检测到 Windows Vulkan Loader：vulkan-1.dll。");
        }

        return new VulkanBackendInfo(
            VulkanBackendStatus.Unavailable,
            "未检测到 Windows Vulkan Loader：vulkan-1.dll。");
    }
}

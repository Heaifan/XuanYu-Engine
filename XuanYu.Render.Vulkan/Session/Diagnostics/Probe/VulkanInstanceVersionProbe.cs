using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

internal readonly record struct VulkanInstanceVersionResolution(
    bool Success, uint Version, bool UsedVersion10Fallback, string Error)
{
    internal static VulkanInstanceVersionResolution Resolve(bool entryPointAvailable,
        Result result, uint reportedVersion)
    {
        if (!entryPointAvailable)
            return new(true, Vk.Version10, true, "");
        if (result != Result.Success)
            return new(false, 0, false, $"vkEnumerateInstanceVersion: {result}");
        if (reportedVersion < Vk.Version10)
            return new(false, 0, false, "Vulkan loader returned an invalid API version");
        return new(true, reportedVersion, false, "");
    }
}

internal static unsafe class VulkanInstanceVersionProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result EnumerateVersion(uint* version);

    internal static VulkanInstanceVersionResolution Query(Vk vk)
    {
        try
        {
            var entryPoint = vk.GetInstanceProcAddr(default, "vkEnumerateInstanceVersion");
            if (entryPoint.Handle is null)
                return VulkanInstanceVersionResolution.Resolve(false, Result.Success, 0);
            var address = (IntPtr)entryPoint.Handle;
            if (address == IntPtr.Zero)
                return VulkanInstanceVersionResolution.Resolve(false, Result.Success, 0);
            var enumerate = Marshal.GetDelegateForFunctionPointer<EnumerateVersion>(address);
            var version = 0u;
            return VulkanInstanceVersionResolution.Resolve(true,
                enumerate(&version), version);
        }
        catch (Exception error)
        {
            return new(false, 0, false, $"版本探测异常：{error.GetType().Name}: {error.Message}");
        }
    }
}

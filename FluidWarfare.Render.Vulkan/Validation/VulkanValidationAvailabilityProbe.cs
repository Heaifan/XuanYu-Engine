using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>
/// 检测 VK_LAYER_KHRONOS_validation 和 VK_EXT_debug_utils 是否可用。
/// 不创建 Vulkan Instance。
/// </summary>
public static unsafe class VulkanValidationAvailabilityProbe
{
    /// <summary>
    /// 检测 Validation 组件是否可用。
    /// </summary>
    public static VulkanValidationInfo Probe()
    {
        var options = VulkanValidationOptions.FromEnvironment();
        if (!options.IsRequested)
            return VulkanValidationInfo.Disabled;

        Vk? vk = null;
        try
        {
            vk = Vk.GetApi();
        }
        catch
        {
            return new VulkanValidationInfo(
                VulkanValidationStatus.Failed,
                "Vulkan Validation：无法加载 Vulkan API。", 0);
        }

        // 检测 VK_LAYER_KHRONOS_validation
        uint layerCount = 0;
        if (vk.EnumerateInstanceLayerProperties(ref layerCount, null) != Result.Success)
        {
            return new VulkanValidationInfo(
                VulkanValidationStatus.Failed,
                "Vulkan Validation：无法枚举 Instance Layer。", 0);
        }

        var layers = new LayerProperties[layerCount];
        fixed (LayerProperties* p = layers)
        {
            if (vk.EnumerateInstanceLayerProperties(ref layerCount, p) != Result.Success)
            {
                return new VulkanValidationInfo(
                    VulkanValidationStatus.Failed,
                    "Vulkan Validation：枚举 Instance Layer 失败。", 0);
            }
        }

        var hasLayer = false;
        foreach (var layer in layers)
        {
            var name = Marshal.PtrToStringAnsi((nint)layer.LayerName);
            if (name == "VK_LAYER_KHRONOS_validation")
            {
                hasLayer = true;
                break;
            }
        }

        if (!hasLayer)
        {
            return new VulkanValidationInfo(
                VulkanValidationStatus.LayerMissing,
                "Vulkan Validation：请求启用，但未检测到 VK_LAYER_KHRONOS_validation。" +
                "请确认 Vulkan SDK 已安装或 Validation Layer 在系统路径中。",
                0);
        }

        // 检测 VK_EXT_debug_utils
        uint extCount = 0;
        if (vk.EnumerateInstanceExtensionProperties((byte*)null, ref extCount, null) != Result.Success)
        {
            return new VulkanValidationInfo(
                VulkanValidationStatus.Failed,
                "Vulkan Validation：无法枚举 Instance Extension。", 0);
        }

        var extensions = new ExtensionProperties[extCount];
        fixed (ExtensionProperties* p = extensions)
        {
            if (vk.EnumerateInstanceExtensionProperties((byte*)null, ref extCount, p) != Result.Success)
            {
                return new VulkanValidationInfo(
                    VulkanValidationStatus.Failed,
                    "Vulkan Validation：枚举 Instance Extension 失败。", 0);
            }
        }

        var hasDebugUtils = false;
        foreach (var ext in extensions)
        {
            var name = Marshal.PtrToStringAnsi((nint)ext.ExtensionName);
            if (name == "VK_EXT_debug_utils")
            {
                hasDebugUtils = true;
                break;
            }
        }

        if (!hasDebugUtils)
        {
            return new VulkanValidationInfo(
                VulkanValidationStatus.ExtensionMissing,
                "Vulkan Validation：请求启用，但未检测到 VK_EXT_debug_utils。" +
                "当前 Vulkan 驱动或 SDK 不支持调试扩展。",
                0);
        }

        return new VulkanValidationInfo(
            VulkanValidationStatus.Enabled,
            "Vulkan Validation：组件已就绪（VK_LAYER_KHRONOS_validation + VK_EXT_debug_utils）。",
            0);
    }
}

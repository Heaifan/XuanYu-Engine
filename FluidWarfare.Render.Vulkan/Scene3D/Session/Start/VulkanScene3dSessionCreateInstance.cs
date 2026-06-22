using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Validation;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Instance 创建（含 Debug Messenger 初始化）。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private bool CreateInstance()
    {
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var eg = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var surf = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var win = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        var debug = Marshal.StringToHGlobalAnsi("VK_EXT_debug_utils");
        var layer = Marshal.StringToHGlobalAnsi("VK_LAYER_KHRONOS_validation");
        try
        {
            var enableValidation = _validationOptions.IsRequested;
            var extCount = enableValidation ? 3u : 2u;
            var extPtrs = stackalloc byte*[3];
            extPtrs[0] = (byte*)surf;
            extPtrs[1] = (byte*)win;
            if (enableValidation) extPtrs[2] = (byte*)debug;

            var ai = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)a, ApplicationVersion = 1,
                PEngineName = (byte*)eg, EngineVersion = 1,
                ApiVersion = (1 << 22) | (0 << 12) | 0
            };
            var ci = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &ai,
                EnabledExtensionCount = extCount,
                PpEnabledExtensionNames = extPtrs
            };

            if (enableValidation)
            {
                var layerPtrs = stackalloc byte*[1];
                layerPtrs[0] = (byte*)layer;
                ci.EnabledLayerCount = 1;
                ci.PpEnabledLayerNames = layerPtrs;
            }

            var result = _vk!.CreateInstance(&ci, null, out _instance);
            if (result != Result.Success) return false;
            _instOk = true;
            _instanceCreateCount++;

            // Create Debug Messenger after Instance
            if (enableValidation)
            {
                try
                {
                    _debugMessengerScope = new VulkanDebugMessengerScope(_vk!, _instance, _validationMessageStore);
                }
                catch
                {
                    // Debug messenger creation failure is non-fatal
                }
            }

            return true;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(eg); Marshal.FreeHGlobal(surf); Marshal.FreeHGlobal(win); Marshal.FreeHGlobal(debug); Marshal.FreeHGlobal(layer); }
    }
}

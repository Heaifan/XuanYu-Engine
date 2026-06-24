using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Surface 创建 + 函数指针加载。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private bool CreateSurface(nint hinstance, nint hwnd)
    {
        var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceFn>(addr);
            var ci = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hinstance = hinstance, Hwnd = hwnd
            };
            fixed (SurfaceKHR* sp = &_surface)
            {
                if (fn(_instance, &ci, null, sp) == Result.Success)
                {
                    _surfOk = true;
                    return true;
                }
            }
            return false;
        }
        finally { Marshal.FreeHGlobal(p); }
    }

    private void LoadSessionFunctionPointers()
    {
        _fnDestroySurface = LoadProc("vkDestroySurfaceKHR");
        _fnGetCaps = LoadProc("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        _fnGetFormats = LoadProc("vkGetPhysicalDeviceSurfaceFormatsKHR");
        _fnGetModes = LoadProc("vkGetPhysicalDeviceSurfacePresentModesKHR");
        _fnDestroySwapchain = LoadDeviceProc("vkDestroySwapchainKHR");
        _fnCreateSwapchain = LoadDeviceProc("vkCreateSwapchainKHR");
        _fnGetSwapchainImages = LoadDeviceProc("vkGetSwapchainImagesKHR");
        _fnAcquireNextImage = LoadDeviceProc("vkAcquireNextImageKHR");
        _fnQueuePresent = LoadDeviceProc("vkQueuePresentKHR");
    }
}

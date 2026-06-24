using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Win32 Surface 创建。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static bool CreateSurface(Vk vk, Silk.NET.Vulkan.Instance inst, nint hi, nint hw, out SurfaceKHR s)
    {
        s = default; var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try { var addr = (nint)vk.GetInstanceProcAddr(inst, (byte*)p); if (addr == 0) return false; var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(addr); var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hi, Hwnd = hw }; fixed (SurfaceKHR* sp = &s) return fn(inst, &ci, null, sp) == Result.Success; }
        finally { Marshal.FreeHGlobal(p); }
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
}

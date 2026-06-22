using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Instance 创建与实例级函数指针加载。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static nint LoadProc(Vk vk, Silk.NET.Vulkan.Instance inst, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetInstanceProcAddr(inst, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    static bool CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance inst)
    {
        inst = default;
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare"); var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface"); var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try { var exts = stackalloc byte*[] { (byte*)s, (byte*)w }; var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackVer(1, 0, 0) }; var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts }; return vk.CreateInstance(&ci, null, out inst) == Result.Success; }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    static uint PackVer(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;
}

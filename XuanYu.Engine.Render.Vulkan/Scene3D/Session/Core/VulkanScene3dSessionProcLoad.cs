using System.Runtime.InteropServices;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Vulkan 函数指针加载辅助方法。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private nint LoadProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    private nint LoadDeviceProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetDeviceProcAddr(_device, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }
}

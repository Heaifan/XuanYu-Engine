using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 AcquireNextImage 阶段。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static Result ProbeAcquireNextImage(VulkanScene3dRenderResources r, nint fnAcquire, out uint imgIndex)
    {
        r.Vk!.WaitForFences(r.Device, 1, ref r.Fence, Vk.True, ulong.MaxValue);
        r.Vk.ResetFences(r.Device, 1, ref r.Fence);
        imgIndex = 0;
        var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(fnAcquire);
        var local = 0u;
        var result = acquireFn(r.Device, r.Swapchain, ulong.MaxValue, r.SemAvail, default, &local);
        imgIndex = local;
        return result;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
}

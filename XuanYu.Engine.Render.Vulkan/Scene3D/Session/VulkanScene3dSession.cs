using System.Runtime.InteropServices;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 持久 Scene3D 渲染会话。
/// 会话级资源（Instance、Device、Shader、VertexBuffer）在整个生命周期保持。
/// Swapchain 级资源在 resize 时重建。
/// 每帧仅重录 CommandBuffer + Submit/Present。
/// </summary>
public sealed unsafe partial class VulkanScene3dSession : IDisposable
{
    // Fields → Session/Core/
    // LoadProc → Session/Core/VulkanScene3dSessionProcLoad.cs

    // ─── 释放 ─────────────────────────────────────────────────────

    public void Dispose()
    {
        _status = VulkanScene3dSessionStatus.Disposed;
        _lastPresentedSnapshot = PresentedCameraSnapshot.Empty;
        if (_vk is null) return;

        if (_devOk && _device.Handle != 0)
            try { _vk.DeviceWaitIdle(_device); } catch { }

        DisposeSessionResources();
        ClearAllResourceFlags();

        if (!VulkanScene3dSwapchainInvariant.IsDisposedValid())
        {
            var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
            System.Diagnostics.Debug.WriteLine(
                $"[严重]Session Dispose 后 Swapchain 不变量失效。\n{diag}");
        }
    }

    // ─── 委托 ────────────────────────────────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result SurfaceSupportFn(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateWin32SurfaceFn(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result AcquireNextImageFn(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result QueuePresentFn(Queue q, PresentInfoKHR* p);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroySurfaceFn(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}

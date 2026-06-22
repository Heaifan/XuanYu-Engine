using System.Diagnostics;
using FluidWarfare.Render.Vulkan.Camera;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>Scene3D 诊断探针编排器。一次性创建→渲染→销毁。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    public static VulkanScene3dInfo RenderWindows(
        nint hinstance, nint hwnd, uint reqW, uint reqH,
        VulkanCameraInfo camera,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        var sw = Stopwatch.StartNew();
        var r = new VulkanScene3dRenderResources();
        try
        {
            r.Vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", sw);

            if (!ProbeCreateSession(r, hinstance, hwnd, reqW, reqH, gridVertices, unitVertices,
                    out var qi, out var fnAcquire, out var fnQueuePresent, out var extent,
                    out var chosenFmt, out var images, out var imgCount, out var gVc, out var uVc,
                    out var createErr))
                return Fail(createErr, sw);

            var info = ProbeRenderFrame(r, camera, extent, chosenFmt, imgCount, gVc, uVc,
                qi, fnAcquire, fnQueuePresent, unitDraws, sw);
            return info;
        }
        finally { r.Dispose(); }
    }
}

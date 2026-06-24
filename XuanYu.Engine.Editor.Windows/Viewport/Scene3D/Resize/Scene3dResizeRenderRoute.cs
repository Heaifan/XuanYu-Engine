using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Vulkan.Clear;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;

/// <summary>视口 Resize / 重绘渲染路由。管理渲染锁、尺寸校验、Session.Resize 和 Clear 降级。</summary>
public sealed class Scene3dResizeRenderRoute
{
    private bool _rendering;

    public Scene3dResizeRenderResult RenderOnce(Scene3dResizeRenderRequest request,
        Scene3dSessionLifecycle lifecycle, VulkanViewportProbeRoute probeRoute,
        Action<string> infoLog, Action<string> warnLog)
    {
        if (!request.BackendAvailable || !request.DeviceCreated || _rendering)
            return Scene3dResizeRenderResult.Skipped;

        _rendering = true;
        try
        {
            if (!IsValidSize(request.NativeHost, out var vpW, out var vpH))
                return Scene3dResizeRenderResult.NotReady;

            if (request.SessionActive && request.Session is not null)
            {
                var pose = request.CameraRoute.CreatePose();
                var drawList = Scene3dDrawListBuilder.Build(request.RenderScene);
                var result = request.Session.Resize(vpW, vpH, pose, [.. drawList]);

                if (result.Success)
                    return Scene3dResizeRenderResult.Resized(
                        $"Scene3D resize：{result.ViewportWidth}x{result.ViewportHeight}");

                warnLog($"Scene3D resize 失败：{result.Message}，回退 Clear。");
                lifecycle.Stop();
                var seq = request.RenderSeq + 1;
                DoClear(probeRoute, request.NativeHost, vpW, vpH, seq, infoLog, warnLog);
                return Scene3dResizeRenderResult.Failure(null, seq);
            }
            else
            {
                var seq = request.RenderSeq + 1;
                DoClear(probeRoute, request.NativeHost, vpW, vpH, seq, infoLog, warnLog);
                return Scene3dResizeRenderResult.ClearOnly(seq);
            }
        }
        finally { _rendering = false; }
    }

    private static bool IsValidSize(VulkanViewportNativeHostInfo host, out uint w, out uint ht)
    {
        w = 0; ht = 0;
        if (host.Width < 1 || host.Height < 1) return false;
        w = checked((uint)host.Width);
        ht = checked((uint)host.Height);
        return true;
    }

    private static void DoClear(VulkanViewportProbeRoute probe, VulkanViewportNativeHostInfo host,
        uint w, uint h, int seq, Action<string> info, Action<string> warn)
    {
        if (!host.HasNativeHandle || host.InstanceHandle == 0 || host.WindowHandle == 0)
        { probe.State.Clear = new(VulkanClearStatus.Failed, "缺少原生句柄，跳过清屏。", "未知", 0, 0, 0); return; }
        info($"RenderSeq-{seq:D3} | Clear | {w}x{h} | resize");
        probe.ProbeClear(host, w, h, "resize", info, warn);
    }
}

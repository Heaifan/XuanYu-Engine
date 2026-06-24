using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Render.Vulkan.Scene3D;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Scene3D.Commands;

public sealed class EditorScene3dCommandRoute
{
    public EditorScene3dCommandState State { get; } = new();

    public EditorScene3dCommandResult Execute(EditorScene3dCommandRequest r)
    {
        return r.Kind switch
        {
            EditorScene3dCommandKind.Run => ExecuteRun(r),
            EditorScene3dCommandKind.Restart => ExecuteRestart(r),
            _ => new(false, false, r.CurrentRenderSeq, false)
        };
    }

    EditorScene3dCommandResult ExecuteRun(EditorScene3dCommandRequest r)
    {
        if (!r.ProbeRoute.State.Gate.CanRun)
        { r.ProbeRoute.State.Scene3d = new(VulkanScene3dStatus.NotChecked, r.ProbeRoute.State.Gate.Message, 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, "不可用（已隔离）", 0); ShowInfo(r); return new(false, true, r.CurrentRenderSeq, false); }
        var h = r.NativeHostInfo;
        if (!h.HasNativeHandle || h.InstanceHandle == 0 || h.WindowHandle == 0)
        { r.ProbeRoute.State.Scene3d = new(VulkanScene3dStatus.Failed, "场景3D：视口未就绪，跳过运行。", 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, "不可用", 0); ShowInfo(r); return new(false, true, r.CurrentRenderSeq, false); }
        var vpW = (uint)Math.Max(h.Width, 1); var vpH = (uint)Math.Max(h.Height, 1);
        var grid = VulkanScene3dVertices.BuildGrid(20, 2); var cube = VulkanScene3dVertices.BuildCube(0, 0, 0, 1.0f);
        var draws = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var o in r.RenderSceneStore.Current.Objects)
        { if (o.VisualKind == RenderObjectVisualKind.UnitMarker) { var p = o.Placement; draws.Add(new(o.EntityId.Value.ToString(), (float)(p?.VisualCenter.X ?? o.Position.X), (float)(p?.VisualCenter.Y ?? o.Position.Y), (float)(p?.VisualCenter.Z ?? o.Position.Z + RenderUnitPlacement.HalfExtent), (float)RenderUnitPlacement.Scale)); } }
        var seq = r.CurrentRenderSeq + 1;
        r.InfoLog($"RenderSeq-{seq:D3} | Scene3D | {vpW}x{vpH} | 手动触发");
        r.ProbeRoute.State.Scene3d = VulkanScene3dRenderer.RenderWindows(h.InstanceHandle, h.WindowHandle, vpW, vpH, VulkanCameraInfo.DefaultBattlefield, grid.AsSpan(), cube.AsSpan(), [.. draws]);
        ShowInfo(r);
        return new(false, true, seq, false);
    }

    EditorScene3dCommandResult ExecuteRestart(EditorScene3dCommandRequest r)
    {
        if (r.Lifecycle.State.Session is not null)
        {
            r.Lifecycle.Stop();
            if (VulkanScene3dSwapchainResources.LiveCount != 0)
            { r.WarnLog($"拒绝重启 Scene3D：仍有 {VulkanScene3dSwapchainResources.LiveCount} 个 Swapchain 存活。"); return new(false, false, r.CurrentRenderSeq, false); }
        }
        if (!r.ProbeRoute.State.Gate.CanRun) { r.WarnLog(r.ProbeRoute.State.Gate.Message); return new(false, false, r.CurrentRenderSeq, false); }
        var h = r.NativeHostInfo;
        if (!h.HasNativeHandle || h.Width < 1 || h.Height < 1) { r.WarnLog("Scene3D 会话：视口未就绪。"); return new(false, false, r.CurrentRenderSeq, false); }
        r.CameraRoute.Reset();
        var startReq = new Scene3dSessionStartRequest(h.InstanceHandle, h.WindowHandle, (uint)h.Width, (uint)h.Height, r.CameraRoute.CreatePose());
        var result = r.Lifecycle.Start(startReq);
        if (!result.Success) return new(false, true, r.CurrentRenderSeq, false);
        var seq = r.CurrentRenderSeq + 1;
        r.InfoLog($"RenderSeq-{seq:D3} | Scene3D Session 启动 | {h.Width}x{h.Height}");
        return new(true, true, seq, true);
    }

    void ShowInfo(EditorScene3dCommandRequest r)
    {
        var s3d = r.ProbeRoute.State.Scene3d;
        if (s3d.IsSucceeded) { r.InfoLog(s3d.Message); } else if (s3d.Status != VulkanScene3dStatus.NotChecked) { r.WarnLog($"Vulkan 3D 场景绘制失败：{s3d.Message}"); }
    }
}

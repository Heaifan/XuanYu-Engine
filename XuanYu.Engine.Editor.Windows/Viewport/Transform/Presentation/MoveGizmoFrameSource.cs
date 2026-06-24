using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Overlay;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Presentation;

/// <summary>
/// Move Gizmo 帧数据源。纯计算：输入相机/实体/工具状态 → 输出顶点 + Pending Snapshot。
/// 不访问 VulkanScene3dSession/WorldState/RenderScene/Shell。
/// </summary>
public static class MoveGizmoFrameSource
{
    public static MoveGizmoFrameResult Build(MoveGizmoFrameInput input)
    {
        var camera = input.Camera;
        if (!camera.IsValid) return MoveGizmoFrameResult.Empty;
        if (!input.MoveToolActive) return MoveGizmoFrameResult.Empty;

        var vp = camera.ViewProjection;
        var w = camera.ViewportWidth;
        var h = camera.ViewportHeight;
        var pose = camera.CameraPose;
        var pivot = input.EntityPosition;

        // 投影 Pivot 到屏幕
        if (!TryProject(pivot, vp, w, h, out var pp))
            return MoveGizmoFrameResult.Empty;

        // 计算世界单位每像素
        var camDist = Math.Sqrt(
            Math.Pow(pose.PositionX - pose.TargetX, 2) +
            Math.Pow(pose.PositionY - pose.TargetY, 2) +
            Math.Pow(pose.PositionZ - pose.TargetZ, 2));
        var vh = Math.Max(1, h);
        var wpp = pose.ProjectionMode == SceneProjectionMode.Orthographic
            ? pose.OrthographicHeight / vh
            : 2.0 * camDist * Math.Tan(pose.FieldOfViewDegrees * Math.PI / 360.0) / vh;

        const double gizmoScreenLen = 80.0;
        var worldLen = gizmoScreenLen * wpp;

        // 三轴端点
        var axes = new[] { Vector3d.UnitX, Vector3d.UnitY, Vector3d.UnitZ };
        var ep = new (double X, double Y)[3];
        var degen = new bool[3];
        for (var i = 0; i < 3; i++)
        {
            var end = pivot + axes[i] * worldLen;
            if (TryProject(end, vp, w, h, out var e))
            { ep[i] = e; degen[i] = false; }
            else
            { ep[i] = pp; degen[i] = true; }
        }

        var layout = MoveGizmoLayout.Build(
            (pp.X, pp.Y), (ep[0].X, ep[0].Y), (ep[1].X, ep[1].Y), (ep[2].X, ep[2].Y),
            degen[0], degen[1], degen[2]);
        if (layout is null) return MoveGizmoFrameResult.Empty;

        var drawVerts = MoveGizmoDrawList.Build(layout,
            MoveGizmoVisualState.Normal, MoveGizmoElement.None, input.HoveredElement);
        var overlayVerts = new VulkanOverlayVertex[drawVerts.Length];
        for (var i = 0; i < drawVerts.Length; i++)
            overlayVerts[i] = new VulkanOverlayVertex(
                drawVerts[i].X, drawVerts[i].Y,
                drawVerts[i].R, drawVerts[i].G,
                drawVerts[i].B, drawVerts[i].A);

        var pending = new PresentedMoveGizmoSnapshot(
            true, input.SelectedEntityId.Value.ToString(),
            0, input.SelectionRevision, camera.CameraRevision, w, h, layout);

        return new MoveGizmoFrameResult(overlayVerts, pending);
    }

    static bool TryProject(Vector3d world, float[] vp, int w, int h,
        out (double X, double Y) pixel)
    {
        pixel = default;
        if (vp is not { Length: 16 } || w <= 0 || h <= 0) return false;
        var cw = vp[3] * world.X + vp[7] * world.Y + vp[11] * world.Z + vp[15];
        if (!double.IsFinite(cw) || Math.Abs(cw) < 1e-6) return false;
        var nx = (vp[0] * world.X + vp[4] * world.Y + vp[8] * world.Z + vp[12]) / cw;
        var ny = (vp[1] * world.X + vp[5] * world.Y + vp[9] * world.Z + vp[13]) / cw;
        if (!double.IsFinite(nx) || !double.IsFinite(ny)) return false;
        pixel = ((nx * 0.5 + 0.5) * w, (ny * 0.5 + 0.5) * h);
        return true;
    }
}

using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 从相机快照构建 AxisTranslationAnchor。
/// 主路径使用 ScreenProjection（稳定可靠），
/// 同时尝试附加 DragPlane 射线约束数据以供 Move 选用。
/// </summary>
public static class AxisDragAnchorBuilder
{
    public static (AxisTranslationAnchor Anchor, bool Success) Build(
        Vector3d axis, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var pose = camera.CameraPose;
        if (pose is null) return (default, false);

        // ── 主路径：ScreenProjection ─────────────────────
        var vp = camera.ViewProjection; var vw = camera.ViewportWidth; var vh = camera.ViewportHeight;
        if (!AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out var dir, out var ppu))
        {
            // 退化降级：基于距离的像素/世界单位比例
            var cd = Math.Sqrt(Math.Pow(pose.PositionX - pose.TargetX, 2) + Math.Pow(pose.PositionY - pose.TargetY, 2) + Math.Pow(pose.PositionZ - pose.TargetZ, 2));
            var fppu = pose.ProjectionMode == SceneProjectionMode.Orthographic ? pose.OrthographicHeight / Math.Max(1, vh) : 2.0 * cd * Math.Tan(pose.FieldOfViewDegrees * Math.PI / 360.0) / Math.Max(1, vh);
            if (fppu <= 0) return (default, false);
            ppu = 1.0 / fppu; dir = new Vector2d(0, -1);
        }

        var anchor = new AxisTranslationAnchor(currentPosition, axis, pivot, ppu, dir, x, y, AxisTranslationMode.ScreenProjection);

        // ── 附加：尝试构建 DragPlane 数据（可选） ────────
        var status = VulkanSceneRayBuilder.TryBuild((float)x, (float)y, camera, (uint)vw, (uint)vh, out var ray);
        if (status == SceneRayBuildStatus.Success && ray is not null)
        {
            var fwd = new Vector3d(pose.TargetX - pose.PositionX, pose.TargetY - pose.PositionY, pose.TargetZ - pose.PositionZ);
            if (!fwd.IsZero)
            {
                fwd = fwd.Normalize();
                var up = new Vector3d(pose.UpX, pose.UpY, pose.UpZ);
                var right = new Vector3d(fwd.Y * up.Z - fwd.Z * up.Y, fwd.Z * up.X - fwd.X * up.Z, fwd.X * up.Y - fwd.Y * up.X);
                var pn = BuildPlaneNormal(axis, fwd, right.IsZero ? new Vector3d(1, 0, 0) : right.Normalize(), up);
                if (pn is not null)
                {
                    var denom = ray.Direction.Dot(pn.Value);
                    if (Math.Abs(denom) >= 0.1) // 阈值 0.1，排除边界退化情况
                    {
                        var t = (pivot - ray.Origin).Dot(pn.Value) / denom;
                        if (t > 0)
                            anchor = anchor with { Mode = AxisTranslationMode.DragPlane, StartIntersection = ray.Origin + ray.Direction * t, DragPlaneNormal = pn.Value, CameraForward = fwd, CameraRight = right, CameraUp = up };
                    }
                }
            }
        }

        return (anchor, true);
    }

    static Vector3d? BuildPlaneNormal(Vector3d axis, Vector3d fwd, Vector3d right, Vector3d up)
    {
        var n = fwd - fwd.Dot(axis) * axis;
        if (n.Length > 1e-6) return n.Normalize();
        n = right - right.Dot(axis) * axis;
        if (n.Length > 1e-6) return n.Normalize();
        n = up - up.Dot(axis) * axis;
        return n.Length > 1e-6 ? n.Normalize() : null;
    }
}

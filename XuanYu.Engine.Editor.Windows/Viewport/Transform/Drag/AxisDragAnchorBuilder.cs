using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

/// <summary>从相机快照构建 AxisTranslationAnchor。优先使用屏幕轴累计，退化时回退 DragPlane。</summary>
public static class AxisDragAnchorBuilder
{
    public static (AxisTranslationAnchor Anchor, bool Success) Build(
        Vector3d axis, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var pose = camera.CameraPose;
        if (pose is null) return (default, false);
        var camForward = new Vector3d(
            pose.TargetX - pose.PositionX,
            pose.TargetY - pose.PositionY,
            pose.TargetZ - pose.PositionZ).Normalize();

        var vp = camera.ViewProjection; var vw = camera.ViewportWidth; var vh = camera.ViewportHeight;
        var hasScreenMetric = AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out var dir, out var ppu);
        if (hasScreenMetric)
            return (new AxisTranslationAnchor(currentPosition, axis, pivot, ppu, dir, x, y, AxisTranslationMode.ScreenProjection), true);
        if (TryBuildDragPlane(axis, x, y, pivot, camera, currentPosition, camForward, out var dragPlane))
            return (dragPlane, true);

        var cd = Math.Sqrt(Math.Pow(pose.PositionX - pose.TargetX, 2) + Math.Pow(pose.PositionY - pose.TargetY, 2) + Math.Pow(pose.PositionZ - pose.TargetZ, 2));
        var fppu = pose.ProjectionMode == SceneProjectionMode.Orthographic ? pose.OrthographicHeight / Math.Max(1, vh) : 2.0 * cd * Math.Tan(pose.FieldOfViewDegrees * Math.PI / 360.0) / Math.Max(1, vh);
        if (fppu <= 0) return (default, false);
        return (new AxisTranslationAnchor(currentPosition, axis, pivot, 1.0 / fppu, new Vector2d(0, -1), x, y, AxisTranslationMode.ScreenProjection), true);
    }

    static bool TryBuildDragPlane(
        Vector3d axis, double x, double y, Vector3d pivot, PresentedCameraSnapshot camera,
        Vector3d currentPosition, Vector3d camForward, out AxisTranslationAnchor anchor)
    {
        anchor = default;
        var planeNormal = Reject(camForward, axis);
        if (planeNormal.IsZero) planeNormal = Reject(Vector3d.UnitY, axis);
        if (planeNormal.IsZero) planeNormal = Reject(Vector3d.UnitX, axis);
        if (planeNormal.IsZero) return false;
        planeNormal = planeNormal.Normalize();

        var status = VulkanSceneRayBuilder.TryBuild((float)x, (float)y, camera,
            (uint)camera.ViewportWidth, (uint)camera.ViewportHeight, out var ray);
        if (status != SceneRayBuildStatus.Success || ray is null) return false;
        var denom = ray.Direction.Dot(planeNormal);
        if (Math.Abs(denom) < 1e-10) return false;
        var t = (pivot - ray.Origin).Dot(planeNormal) / denom;
        if (t <= 0) return false;

        anchor = new AxisTranslationAnchor(currentPosition, axis, pivot, 0, default, x, y, AxisTranslationMode.DragPlane)
        {
            StartIntersection = ray.At(t),
            DragPlaneNormal = planeNormal,
            CameraForward = camForward,
            CameraRight = Vector3d.UnitX,
            CameraUp = Vector3d.UnitY,
        };
        return true;
    }

    static Vector3d Reject(Vector3d value, Vector3d axis) => value - value.Dot(axis) * axis;
}

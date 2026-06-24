using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 从相机快照构建 AxisTranslationAnchor。
/// 唯一路径：ScreenProjection（稳定可靠，三轴统一）。
/// DragPlane 临时方案已在 9.0D-R2B 中移除，后续独立处理。
/// </summary>
public static class AxisDragAnchorBuilder
{
    public static (AxisTranslationAnchor Anchor, bool Success) Build(
        Vector3d axis, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var pose = camera.CameraPose;
        if (pose is null) return (default, false);

        var vp = camera.ViewProjection; var vw = camera.ViewportWidth; var vh = camera.ViewportHeight;
        if (!AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out var dir, out var ppu))
        {
            // 退化降级：基于距离的像素/世界单位比例
            var cd = Math.Sqrt(Math.Pow(pose.PositionX - pose.TargetX, 2) + Math.Pow(pose.PositionY - pose.TargetY, 2) + Math.Pow(pose.PositionZ - pose.TargetZ, 2));
            var fppu = pose.ProjectionMode == SceneProjectionMode.Orthographic ? pose.OrthographicHeight / Math.Max(1, vh) : 2.0 * cd * Math.Tan(pose.FieldOfViewDegrees * Math.PI / 360.0) / Math.Max(1, vh);
            if (fppu <= 0) return (default, false);
            ppu = 1.0 / fppu; dir = new Vector2d(0, -1);
        }

        return (new AxisTranslationAnchor(currentPosition, axis, pivot, ppu, dir, x, y, AxisTranslationMode.ScreenProjection), true);
    }
}

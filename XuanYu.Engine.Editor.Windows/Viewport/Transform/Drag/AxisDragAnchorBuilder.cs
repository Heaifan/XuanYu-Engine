using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 从相机快照和拖动参数构建 AxisTranslationAnchor。
/// 尝试屏幕投影度量，退化时降级为像素/世界单位比例 fallback。
/// </summary>
public static class AxisDragAnchorBuilder
{
    public static (AxisTranslationAnchor Anchor, bool Success) Build(
        Vector3d axis, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var vp = camera.ViewProjection;
        var vw = camera.ViewportWidth;
        var vh = camera.ViewportHeight;
        var pose = camera.CameraPose;

        if (AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out var dir, out var ppu))
        {
            return (new AxisTranslationAnchor(
                currentPosition, axis, pivot, ppu, dir, x, y,
                AxisTranslationMode.ScreenProjection), true);
        }

        // 退化降级：使用基于距离的像素/世界单位比例
        var isOrtho = pose.ProjectionMode == SceneProjectionMode.Orthographic;
        var camDist = Math.Sqrt(
            Math.Pow(pose.PositionX - pose.TargetX, 2) +
            Math.Pow(pose.PositionY - pose.TargetY, 2) +
            Math.Pow(pose.PositionZ - pose.TargetZ, 2));
        var vh2 = Math.Max(1, vh);
        var fallbackPpu = isOrtho
            ? pose.OrthographicHeight / vh2
            : 2.0 * camDist * Math.Tan(pose.FieldOfViewDegrees * Math.PI / 360.0) / vh2;
        if (fallbackPpu <= 0) return (default, false);

        return (new AxisTranslationAnchor(
            currentPosition, axis, pivot, 1.0 / fallbackPpu, new Vector2d(0, -1),
            x, y, AxisTranslationMode.ScreenProjection), true);
    }
}

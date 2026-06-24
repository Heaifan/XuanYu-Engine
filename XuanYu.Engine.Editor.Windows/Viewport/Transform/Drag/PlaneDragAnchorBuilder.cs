using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Transform.Translation.Plane;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 从相机快照构建射线，与平面求交后创建 PlaneTranslationAnchor。
/// </summary>
public static class PlaneDragAnchorBuilder
{
    public static (PlaneTranslationAnchor Anchor, bool Success) Build(
        Vector3d pivot, Vector3d normal, double x, double y,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var status = VulkanSceneRayBuilder.TryBuild(
            (float)x, (float)y, camera,
            (uint)camera.ViewportWidth, (uint)camera.ViewportHeight,
            out var ray);
        if (status != SceneRayBuildStatus.Success || ray is null)
            return (default, false);

        var denom = ray.Direction.Dot(normal);
        if (Math.Abs(denom) < 1e-10) return (default, false);

        var t = (pivot - ray.Origin).Dot(normal) / denom;
        if (t <= 0) return (default, false);

        var anchor = PlaneTranslationStart.CreateAnchor(
            currentPosition, pivot, normal, ray.At(t));
        return (anchor, true);
    }
}

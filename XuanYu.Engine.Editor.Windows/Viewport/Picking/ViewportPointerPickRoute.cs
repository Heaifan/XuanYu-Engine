using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Pointer;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Selection.Screen;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace FluidWarfare.Editor.Windows.Viewport.Picking;

/// <summary>
/// 视口 Picking 路由。单一职责：将像素坐标转换为拾取结果。
/// 不持有 EditorShell 引用，所有输入通过 ViewportPickRequest 传入。
/// </summary>
public sealed class ViewportPointerPickRoute
{
    /// <summary>执行 Picking。优先级：精确 AABB → 5px 屏幕容错 → Ground → None。</summary>
    public ViewportPickResult Pick(ViewportPickRequest req)
    {
        var snap = req.CameraSnapshot;
        if (!snap.IsValid)
        {
            var fallback = RayBuilder.BuildOrFallback(req);
            if (fallback is null) return ViewportPickResult.None;
            return ViewportPickResult.FromPointerResult(
                ScenePointerPicker.Pick(fallback, req.RenderScene, req.Ground));
        }

        // 1. 精确 Ray-AABB（使用 Presented Pick Snapshot）
        var ray = RayBuilder.Build(req);
        if (ray is null) return ViewportPickResult.None;

        if (req.PickSnapshot.IsValid)
        {
            var exact = ScenePointerPicker.Pick(ray, req.PickSnapshot, req.Ground);
            if (exact.Kind == ScenePointerPickKind.Entity)
                return ViewportPickResult.FromPointerResult(exact);

            // 2. 5px 屏幕空间容错
            var entities = req.PickSnapshot.Entities;
            var span = new ReadOnlySpan<PresentedEntityBounds>([.. entities]);
            var screenHit = ScreenEntityPicker.Pick(
                req.PixelX, req.PixelY,
                snap.ViewProjection, snap.ViewportWidth, snap.ViewportHeight,
                span);
            if (screenHit is not null)
                return ViewportPickResult.FromEntity(EntityId.FromInt(screenHit.Value.EntityId));
        }

        // 3. Ground / None
        return ViewportPickResult.FromPointerResult(
            ScenePointerPicker.Pick(ray, req.RenderScene, req.Ground));
    }
}

/// <summary>射线构建辅助。</summary>
internal static class RayBuilder
{
    public static SceneRay? Build(ViewportPickRequest req)
    {
        var snap = req.CameraSnapshot;
        var status = VulkanSceneRayBuilder.TryBuild(
            req.PixelX, req.PixelY, snap,
            (uint)snap.ViewportWidth, (uint)snap.ViewportHeight,
            out var ray);
        return status == SceneRayBuildStatus.Success ? ray : null;
    }

    public static SceneRay? BuildOrFallback(ViewportPickRequest req)
    {
        // 无有效 Snapshot 时用 fallback 射线
        var dir = new Vector3d(0, 0, -1).Normalize();
        return new SceneRay(new Vector3d(0, 0, 100), dir);
    }
}

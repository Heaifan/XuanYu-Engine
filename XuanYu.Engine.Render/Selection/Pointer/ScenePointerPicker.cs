using XuanYu.Engine.Core.Identity;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection.Presented;

namespace FluidWarfare.Render.Selection.Pointer;

/// <summary>
/// 统一 Pointer Picking 调度器。
/// 支持从 PresentedScenePickSnapshot 拾取（与画面同步）。
/// </summary>
public static class ScenePointerPicker
{
    /// <summary>
    /// 使用 PresentedScenePickSnapshot 执行 Picking。
    /// </summary>
    public static ScenePointerPickResult Pick(
        SceneRay ray,
        PresentedScenePickSnapshot snap,
        SceneGroundPlane ground)
    {
        if (snap.IsValid && snap.Entities.Count > 0)
        {
            EntityId? bestId = null;
            double bestDist = double.MaxValue;
            foreach (var e in snap.Entities)
            {
                if (!SceneRayBoundsIntersection.Test(ray, e.Bounds, out var d))
                    continue;
                if (d >= 0 && d < bestDist) { bestDist = d; bestId = EntityId.FromInt(e.EntityId); }
            }
            if (bestId is not null)
                return ScenePointerPickResult.FromEntity(
                    RenderScenePickResult.Hit(bestId.Value, "", bestDist, ray.At(bestDist), 0));
        }

        var groundHit = SceneRayGroundIntersection.Intersect(ray, ground);
        if (groundHit.IsHit) return ScenePointerPickResult.FromGround(groundHit);
        return ScenePointerPickResult.None;
    }

    /// <summary>
    /// 使用 RenderScene 执行 Picking（兼容旧路径）。
    /// </summary>
    public static ScenePointerPickResult Pick(
        SceneRay ray, RenderScene? scene, SceneGroundPlane ground)
    {
        if (scene is not null && scene.Objects.Count > 0)
        {
            var r = RenderScenePicker.Pick(ray, scene);
            if (r.IsHit) return ScenePointerPickResult.FromEntity(r);
        }
        var gh = SceneRayGroundIntersection.Intersect(ray, ground);
        if (gh.IsHit) return ScenePointerPickResult.FromGround(gh);
        return ScenePointerPickResult.None;
    }
}

using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Render.Selection;

/// <summary>
/// 在 RenderScene 中执行 CPU Ray-AABB Picking。
/// 线性遍历，复杂度 O(n)，适合早期场景规模。
/// 只检测 UnitMarker 类型对象。返回最近命中对象。
/// </summary>
public static class RenderScenePicker
{
    /// <summary>
    /// 对 RenderScene 执行 Picking。
    /// </summary>
    /// <param name="ray">世界空间射线。</param>
    /// <param name="scene">渲染场景。</param>
    /// <returns>Picking 结果，未命中时返回 NoHit。</returns>
    public static RenderScenePickResult Pick(SceneRay ray, RenderScene scene)
    {
        if (scene.Objects.Count == 0)
            return RenderScenePickResult.NoHit;

        EntityId? bestId = null;
        string? bestName = null;
        double bestDistance = double.MaxValue;
        Vector3d? bestHitPos = null;
        var tested = 0;

        foreach (var obj in scene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker)
                continue;

            if (obj.SelectionBounds is null)
                continue;

            tested++;

            if (!SceneRayBoundsIntersection.Test(ray, obj.SelectionBounds, out var dist))
                continue;

            if (dist >= 0 && dist < bestDistance)
            {
                bestDistance = dist;
                bestId = obj.EntityId;
                bestName = obj.DisplayName;
                bestHitPos = ray.At(dist);
            }
        }

        if (bestId is null)
            return RenderScenePickResult.NoHit with { TestedObjectCount = tested };

        return RenderScenePickResult.Hit(
            bestId.Value, bestName ?? "", bestDistance, bestHitPos!.Value, tested);
    }
}

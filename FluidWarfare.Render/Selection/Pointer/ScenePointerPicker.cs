using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Render.Selection.Pointer;

/// <summary>
/// 统一 Pointer Picking 调度器。
/// Picking 优先级固定：Entity AABB > Ground Ray > None。
/// </summary>
public static class ScenePointerPicker
{
    /// <summary>
    /// 执行统一 Picking 检测。
    /// </summary>
    /// <param name="ray">世界空间射线。</param>
    /// <param name="scene">渲染场景（可为空）。</param>
    /// <param name="ground">地面平面定义。</param>
    /// <returns>结构化的 Picking 结果。</returns>
    public static ScenePointerPickResult Pick(
        SceneRay ray,
        RenderScene? scene,
        SceneGroundPlane ground)
    {
        // 1. 实体 AABB 优先
        if (scene is not null && scene.Objects.Count > 0)
        {
            var entityResult = RenderScenePicker.Pick(ray, scene);
            if (entityResult.IsHit)
                return ScenePointerPickResult.FromEntity(entityResult);
        }

        // 2. 地面求交
        var groundHit = SceneRayGroundIntersection.Intersect(ray, ground);
        if (groundHit.IsHit)
            return ScenePointerPickResult.FromGround(groundHit);

        // 3. 未命中
        return ScenePointerPickResult.None;
    }
}

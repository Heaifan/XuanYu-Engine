using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Render.Selection.Ground;

/// <summary>
/// 水平地面（Z-Up，法线 +Z）射线求交。
/// 使用 Direction.Z 测试与 XY 地面的交点。
/// 不依赖 Avalonia/Win32/Vulkan/Editor。
/// </summary>
public static class SceneRayGroundIntersection
{
    private const double Epsilon = 1e-12;

    /// <summary>
    /// 计算射线与水平地面的交点。
    /// </summary>
    /// <param name="ray">世界空间射线，Direction 应已归一化。</param>
    /// <param name="ground">地面平面定义（Z-Up，ElevationZ）。</param>
    /// <returns>结构化求交结果（NoHit 或 Hit）。</returns>
    public static SceneGroundHit Intersect(SceneRay ray, SceneGroundPlane ground)
    {
        // Direction 与地面法线 (0,0,1) 点积 → Direction.Z
        // 如果 Direction.Z 接近零，射线与地面平行，不会相交
        if (Math.Abs(ray.Direction.Z) < Epsilon)
            return SceneGroundHit.NoHit;

        // t = (groundElevation - Origin.Z) / Direction.Z
        var t = (ground.ElevationZ - ray.Origin.Z) / ray.Direction.Z;

        // 交点在相机后方
        if (t < 0)
            return SceneGroundHit.NoHit;

        // 命中位置：P(t) = Origin + Direction * t
        var hitPos = ray.At(t);
        return SceneGroundHit.Hit(t, hitPos);
    }
}

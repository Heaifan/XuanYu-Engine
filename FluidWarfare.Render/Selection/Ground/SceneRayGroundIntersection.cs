using FluidWarfare.Core.Math;

namespace FluidWarfare.Render.Selection.Ground;

/// <summary>
/// 水平地面（Y = Height）射线求交。
/// 不依赖 Avalonia/Win32/Vulkan/Editor。
/// </summary>
public static class SceneRayGroundIntersection
{
    private const double Epsilon = 1e-12;

    /// <summary>
    /// 计算射线与水平地面的交点。
    /// </summary>
    /// <param name="ray">世界空间射线，Direction 应已归一化。</param>
    /// <param name="ground">地面平面定义。</param>
    /// <returns>结构化求交结果（NoHit 或 Hit）。</returns>
    public static SceneGroundHit Intersect(SceneRay ray, SceneGroundPlane ground)
    {
        // Direction 与地面法线 (0,1,0) 点积 → Direction.Y
        // 如果 Direction.Y 接近零，射线与地面平行，不会相交
        if (Math.Abs(ray.Direction.Y) < Epsilon)
            return SceneGroundHit.NoHit;

        // t = (groundHeight - Origin.Y) / Direction.Y
        var t = (ground.Height - ray.Origin.Y) / ray.Direction.Y;

        // 交点在相机后方
        if (t < 0)
            return SceneGroundHit.NoHit;

        // 命中位置：P(t) = Origin + Direction * t
        var hitPos = ray.At(t);
        return SceneGroundHit.Hit(t, hitPos);
    }
}

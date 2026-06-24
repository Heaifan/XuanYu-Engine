using FluidWarfare.Core.Math;

namespace FluidWarfare.Render.Selection;

/// <summary>
/// Slab 法射线与 AABB 相交检测。
/// 对 X/Y/Z 三轴分别计算 tNear/tFar 区间，合并后判断是否命中。
/// 正确处理方向分量为零、射线从内部发射、AABB 在后方等边界。
/// </summary>
public static class SceneRayBoundsIntersection
{
    private const double Epsilon = 1e-12;

    /// <summary>
    /// 测试射线是否与包围盒相交。
    /// </summary>
    /// <param name="ray">世界空间射线，Direction 应已归一化。</param>
    /// <param name="bounds">轴对齐包围盒。</param>
    /// <param name="hitDistance">命中距离（未命中时为 0）。</param>
    /// <returns>是否命中。</returns>
    public static bool Test(SceneRay ray, SceneAxisAlignedBounds bounds, out double hitDistance)
    {
        hitDistance = 0;

        var min = bounds.Minimum;
        var max = bounds.Maximum;
        var origin = ray.Origin;
        var dir = ray.Direction;

        double tNear = double.NegativeInfinity;
        double tFar = double.PositiveInfinity;

        // X 轴
        if (IntersectAxis(origin.X, dir.X, min.X, max.X, ref tNear, ref tFar) == AxisResult.Miss)
            return false;

        // Y 轴
        if (IntersectAxis(origin.Y, dir.Y, min.Y, max.Y, ref tNear, ref tFar) == AxisResult.Miss)
            return false;

        // Z 轴
        if (IntersectAxis(origin.Z, dir.Z, min.Z, max.Z, ref tNear, ref tFar) == AxisResult.Miss)
            return false;

        // 区间无效
        if (tNear > tFar)
            return false;

        // 全部在后方
        if (tFar < 0)
            return false;

        // 命中距离
        hitDistance = tNear >= 0 ? tNear : tFar;
        return true;
    }

    private enum AxisResult { Continue, Miss }

    private static AxisResult IntersectAxis(
        double origin, double direction,
        double min, double max,
        ref double tNear, ref double tFar)
    {
        if (Math.Abs(direction) < Epsilon)
        {
            // 方向分量接近零：射线与轴平行
            if (origin < min || origin > max)
                return AxisResult.Miss; // 射线在包围盒外，永不进入
            return AxisResult.Continue; // 射线在 slab 内
        }

        var invDir = 1.0 / direction;
        var t1 = (min - origin) * invDir;
        var t2 = (max - origin) * invDir;

        var axisNear = Math.Min(t1, t2);
        var axisFar = Math.Max(t1, t2);

        if (axisNear > tNear) tNear = axisNear;
        if (axisFar < tFar) tFar = axisFar;

        return AxisResult.Continue;
    }
}

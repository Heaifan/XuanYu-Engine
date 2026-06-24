using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Render.Selection;

/// <summary>
/// 3D 空间射线，用于 Picking 和命中检测。
/// Origin 和 Direction 均在 World Space 中表达。
/// Direction 保证已归一化且非零。
/// </summary>
public sealed record SceneRay(Vector3d Origin, Vector3d Direction)
{
    /// <summary>
    /// 沿射线移动距离 t 后的位置。
    /// </summary>
    public Vector3d At(double t) =>
        new(
            Origin.X + Direction.X * t,
            Origin.Y + Direction.Y * t,
            Origin.Z + Direction.Z * t);
}

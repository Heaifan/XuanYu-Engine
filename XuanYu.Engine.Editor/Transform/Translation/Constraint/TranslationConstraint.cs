using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Constraint;

/// <summary>
/// 平移约束的两种形式：轴向约束或平面约束。
/// Axis 约束沿单轴移动，Plane 约束在平面内自由移动。
/// </summary>
public abstract record TranslationConstraint
{
    /// <summary>沿 Direction 方向移动（如 UnitX, UnitY, UnitZ）。</summary>
    public sealed record Axis(Vector3d Direction) : TranslationConstraint;

    /// <summary>在法线为 Normal 的平面内移动（如 Z=0 → XY 平面）。</summary>
    public sealed record Plane(Vector3d Normal) : TranslationConstraint;
}

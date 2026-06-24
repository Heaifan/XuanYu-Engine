using FluidWarfare.Core.Math;

namespace FluidWarfare.Project.World.Transform;

/// <summary>
/// 实体 Transform 单一真源。渲染、Picking、Gizmo、检查器全部从此派生。
/// 当前阶段只有 Position 生效。Rotation/Scale 为后续阶段占位。
/// </summary>
public readonly record struct SceneTransform(
    Vector3d Position,
    Vector3d Rotation,
    Vector3d Scale)
{
    public static readonly SceneTransform Identity = new(Vector3d.Zero, Vector3d.Zero, new Vector3d(1, 1, 1));
}

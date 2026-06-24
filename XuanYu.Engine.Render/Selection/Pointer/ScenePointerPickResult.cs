using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Selection.Ground;

namespace FluidWarfare.Render.Selection.Pointer;

/// <summary>
/// 统一 Pointer Picking 结果，包含实体命中、地面命中和未命中三种状态。
/// </summary>
public sealed record ScenePointerPickResult(
    ScenePointerPickKind Kind,
    RenderScenePickResult? EntityResult,
    SceneGroundHit? GroundHit)
{
    public static readonly ScenePointerPickResult None = new(
        ScenePointerPickKind.None, null, null);

    public static ScenePointerPickResult FromEntity(RenderScenePickResult entityResult) =>
        new(ScenePointerPickKind.Entity, entityResult, null);

    public static ScenePointerPickResult FromGround(SceneGroundHit groundHit) =>
        new(ScenePointerPickKind.Ground, null, groundHit);

    /// <summary>命中实体的 EntityId（仅 Kind == Entity 时有效）。</summary>
    public EntityId? EntityId => EntityResult?.EntityId;

    /// <summary>地面命中位置（仅 Kind == Ground 时有效）。</summary>
    public Vector3d? GroundPosition => GroundHit?.WorldPosition;

    /// <summary>地面命中距离（仅 Kind == Ground 时有效）。</summary>
    public double GroundDistance => GroundHit?.Distance ?? 0;

    /// <summary>实体命中距离（仅 Kind == Entity 时有效）。</summary>
    public double EntityDistance => EntityResult?.Distance ?? 0;
}

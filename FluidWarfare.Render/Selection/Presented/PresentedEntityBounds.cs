using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Selection.Presented;

/// <summary>
/// 已呈现帧中单个实体的包围盒快照。
/// 由 PresentedScenePickSnapshotBuilder 在 Present 成功后生成。
/// </summary>
public readonly record struct PresentedEntityBounds(
    int EntityId,
    SceneAxisAlignedBounds Bounds,
    float ViewDepth,
    long TransformRevision);

using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Move.Projection;

/// <summary>
/// 地面移动锚点。记录开始拖动时实体位置、平面交点与鼠标像素位置。
/// 后续所有 PlaneIntersection 映射使用相对差值：
///   TargetPosition = InitialEntityPosition + (CurrentPlaneHit - InitialPlaneHit)
/// 禁止使用 CurrentPlaneHit 作为绝对目标位置。
/// </summary>
public readonly record struct GroundMoveAnchor(
    Vector3d InitialEntityPosition,
    Vector3d InitialPlaneHit,
    double InitialPointerX,
    double InitialPointerY)
{
    /// <summary>锚点是否已初始化（所有分量有限）。</summary>
    public bool IsValid =>
        double.IsFinite(InitialEntityPosition.X) &&
        double.IsFinite(InitialPlaneHit.X) &&
        double.IsFinite(InitialPointerX);
}

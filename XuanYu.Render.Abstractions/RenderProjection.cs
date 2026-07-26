using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderProjection(
    RenderCameraProjection Camera,
    IReadOnlyList<RenderEntityProjection> Entities,
    bool GizmoVisible,
    Vector3d GizmoPosition,
    bool RotateGizmoVisible = false)
{
    public int EntityCount => Entities.Count;
}

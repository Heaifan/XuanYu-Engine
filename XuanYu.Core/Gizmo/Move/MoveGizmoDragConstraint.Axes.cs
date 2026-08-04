using XuanYu.Core.Math;

namespace XuanYu.Core.Gizmo;

public readonly partial record struct MoveGizmoDragConstraint
{
    Vector3d AxisVector() => Axis switch
    {
        MoveGizmoAxis.X => Vector3d.UnitX,
        MoveGizmoAxis.Y => Vector3d.UnitY,
        MoveGizmoAxis.Z => Vector3d.UnitZ,
        _ => throw new InvalidOperationException("未知 Move Gizmo 轴。")
    };

    Vector3d PlaneAxisA() => Axis switch
    {
        MoveGizmoAxis.XY or MoveGizmoAxis.XZ => Vector3d.UnitX,
        MoveGizmoAxis.YZ => Vector3d.UnitY,
        _ => throw new InvalidOperationException("未知 Move Gizmo 平面。")
    };

    Vector3d PlaneAxisB() => Axis switch
    {
        MoveGizmoAxis.XY => Vector3d.UnitY,
        MoveGizmoAxis.XZ => Vector3d.UnitZ,
        MoveGizmoAxis.YZ => Vector3d.UnitZ,
        _ => throw new InvalidOperationException("未知 Move Gizmo 平面。")
    };
}

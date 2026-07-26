using XuanYu.Core.Math;

namespace XuanYu.Core.Gizmo;

// 旋转解算的纯静态数学辅助，与实例状态分离的 partial。
public sealed partial class RotateGizmoDrag
{
    static Vector3d AxisUnit(RotateGizmoAxis axis) => axis switch
    {
        RotateGizmoAxis.X => Vector3d.UnitX,
        RotateGizmoAxis.Y => Vector3d.UnitY,
        RotateGizmoAxis.Z => Vector3d.UnitZ,
        _ => throw new global::System.ArgumentOutOfRangeException(nameof(axis))
    };

    static double Component(Vector3d r, RotateGizmoAxis axis) => axis switch
    {
        RotateGizmoAxis.X => r.X,
        RotateGizmoAxis.Y => r.Y,
        RotateGizmoAxis.Z => r.Z,
        _ => throw new global::System.ArgumentOutOfRangeException(nameof(axis))
    };

    static Vector3d WithComponent(Vector3d r, RotateGizmoAxis axis, double value) => axis switch
    {
        RotateGizmoAxis.X => new Vector3d(value, r.Y, r.Z),
        RotateGizmoAxis.Y => new Vector3d(r.X, value, r.Z),
        RotateGizmoAxis.Z => new Vector3d(r.X, r.Y, value),
        _ => throw new global::System.ArgumentOutOfRangeException(nameof(axis))
    };

    static double UnwrapToPlusMinus180(double delta)
    {
        var d = delta;
        while (d > 180.0) d -= 360.0;
        while (d <= -180.0) d += 360.0;
        return d;
    }
}

using XuanYu.Core.Math;

namespace XuanYu.Core.Gizmo;

public readonly record struct MoveGizmoDragConstraint
{
    public MoveGizmoDragConstraint(MoveGizmoSegment segment, double pointerX, double pointerY)
    {
        if (!double.IsFinite(pointerX) || !double.IsFinite(pointerY))
            throw new ArgumentOutOfRangeException(nameof(pointerX));
        if (segment.Length < 1.0) throw new ArgumentOutOfRangeException(nameof(segment));
        Axis = segment.Axis;
        PointerX = pointerX;
        PointerY = pointerY;
        ScreenX = segment.End.X - segment.Start.X;
        ScreenY = segment.End.Y - segment.Start.Y;
        ScreenLengthSquared = (ScreenX * ScreenX) + (ScreenY * ScreenY);
        PlaneScreenX = 0;
        PlaneScreenY = 0;
    }

    public MoveGizmoAxis Axis { get; }
    public double PointerX { get; }
    public double PointerY { get; }
    public double ScreenX { get; }
    public double ScreenY { get; }
    public double ScreenLengthSquared { get; }
    public double PlaneScreenX { get; }
    public double PlaneScreenY { get; }

    MoveGizmoDragConstraint(
        MoveGizmoAxis axis,
        MoveGizmoSegment a,
        MoveGizmoSegment b,
        double pointerX,
        double pointerY) : this(a, pointerX, pointerY)
    {
        Axis = axis;
        PlaneScreenX = b.End.X - b.Start.X;
        PlaneScreenY = b.End.Y - b.Start.Y;
    }

    public static MoveGizmoDragConstraint Plane(
        MoveGizmoAxis axis,
        MoveGizmoSegment a,
        MoveGizmoSegment b,
        double pointerX,
        double pointerY)
    {
        if (axis is not (MoveGizmoAxis.XY or MoveGizmoAxis.XZ or MoveGizmoAxis.YZ))
            throw new ArgumentOutOfRangeException(nameof(axis));
        if (a.Length < 1.0 || b.Length < 1.0)
            throw new ArgumentOutOfRangeException(nameof(a));
        return new MoveGizmoDragConstraint(axis, a, b, pointerX, pointerY);
    }

    public Vector3d Solve(Vector3d start, double pointerX, double pointerY)
    {
        var dx = pointerX - PointerX;
        var dy = pointerY - PointerY;
        if (Axis is MoveGizmoAxis.XY or MoveGizmoAxis.XZ or MoveGizmoAxis.YZ)
            return SolvePlane(start, dx, dy);
        var projected = ((dx * ScreenX) + (dy * ScreenY)) / ScreenLengthSquared;
        var distance = projected * MoveGizmoLayout.AxisLength;
        return start + (AxisVector() * distance);
    }

    Vector3d SolvePlane(Vector3d start, double dx, double dy)
    {
        var det = (ScreenX * PlaneScreenY) - (ScreenY * PlaneScreenX);
        if (global::System.Math.Abs(det) < 0.000001) return start;
        var a = ((dx * PlaneScreenY) - (dy * PlaneScreenX)) / det;
        var b = ((ScreenX * dy) - (ScreenY * dx)) / det;
        return start + (PlaneAxisA() * (a * MoveGizmoLayout.AxisLength))
            + (PlaneAxisB() * (b * MoveGizmoLayout.AxisLength));
    }

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

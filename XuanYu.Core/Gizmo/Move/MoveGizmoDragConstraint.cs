using XuanYu.Core.Math;

namespace XuanYu.Core.Gizmo;

public readonly partial record struct MoveGizmoDragConstraint
{
    public MoveGizmoDragConstraint(MoveGizmoSegment segment, double pointerX, double pointerY)
        : this(segment, pointerX, pointerY, MoveGizmoLayout.AxisLength)
    {
    }

    public MoveGizmoDragConstraint(
        MoveGizmoSegment segment, double pointerX, double pointerY, double worldAxisLength)
    {
        if (!double.IsFinite(pointerX) || !double.IsFinite(pointerY))
            throw new ArgumentOutOfRangeException(nameof(pointerX));
        if (segment.Length < 1.0) throw new ArgumentOutOfRangeException(nameof(segment));
        if (!double.IsFinite(worldAxisLength) || worldAxisLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(worldAxisLength));
        Axis = segment.Axis;
        WorldAxisLength = worldAxisLength;
        PointerX = pointerX;
        PointerY = pointerY;
        ScreenX = segment.End.X - segment.Start.X;
        ScreenY = segment.End.Y - segment.Start.Y;
        ScreenLengthSquared = (ScreenX * ScreenX) + (ScreenY * ScreenY);
        PlaneScreenX = 0;
        PlaneScreenY = 0;
    }

    public MoveGizmoAxis Axis { get; }
    public double WorldAxisLength { get; }
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
        double pointerY,
        double worldAxisLength) : this(a, pointerX, pointerY, worldAxisLength)
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
        double pointerY,
        double worldAxisLength = MoveGizmoLayout.AxisLength)
    {
        if (axis is not (MoveGizmoAxis.XY or MoveGizmoAxis.XZ or MoveGizmoAxis.YZ))
            throw new ArgumentOutOfRangeException(nameof(axis));
        if (a.Length < 1.0 || b.Length < 1.0)
            throw new ArgumentOutOfRangeException(nameof(a));
        return new MoveGizmoDragConstraint(axis, a, b, pointerX, pointerY, worldAxisLength);
    }

    public Vector3d Solve(Vector3d start, double pointerX, double pointerY)
    {
        var dx = pointerX - PointerX;
        var dy = pointerY - PointerY;
        if (Axis is MoveGizmoAxis.XY or MoveGizmoAxis.XZ or MoveGizmoAxis.YZ)
            return SolvePlane(start, dx, dy);
        var projected = ((dx * ScreenX) + (dy * ScreenY)) / ScreenLengthSquared;
        var distance = projected * WorldAxisLength;
        return start + (AxisVector() * distance);
    }

    Vector3d SolvePlane(Vector3d start, double dx, double dy)
    {
        var det = (ScreenX * PlaneScreenY) - (ScreenY * PlaneScreenX);
        if (global::System.Math.Abs(det) < 0.000001) return start;
        var a = ((dx * PlaneScreenY) - (dy * PlaneScreenX)) / det;
        var b = ((ScreenX * dy) - (ScreenY * dx)) / det;
        return start + (PlaneAxisA() * (a * WorldAxisLength))
            + (PlaneAxisB() * (b * WorldAxisLength));
    }

}

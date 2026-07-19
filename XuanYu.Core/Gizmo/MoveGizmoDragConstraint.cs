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
    }

    public MoveGizmoAxis Axis { get; }
    public double PointerX { get; }
    public double PointerY { get; }
    public double ScreenX { get; }
    public double ScreenY { get; }
    public double ScreenLengthSquared { get; }

    public Vector3d Solve(Vector3d start, double pointerX, double pointerY)
    {
        var dx = pointerX - PointerX;
        var dy = pointerY - PointerY;
        var distance = (((dx * ScreenX) + (dy * ScreenY)) / ScreenLengthSquared) * MoveGizmoLayout.AxisLength;
        return start + (AxisVector() * distance);
    }

    Vector3d AxisVector() => Axis switch
    {
        MoveGizmoAxis.X => Vector3d.UnitX,
        MoveGizmoAxis.Y => Vector3d.UnitY,
        MoveGizmoAxis.Z => Vector3d.UnitZ,
        _ => throw new InvalidOperationException("未知 Move Gizmo 轴。")
    };
}

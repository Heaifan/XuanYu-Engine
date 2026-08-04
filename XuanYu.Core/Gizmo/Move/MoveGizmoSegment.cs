namespace XuanYu.Core.Gizmo;

public readonly record struct MoveGizmoSegment(
    MoveGizmoAxis Axis,
    ScreenPoint Start,
    ScreenPoint End)
{
    public double Length => global::System.Math.Sqrt(
        ((End.X - Start.X) * (End.X - Start.X)) +
        ((End.Y - Start.Y) * (End.Y - Start.Y)));
}

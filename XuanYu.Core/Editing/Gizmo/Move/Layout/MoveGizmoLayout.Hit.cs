namespace XuanYu.Core.Gizmo;

public sealed partial class MoveGizmoLayout
{
    static (MoveGizmoAxis Axis, double Distance, double Alignment) Hit(
        MoveGizmoSegment segment,
        ScreenPoint origin,
        double x,
        double y)
    {
        return (segment.Axis, Distance(segment, x, y), Alignment(segment, origin, x, y));
    }

    static double Distance(MoveGizmoSegment segment, double x, double y)
    {
        var dx = segment.End.X - segment.Start.X;
        var dy = segment.End.Y - segment.Start.Y;
        var length2 = (dx * dx) + (dy * dy);
        var sx = x - segment.Start.X;
        var sy = y - segment.Start.Y;
        var t = ((sx * dx) + (sy * dy)) / length2;
        t = global::System.Math.Clamp(t, 0, 1);
        var px = segment.Start.X + (t * dx);
        var py = segment.Start.Y + (t * dy);
        return global::System.Math.Sqrt(((x - px) * (x - px)) + ((y - py) * (y - py)));
    }

    static double Alignment(MoveGizmoSegment segment, ScreenPoint origin, double x, double y)
    {
        var ax = segment.End.X - segment.Start.X;
        var ay = segment.End.Y - segment.Start.Y;
        var px = x - origin.X;
        var py = y - origin.Y;
        var axisLength = segment.Length;
        var pointerLength = global::System.Math.Sqrt((px * px) + (py * py));
        if (pointerLength < 0.000001) return 1.0;
        return ((px * ax) + (py * ay)) / (pointerLength * axisLength);
    }
}

using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

public sealed class MoveGizmoLayout
{
    public const double AxisLength = 1.2;
    public const double HitWidth = 18.0;
    public const double GuardWidth = 48.0;

    MoveGizmoLayout(MoveGizmoSegment[] segments) => Segments = segments;

    public IReadOnlyList<MoveGizmoSegment> Segments { get; }

    public static MoveGizmoLayout Project(ViewProjectionState state, Vector3d origin)
    {
        var start = state.ProjectWorldPoint(origin);
        return new MoveGizmoLayout(
        [
            Segment(MoveGizmoAxis.X, start, state.ProjectWorldPoint(origin + new Vector3d(AxisLength, 0, 0))),
            Segment(MoveGizmoAxis.Y, start, state.ProjectWorldPoint(origin + new Vector3d(0, AxisLength, 0))),
            Segment(MoveGizmoAxis.Z, start, state.ProjectWorldPoint(origin + new Vector3d(0, 0, AxisLength)))
        ]);
    }

    public MoveGizmoAxis? HitTest(double x, double y, double width = HitWidth)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(x));
        var origin = Segments[0].Start;
        return Segments
            .Select(segment => Hit(segment, origin, x, y))
            .Where(hit => hit.Distance <= width && hit.Alignment >= 0)
            .OrderByDescending(hit => hit.Alignment)
            .ThenBy(hit => hit.Distance)
            .ThenBy(hit => hit.Axis)
            .Select(hit => (MoveGizmoAxis?)hit.Axis)
            .FirstOrDefault();
    }

    public MoveGizmoAxis? GuardHitTest(double x, double y) => HitTest(x, y, GuardWidth);

    static MoveGizmoSegment Segment(MoveGizmoAxis axis, ScreenPoint start, ScreenPoint end)
    {
        var segment = new MoveGizmoSegment(axis, start, end);
        if (segment.Length < 1.0) throw new InvalidOperationException($"Gizmo {axis} 轴投影已退化。");
        return segment;
    }

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
        var t = (((x - segment.Start.X) * dx) + ((y - segment.Start.Y) * dy)) / length2;
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

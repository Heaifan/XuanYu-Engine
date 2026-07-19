using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

public sealed class MoveGizmoLayout
{
    public const double AxisLength = 1.2;
    public const double HitWidth = 18.0;

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
        return Segments
            .Select(segment => (segment.Axis, Distance(segment, x, y)))
            .Where(hit => hit.Item2 <= width)
            .OrderBy(hit => hit.Item2)
            .ThenBy(hit => hit.Axis)
            .Select(hit => (MoveGizmoAxis?)hit.Axis)
            .FirstOrDefault();
    }

    static MoveGizmoSegment Segment(MoveGizmoAxis axis, ScreenPoint start, ScreenPoint end)
    {
        var segment = new MoveGizmoSegment(axis, start, end);
        if (segment.Length < 1.0) throw new InvalidOperationException($"Gizmo {axis} 轴投影已退化。");
        return segment;
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
}

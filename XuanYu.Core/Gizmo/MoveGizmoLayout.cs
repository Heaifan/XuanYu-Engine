using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

public sealed partial class MoveGizmoLayout
{
    public const double AxisLength = 1.2;

    // 可见轴杆线宽（DIP）。与 Vulkan 顶点着色器生成的 Gizmo 几何同尺度（审计实测约 2–3px）。
    // 命中区必须以可见几何为唯一真源，禁止另开一套大半径。
    public const double GizmoVisualLineWidth = 2.0;

    // 有限、明确的交互容差（DIP）。仅补偿指针精度，非大范围抢占。
    public const double HitMargin = 5.0;

    // 命中半径 = 可见半径 + 交互容差 ⇒ 看得到 ≈ 点得到。
    public const double HitWidth = (GizmoVisualLineWidth / 2.0) + HitMargin;
    [Obsolete("Use MoveGizmoScreenSize for plane DIP dimensions.")]
    public const double PlaneInset = 0.22;
    [Obsolete("Use MoveGizmoScreenSize for plane DIP dimensions.")]
    public const double PlaneSize = 0.38;
    public double WorldAxisLength { get; }

    MoveGizmoLayout(
        MoveGizmoSegment[] segments, MoveGizmoPlane[] planes, double worldAxisLength)
    {
        Segments = segments;
        Planes = planes;
        WorldAxisLength = worldAxisLength;
    }

    public IReadOnlyList<MoveGizmoSegment> Segments { get; }
    public IReadOnlyList<MoveGizmoPlane> Planes { get; }

    public static MoveGizmoLayout Project(
        ViewProjectionState state, Vector3d origin, double worldAxisLength = AxisLength)
    {
        if (!double.IsFinite(worldAxisLength) || worldAxisLength <= 0)
            worldAxisLength = AxisLength;
        var start = state.ProjectWorldPoint(origin);
        return new MoveGizmoLayout(
        [
            Segment(MoveGizmoAxis.X, start,
                state.ProjectWorldPoint(origin + new Vector3d(worldAxisLength, 0, 0))),
            Segment(MoveGizmoAxis.Y, start,
                state.ProjectWorldPoint(origin + new Vector3d(0, worldAxisLength, 0))),
            Segment(MoveGizmoAxis.Z, start,
                state.ProjectWorldPoint(origin + new Vector3d(0, 0, worldAxisLength)))
        ],
        [
            Plane(state, origin, MoveGizmoAxis.XY, Vector3d.UnitX, Vector3d.UnitY, worldAxisLength),
            Plane(state, origin, MoveGizmoAxis.XZ, Vector3d.UnitX, Vector3d.UnitZ, worldAxisLength),
            Plane(state, origin, MoveGizmoAxis.YZ, Vector3d.UnitY, Vector3d.UnitZ, worldAxisLength)
        ], worldAxisLength);
    }

    public MoveGizmoAxis? HitTest(double x, double y, double width = HitWidth)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(x));
        var visiblePlane = Planes
            .Where(plane => plane.ContainsVisible(x, y))
            .OrderBy(plane => plane.CenterDistanceSquared(x, y))
            .Select(plane => (MoveGizmoPlane?)plane)
            .FirstOrDefault();
        if (visiblePlane is { } visible)
            return visible.Axis;
        var origin = Segments[0].Start;
        var axis = Segments
            .Select(segment => Hit(segment, origin, x, y))
            .Where(hit => hit.Distance <= width && hit.Alignment >= 0)
            .OrderByDescending(hit => hit.Alignment)
            .ThenBy(hit => hit.Distance)
            .ThenBy(hit => hit.Axis)
            .Select(hit => (MoveGizmoAxis?)hit.Axis)
            .FirstOrDefault();
        if (axis is not null) return axis;
        foreach (var plane in Planes)
            if (plane.Contains(x, y)) return plane.Axis;
        return null;
    }

    static MoveGizmoSegment Segment(MoveGizmoAxis axis, ScreenPoint start, ScreenPoint end)
    {
        var segment = new MoveGizmoSegment(axis, start, end);
        if (segment.Length < 1.0) throw new InvalidOperationException($"Gizmo {axis} 轴投影已退化。");
        return segment;
    }

}

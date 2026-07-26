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
    public const double PlaneInset = 0.22;
    public const double PlaneSize = 0.38;

    MoveGizmoLayout(MoveGizmoSegment[] segments, MoveGizmoPlane[] planes)
    {
        Segments = segments;
        Planes = planes;
    }

    public IReadOnlyList<MoveGizmoSegment> Segments { get; }
    public IReadOnlyList<MoveGizmoPlane> Planes { get; }

    public static MoveGizmoLayout Project(ViewProjectionState state, Vector3d origin)
    {
        var start = state.ProjectWorldPoint(origin);
        return new MoveGizmoLayout(
        [
            Segment(MoveGizmoAxis.X, start,
                state.ProjectWorldPoint(origin + new Vector3d(AxisLength, 0, 0))),
            Segment(MoveGizmoAxis.Y, start,
                state.ProjectWorldPoint(origin + new Vector3d(0, AxisLength, 0))),
            Segment(MoveGizmoAxis.Z, start,
                state.ProjectWorldPoint(origin + new Vector3d(0, 0, AxisLength)))
        ],
        [
            Plane(state, origin, MoveGizmoAxis.XY, Vector3d.UnitX, Vector3d.UnitY),
            Plane(state, origin, MoveGizmoAxis.XZ, Vector3d.UnitX, Vector3d.UnitZ),
            Plane(state, origin, MoveGizmoAxis.YZ, Vector3d.UnitY, Vector3d.UnitZ)
        ]);
    }

    public MoveGizmoAxis? HitTest(double x, double y, double width = HitWidth)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(x));
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

    static MoveGizmoPlane Plane(
        ViewProjectionState state,
        Vector3d origin,
        MoveGizmoAxis axis,
        Vector3d a,
        Vector3d b)
    {
        var i = PlaneInset;
        var o = PlaneInset + PlaneSize;
        return new MoveGizmoPlane(
            axis,
            state.ProjectWorldPoint(origin + (a * i) + (b * i)),
            state.ProjectWorldPoint(origin + (a * o) + (b * i)),
            state.ProjectWorldPoint(origin + (a * o) + (b * o)),
            state.ProjectWorldPoint(origin + (a * i) + (b * o)));
    }

}

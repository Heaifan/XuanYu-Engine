using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

public sealed partial class RotateGizmoLayout
{
    // 旋转环世界半径默认值（与 MoveGizmo AxisLength=1.2 同尺度）。
    // 屏幕空间恒定尺寸由调用方按视口/相机换算 worldRadius 覆盖。
    public const double RingRadius = 1.2;
    public const int RingSegments = 48;
    public const double RingVisualWidth = 2.0;

    // 有限、明确的交互容差（DIP）。仅补偿指针精度，非大范围抢占。
    public const double HitMargin = 5.0;

    // 命中半径 = 可见环管半径 + 交互容差 ⇒ 看得到 ≈ 点得到。
    public const double HitWidth = (RingVisualWidth / 2.0) + HitMargin;

    // 屏幕投影半径小于此值视为边视环，不参与命中。
    public const double EdgeOnScreenRadius = 6.0;

    RotateGizmoLayout(RotateGizmoRing[] rings)
    {
        Rings = rings;
    }

    public IReadOnlyList<RotateGizmoRing> Rings { get; }

    public static RotateGizmoLayout Project(
        ViewProjectionState state,
        Vector3d origin,
        double worldRadius = RingRadius)
    {
        return new RotateGizmoLayout(
        [
            Ring(state, origin, RotateGizmoAxis.X, worldRadius),
            Ring(state, origin, RotateGizmoAxis.Y, worldRadius),
            Ring(state, origin, RotateGizmoAxis.Z, worldRadius)
        ]);
    }

    public RotateGizmoAxis? HitTest(double x, double y, double width = HitWidth)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(x));
        RotateGizmoAxis? best = null;
        var bestDist = double.MaxValue;
        foreach (var ring in Rings)
        {
            if (ring.IsEdgeOn) continue;
            var d = ring.NearestDistance(x, y);
            if (d <= width && d < bestDist)
            {
                bestDist = d;
                best = ring.Axis;
            }
        }
        return best;
    }

    static RotateGizmoRing Ring(
        ViewProjectionState state, Vector3d origin, RotateGizmoAxis axis, double worldRadius)
    {
        var (b1, b2) = Basis(axis);
        var points = new ScreenPoint[RingSegments];
        ScreenPoint center;
        // 投影退化（点位于相机后方/非有限）时返回空环（不显示该轴环），不污染布局状态。
        try { center = state.ProjectWorldPoint(origin); }
        catch (InvalidOperationException) { return new RotateGizmoRing(axis, default, [], 0.0); }
        var maxR = 0.0;
        for (var i = 0; i < RingSegments; i++)
        {
            var theta = (i * 2.0 * global::System.Math.PI) / RingSegments;
            var dir = (b1 * global::System.Math.Cos(theta)) + (b2 * global::System.Math.Sin(theta));
            var world = origin + (dir * worldRadius);
            ScreenPoint p;
            // 单个环点投影退化时放弃整环（空点集），避免半环伪影。
            try { p = state.ProjectWorldPoint(world); }
            catch (InvalidOperationException) { return new RotateGizmoRing(axis, center, [], 0.0); }
            points[i] = p;
            var r = global::System.Math.Sqrt(
                ((p.X - center.X) * (p.X - center.X)) + ((p.Y - center.Y) * (p.Y - center.Y)));
            if (r > maxR) maxR = r;
        }
        return new RotateGizmoRing(axis, center, points, maxR);
    }

    internal static (Vector3d, Vector3d) Basis(RotateGizmoAxis axis) => axis switch
    {
        RotateGizmoAxis.X => (Vector3d.UnitY, Vector3d.UnitZ),
        RotateGizmoAxis.Y => (Vector3d.UnitZ, Vector3d.UnitX),
        RotateGizmoAxis.Z => (Vector3d.UnitX, Vector3d.UnitY),
        _ => throw new global::System.ArgumentOutOfRangeException(nameof(axis))
    };
}

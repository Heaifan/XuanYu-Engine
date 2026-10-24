using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class MoveGizmoLayoutTests
{
    [Fact]
    public void Hit_radius_follows_visible_geometry_with_explicit_margin()
    {
        var layout = Layout();
        var x = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.X);
        var midX = (x.Start.X + x.End.X) * 0.5;
        var midY = (x.Start.Y + x.End.Y) * 0.5;
        var dx = x.End.X - x.Start.X;
        var dy = x.End.Y - x.Start.Y;
        var length = x.Length;
        Assert.Equal(MoveGizmoAxis.X, layout.HitTest(midX, midY)); // 可见轴杆中心必命中
        var inTolerance = MoveGizmoLayout.HitMargin * 0.5;
        Assert.Equal(MoveGizmoAxis.X, layout.HitTest(
            midX - (dy / length * inTolerance), midY + (dx / length * inTolerance)));
        var beyond = MoveGizmoLayout.HitMargin * 3.0; // 远超“可见半径+容差”必 MISS
        Assert.Null(layout.HitTest(
            midX - (dy / length * beyond), midY + (dx / length * beyond)));
    }

    [Fact]
    public void Removed_wide_guard_no_longer_captures_far_off_axis_clicks()
    {
        var layout = Layout();
        var x = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.X);
        var midX = (x.Start.X + x.End.X) * 0.5;
        var midY = (x.Start.Y + x.End.Y) * 0.5;
        var dx = x.End.X - x.Start.X;
        var dy = x.End.Y - x.Start.Y;
        var length = x.Length;
        // 旧 48px 守卫会捕获轴外 40px 的点（隐形光环）；修复后仅“可见半径+容差”，必须 MISS。
        Assert.Null(layout.HitTest(midX - (dy / length * 40), midY + (dx / length * 40)));
    }

    [Fact]
    public void Far_from_gizmo_misses_so_click_falls_through_to_picking()
    {
        var layout = Layout();
        // 视口角落远离 Gizmo 原点与所有轴 → 不命中 → 调用方继续 Viewport Picking
        Assert.Null(layout.HitTest(5, 5));
        Assert.Null(layout.HitTest(795, 595));
    }
}

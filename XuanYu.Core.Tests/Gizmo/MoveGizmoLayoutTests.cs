using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class MoveGizmoLayoutTests
{
    [Fact]
    public void Oblique_camera_projects_three_visible_axes()
    {
        var layout = Layout();
        // 命中半径必须由“可见几何 + 显式容差”派生，禁止再开大半径
        Assert.Equal((MoveGizmoLayout.GizmoVisualLineWidth / 2.0) + MoveGizmoLayout.HitMargin,
            MoveGizmoLayout.HitWidth);
        Assert.True(MoveGizmoLayout.HitWidth < 12.0); // 防 P0 隐形大半径回归
        Assert.All(layout.Segments, segment => Assert.True(segment.Length > 20));
        Assert.Equal(3, layout.Segments.Count);
    }

    [Theory]
    [InlineData(MoveGizmoAxis.X)]
    [InlineData(MoveGizmoAxis.Y)]
    [InlineData(MoveGizmoAxis.Z)]
    public void Axis_midpoint_hits_expected_axis(MoveGizmoAxis axis)
    {
        var layout = Layout();
        var segment = layout.Segments.Single(item => item.Axis == axis);

        Assert.Equal(axis, layout.HitTest(
            (segment.Start.X + segment.End.X) * 0.5,
            (segment.Start.Y + segment.End.Y) * 0.5));
    }

    [Fact]
    public void Miss_and_hit_width_boundary_are_stable()
    {
        var layout = Layout();
        var x = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.X);
        var midX = (x.Start.X + x.End.X) * 0.5;
        var midY = (x.Start.Y + x.End.Y) * 0.5;
        var dx = x.End.X - x.Start.X;
        var dy = x.End.Y - x.Start.Y;
        var length = x.Length;

        Assert.Equal(MoveGizmoAxis.X, layout.HitTest(
            midX - (dy / length * 8.9), midY + (dx / length * 8.9), 9.0));
        Assert.Null(layout.HitTest(
            midX - (dy / length * 9.1), midY + (dx / length * 9.1), 9.0));
        Assert.Null(layout.HitTest(5, 5));
    }

    [Fact]
    public void Shared_origin_uses_pointer_direction_before_distance_tie()
    {
        var layout = Layout();
        var y = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.Y);
        var origin = y.Start;
        var dx = y.End.X - y.Start.X;
        var dy = y.End.Y - y.Start.Y;
        var length = y.Length;
        // 显式宽度仅用于覆盖沿轴 28px 的点，验证对齐优先于距离的平局裁决
        Assert.Equal(MoveGizmoAxis.Y, layout.HitTest(
            origin.X + (dx / length * 28),
            origin.Y + (dy / length * 28), 30.0));
    }

    [Fact]
    public void Equal_distance_uses_fixed_axis_order()
    {
        var layout = Layout();
        var origin = layout.Segments[0].Start;

        Assert.Equal(MoveGizmoAxis.X, layout.HitTest(origin.X, origin.Y));
    }

    static MoveGizmoLayout Layout()
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        return MoveGizmoLayout.Project(
            ViewProjectionState.Create(DefaultEditorCamera.Create(1), viewport),
            Vector3d.Zero);
    }
}

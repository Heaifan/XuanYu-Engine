using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed class RotateGizmoLayoutTests
{
    [Fact]
    public void Oblique_camera_projects_three_visible_rings()
    {
        var layout = Layout();
        // 命中半径必须由“可见环几何 + 显式容差”派生，禁止再开大半径
        Assert.Equal((RotateGizmoLayout.RingVisualWidth / 2.0) + RotateGizmoLayout.HitMargin,
            RotateGizmoLayout.HitWidth);
        Assert.True(RotateGizmoLayout.HitWidth < 12.0); // 防 P0 隐形大半径回归
        Assert.Equal(3, layout.Rings.Count);
        Assert.All(layout.Rings, ring => Assert.Equal(RotateGizmoLayout.RingSegments, ring.Points.Count));
        Assert.All(layout.Rings, ring => Assert.False(ring.IsEdgeOn));
    }

    [Theory]
    [InlineData(RotateGizmoAxis.X)]
    [InlineData(RotateGizmoAxis.Y)]
    [InlineData(RotateGizmoAxis.Z)]
    public void Ring_point_at_45_degrees_hits_expected_axis(RotateGizmoAxis axis)
    {
        var layout = Layout();
        var ring = layout.Rings.Single(item => item.Axis == axis);
        // θ=45° 采样点：与相邻环无 3D 交点（轴线交点在 0/90/180/270°），命中唯一
        var p = ring.Points[6];
        Assert.Equal(axis, layout.HitTest(p.X, p.Y));
    }

    [Fact]
    public void Far_point_misses_all_rings()
    {
        var layout = Layout();
        Assert.Null(layout.HitTest(5, 5));
    }

    static RotateGizmoLayout Layout()
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        return RotateGizmoLayout.Project(
            ViewProjectionState.Create(DefaultEditorCamera.Create(1), viewport),
            Vector3d.Zero);
    }
}

using XuanYu.Core.Gizmo;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class MoveGizmoLayoutTests
{
    [Fact]
    public void Project_includes_three_visible_plane_handles()
    {
        var layout = Layout();

        Assert.Equal(3, layout.Segments.Count);
        Assert.Equal(3, layout.Planes.Count);
        Assert.Contains(layout.Planes, p => p.Axis == MoveGizmoAxis.XY);
        Assert.Contains(layout.Planes, p => p.Axis == MoveGizmoAxis.XZ);
        Assert.Contains(layout.Planes, p => p.Axis == MoveGizmoAxis.YZ);
    }

    [Theory]
    [InlineData(MoveGizmoAxis.XY)]
    [InlineData(MoveGizmoAxis.XZ)]
    [InlineData(MoveGizmoAxis.YZ)]
    public void Plane_center_hits_expected_plane(MoveGizmoAxis axis)
    {
        var layout = Layout();
        var plane = layout.Planes.Single(p => p.Axis == axis);
        var x = (plane.A.X + plane.B.X + plane.C.X + plane.D.X) / 4.0;
        var y = (plane.A.Y + plane.B.Y + plane.C.Y + plane.D.Y) / 4.0;

        Assert.Equal(axis, layout.HitTest(x, y));
    }

    [Fact]
    public void Axis_hit_takes_priority_over_plane_hit()
    {
        var layout = Layout();
        var x = layout.Segments.Single(s => s.Axis == MoveGizmoAxis.X);
        var px = x.Start.X + ((x.End.X - x.Start.X) * MoveGizmoLayout.PlaneInset);
        var py = x.Start.Y + ((x.End.Y - x.Start.Y) * MoveGizmoLayout.PlaneInset);

        Assert.Equal(MoveGizmoAxis.X, layout.HitTest(px, py));
    }
}

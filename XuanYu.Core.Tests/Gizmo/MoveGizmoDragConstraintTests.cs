using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Gizmo;

public sealed class MoveGizmoDragConstraintTests
{
    [Theory]
    [InlineData(MoveGizmoAxis.X, 1.2, 0, 0)]
    [InlineData(MoveGizmoAxis.Y, 0, 1.2, 0)]
    [InlineData(MoveGizmoAxis.Z, 0, 0, 1.2)]
    public void Full_projected_axis_drag_moves_one_axis_length(
        MoveGizmoAxis axis, double x, double y, double z)
    {
        var segment = new MoveGizmoSegment(axis, new ScreenPoint(10, 20), new ScreenPoint(40, 60));
        var constraint = new MoveGizmoDragConstraint(segment, 10, 20);

        var result = constraint.Solve(Vector3d.Zero, 40, 60);

        Assert.Equal(new Vector3d(x, y, z), result);
    }

    [Fact]
    public void Perpendicular_pointer_motion_does_not_move_entity()
    {
        var segment = new MoveGizmoSegment(
            MoveGizmoAxis.X,
            new ScreenPoint(0, 0),
            new ScreenPoint(40, 0));
        var constraint = new MoveGizmoDragConstraint(segment, 0, 0);

        Assert.Equal(Vector3d.Zero, constraint.Solve(Vector3d.Zero, 0, 30));
    }

    [Theory]
    [InlineData(MoveGizmoAxis.XY, 1.2, 1.2, 0)]
    [InlineData(MoveGizmoAxis.XZ, 1.2, 0, 1.2)]
    [InlineData(MoveGizmoAxis.YZ, 0, 1.2, 1.2)]
    public void Plane_drag_moves_only_plane_axes(MoveGizmoAxis axis, double x, double y, double z)
    {
        var a = new MoveGizmoSegment(
            MoveGizmoAxis.X,
            new ScreenPoint(0, 0),
            new ScreenPoint(40, 0));
        var b = new MoveGizmoSegment(
            MoveGizmoAxis.Y,
            new ScreenPoint(0, 0),
            new ScreenPoint(0, 40));
        var constraint = MoveGizmoDragConstraint.Plane(axis, a, b, 0, 0);

        var result = constraint.Solve(Vector3d.Zero, 40, 40);

        Assert.Equal(new Vector3d(x, y, z), result);
    }
}

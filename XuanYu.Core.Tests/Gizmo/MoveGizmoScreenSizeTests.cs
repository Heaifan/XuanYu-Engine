using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed class MoveGizmoScreenSizeTests
{
    [Fact]
    public void Dolly_keeps_axis_and_plane_square_screen_size_stable()
    {
        var near = DefaultEditorCamera.Create(1);
        var far = new CameraState(
            near.Position * 2.0, near.Forward, near.Up, near.VerticalFovDegrees,
            near.NearPlane, near.FarPlane, near.Revision);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var a = Layout(near, viewport).Planes[0];
        var b = Layout(far, viewport).Planes[0];

        Assert.InRange(Side(a), 13.0, 19.0);
        Assert.InRange(Side(b), 13.0, 19.0);
        Assert.InRange(System.Math.Abs(Side(a) - Side(b)), 0.0, 2.0);
    }

    [Fact]
    public void Visible_square_and_picking_square_use_separate_sizes()
    {
        var plane = Layout(DefaultEditorCamera.Create(1),
            new ViewportState(0, 0, 800, 600, 800, 600, 1, 1)).Planes[0];

        Assert.True(Distance(plane.HitA, plane.HitB) > Distance(plane.A, plane.B));
        Assert.Equal(MoveGizmoAxis.XY, Layout(DefaultEditorCamera.Create(1),
            new ViewportState(0, 0, 800, 600, 800, 600, 1, 1))
            .HitTest((plane.HitA.X + plane.HitB.X + plane.HitC.X + plane.HitD.X) / 4,
                (plane.HitA.Y + plane.HitB.Y + plane.HitC.Y + plane.HitD.Y) / 4));
    }

    [Fact]
    public void Move_layout_does_not_consume_entity_rotation_or_scale()
    {
        var camera = DefaultEditorCamera.Create(1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(camera, viewport);
        var origin = new Vector3d(2, 1, 0);
        var length = MoveGizmoScreenSize.ComputeWorldAxisLength(camera, viewport, origin);
        var baseline = MoveGizmoLayout.Project(state, origin, length);

        foreach (var scale in new[] { 0.1, 1.0, 100.0 })
        {
            _ = scale;
            var projected = MoveGizmoLayout.Project(state, origin, length);
            Assert.Equal(baseline.Segments.Select(x => x.End), projected.Segments.Select(x => x.End));
        }
    }

    static MoveGizmoLayout Layout(CameraState camera, ViewportState viewport)
    {
        var state = ViewProjectionState.Create(camera, viewport);
        var length = MoveGizmoScreenSize.ComputeWorldAxisLength(camera, viewport, Vector3d.Zero);
        return MoveGizmoLayout.Project(state, Vector3d.Zero, length);
    }

    static double Side(MoveGizmoPlane plane) => Distance(plane.A, plane.B);

    static double Distance(ScreenPoint a, ScreenPoint b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return System.Math.Sqrt((dx * dx) + (dy * dy));
    }
}

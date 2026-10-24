using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class ScaleGizmoTests
{
    [Fact]
    public void R5R1_hit_center_core_returns_uniform_before_axes()
    {
        var layout = Layout(Vector3d.Zero);
        Assert.Equal(ScaleGizmoHandle.Uniform,
            ScaleGizmoHitTester.HitTest(layout, layout.Center.X + 6, layout.Center.Y));
    }

    [Fact]
    public void R5R1_axis_hits_outside_center_return_their_axis()
    {
        var layout = Layout(Vector3d.Zero);
        for (var i = 0; i < 3; i++)
        {
            var p = PointOnAxis(layout, i, 0.76);
            var expected = i == 0 ? ScaleGizmoHandle.X :
                (i == 1 ? ScaleGizmoHandle.Y : ScaleGizmoHandle.Z);
            Assert.Equal(expected, ScaleGizmoHitTester.HitTest(layout, p.X, p.Y));
        }
    }

    [Fact]
    public void R5R1_uniform_preview_preserves_original_scale_ratio()
    {
        var drag = new ScaleGizmoDrag(
            ScaleGizmoHandle.Uniform, new Vector3d(1, 2, 3), 200, 200, default);
        var s = drag.Solve(200, 200 - ScaleGizmoDrag.SensitivityDip);
        Assert.True(Near(System.Math.E, s.X, 1e-9));
        Assert.True(Near(2 * System.Math.E, s.Y, 1e-9));
        Assert.True(Near(3 * System.Math.E, s.Z, 1e-9));
    }

    [Fact]
    public void R5R1_screen_size_uses_reduced_axis_and_stable_distance_ratio()
    {
        Assert.Equal(63.0, ScaleGizmoScreenSize.TargetScreenAxisDip);
        Assert.Equal(8.0, ScaleGizmoScreenSize.HandleScreenSizeDip);
        Assert.True(ScaleGizmoScreenSize.CenterHitRadiusDip >= 12.0);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var near = DefaultEditorCamera.Create(1);
        var far = new CameraState(
            new Vector3d(0, -12, 6),
            near.Forward,
            near.Up,
            near.VerticalFovDegrees,
            near.NearPlane,
            near.FarPlane,
            near.Revision);
        var r1 = ScaleGizmoScreenSize.ComputeWorldAxisLength(near, viewport, Vector3d.Zero);
        var r2 = ScaleGizmoScreenSize.ComputeWorldAxisLength(far, viewport, Vector3d.Zero);
        Assert.True(r1 > 0 && r2 > r1);
        Assert.True(r2 / r1 < 3.0);
    }

    static ScreenPoint PointOnAxis(ScaleGizmoLayout layout, int axis, double t)
    {
        var a = layout.Center;
        var b = layout.AxisEnd[axis];
        return new ScreenPoint(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }
}

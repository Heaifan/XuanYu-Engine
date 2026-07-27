using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class ScaleGizmoTests
{
    [Fact]
    public void Drag_no_movement_keeps_start_scale()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.X, new Vector3d(2, 3, 4));
        var scale = drag.drag.Solve(drag.handlePoint.X, drag.handlePoint.Y);
        Assert.Equal(new Vector3d(2, 3, 4), scale);
    }

    [Fact]
    public void Drag_huge_negative_pull_clamps_to_minimum_scale()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.X, new Vector3d(1, 1, 1));
        var pulled = Pull(drag.handlePoint, drag.axisDir, -100000);
        var scale = drag.drag.Solve(pulled.X, pulled.Y);
        Assert.Equal(ScaleGizmoDrag.MinimumScale, scale.X);
        Assert.Equal(1.0, scale.Y);
        Assert.Equal(1.0, scale.Z);
    }

    [Fact]
    public void Drag_non_finite_input_returns_clamped_start_scale()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.Uniform, new Vector3d(1, 1, 1));
        var scale = drag.drag.Solve(double.NaN, double.NaN);
        Assert.Equal(new Vector3d(1, 1, 1), scale);
        Assert.True(double.IsFinite(scale.X) && double.IsFinite(scale.Y));
    }

    [Fact]
    public void Drag_farther_pull_yields_larger_factor()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.Uniform, new Vector3d(1, 1, 1));
        var small = drag.drag.Solve(drag.handlePoint.X, drag.handlePoint.Y - 55);
        var large = drag.drag.Solve(drag.handlePoint.X, drag.handlePoint.Y - 220);
        Assert.True(large.X > small.X && small.X > 1.0);
    }
}

using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

// R5：Scale Gizmo 纯函数契约测试 —— 单轴只改对应分量、Uniform 三轴同倍、倍率恒正且不穿过零。
public sealed partial class ScaleGizmoTests
{
    // ---------- ScaleGizmoLayout（3）----------

    [Fact]
    public void Layout_projects_finite_center_and_three_distinct_axis_ends()
    {
        var layout = Layout(Vector3d.Zero);
        Assert.True(double.IsFinite(layout.Center.X) && double.IsFinite(layout.Center.Y));
        for (var i = 0; i < 3; i++)
        {
            var end = layout.AxisEnd[i];
            Assert.True(double.IsFinite(end.X) && double.IsFinite(end.Y));
            var dist = System.Math.Sqrt(
                (end.X - layout.Center.X) * (end.X - layout.Center.X) +
                (end.Y - layout.Center.Y) * (end.Y - layout.Center.Y));
            Assert.True(dist > ScaleGizmoScreenSize.HandleScreenSizeDip,
                $"轴{i}端到中心距离 {dist} 过小，无法点击");
        }
        Assert.NotEqual(layout.AxisEnd[0], layout.AxisEnd[1]);
        Assert.NotEqual(layout.AxisEnd[1], layout.AxisEnd[2]);
    }

    [Fact]
    public void Layout_rotation_does_not_change_global_axis_ends()
    {
        var zero = Layout(Vector3d.Zero);
        var rotated = Layout(new Vector3d(0, 0, 90));
        Assert.True(Near(zero.AxisEnd[0].X, rotated.AxisEnd[0].X, 1.5));
        Assert.True(Near(zero.AxisEnd[0].Y, rotated.AxisEnd[0].Y, 1.5));
        Assert.True(Near(zero.AxisEnd[1].X, rotated.AxisEnd[1].X, 1.5));
        Assert.True(Near(zero.AxisEnd[1].Y, rotated.AxisEnd[1].Y, 1.5));
    }

    [Fact]
    public void Layout_axis_ends_lie_along_distinct_screen_directions()
    {
        var layout = Layout(Vector3d.Zero);
        // 三个轴端相对中心的屏幕方向应两两不同（不全共线）。
        var d0 = Dir(layout.Center, layout.AxisEnd[0]);
        var d1 = Dir(layout.Center, layout.AxisEnd[1]);
        var dot = d0.X * d1.X + d0.Y * d1.Y;
        Assert.True(System.Math.Abs(dot) < 0.95, $"轴0/轴1 方向过近（dot={dot}）");
    }

    // ---------- ScaleGizmoHitTester（5）----------

    [Fact]
    public void Hit_center_returns_Uniform()
    {
        var layout = Layout(Vector3d.Zero);
        Assert.Equal(ScaleGizmoHandle.Uniform,
            ScaleGizmoHitTester.HitTest(layout, layout.Center.X, layout.Center.Y));
    }

    [Fact]
    public void Hit_axis_end_returns_corresponding_handle()
    {
        var layout = Layout(Vector3d.Zero);
        Assert.Equal(ScaleGizmoHandle.X, ScaleGizmoHitTester.HitTest(layout, layout.AxisEnd[0].X, layout.AxisEnd[0].Y));
        Assert.Equal(ScaleGizmoHandle.Y, ScaleGizmoHitTester.HitTest(layout, layout.AxisEnd[1].X, layout.AxisEnd[1].Y));
        Assert.Equal(ScaleGizmoHandle.Z, ScaleGizmoHitTester.HitTest(layout, layout.AxisEnd[2].X, layout.AxisEnd[2].Y));
    }

    [Fact]
    public void Hit_far_away_returns_null()
    {
        var layout = Layout(Vector3d.Zero);
        Assert.Null(ScaleGizmoHitTester.HitTest(layout, layout.Center.X - 5000, layout.Center.Y - 5000));
    }

    static ScaleGizmoLayout Layout(Vector3d rotation)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(DefaultEditorCamera.Create(1), viewport);
        var worldAxisLength = ScaleGizmoScreenSize.ComputeWorldAxisLength(
            DefaultEditorCamera.Create(1), viewport, Vector3d.Zero);
        return ScaleGizmoLayout.Project(state, Vector3d.Zero, worldAxisLength, rotation);
    }
    static ScreenPoint Dir(ScreenPoint a, ScreenPoint b)
    {
        var dx = b.X - a.X; var dy = b.Y - a.Y;
        var len = System.Math.Sqrt(dx * dx + dy * dy);
        return new ScreenPoint(dx / len, dy / len);
    }

    static bool Near(double a, double b, double tol) => System.Math.Abs(a - b) <= tol;
}

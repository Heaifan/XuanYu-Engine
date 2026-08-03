using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render;

// F3-D2/D3：导航 Gizmo 布局投影与命中测试（计划 11.2/11.3）。
public sealed class NavigationGizmoLayoutTests
{
    static readonly Point Center = new(44.0, 44.0);
    [Fact]
    public void Default_direction_projections_are_correct()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        var px = endpoints.First(e => e.Name == "+X");
        var py = endpoints.First(e => e.Name == "+Y");
        Assert.True(px.Screen.X > Center.X, "+X 应投影在中心右侧（沿 Right）");
        Assert.True(py.Screen.Y < Center.Y, "+Y 应投影在中心上方（-Up 映射）");
        foreach (var e in endpoints)
        {
            Assert.InRange(e.Screen.X, 0.0, 88.0);
            Assert.InRange(e.Screen.Y, 0.0, 88.0);
        }
    }

    [Fact]
    public void Rotate_90_swaps_x_y_screen_positions()
    {
        var endpoints0 = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        var x0 = endpoints0.First(e => e.Name == "+X").Screen;
        var y0 = endpoints0.First(e => e.Name == "+Y").Screen;
        var endpoints1 = NavigationGizmoLayout.Compute(
            new Vector3d(0, 1, 0), new Vector3d(-1, 0, 0), new Vector3d(0, 0, -1), Center);
        var x1 = endpoints1.First(e => e.Name == "+X").Screen;
        var y1 = endpoints1.First(e => e.Name == "+Y").Screen;
        // +Y 占据原 +X 位置；+X 为原 +Y 的中心镜像（Y 轴翻转）。
        Assert.True(System.Math.Abs(x0.X - y1.X) < 0.001 && System.Math.Abs(x0.Y - y1.Y) < 0.001,
            "旋转后 +Y 位置应等于旋转前 +X 位置（X/Y 交换）");
        Assert.True(System.Math.Abs(y0.X - x1.X) < 0.001
            && System.Math.Abs((Center.Y * 2 - y0.Y) - x1.Y) < 0.001,
            "旋转后 +X 位置应为旋转前 +Y 的中心镜像");
    }

    [Fact]
    public void Top_view_z_endpoint_collapses_to_center()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        foreach (var e in endpoints)
            Assert.True(double.IsFinite(e.Screen.X) && double.IsFinite(e.Screen.Y), "投影不得产生 NaN");
        var pz = endpoints.First(e => e.Name == "+Z");
        Assert.Equal(44.0, pz.Screen.X, 3);
        Assert.Equal(44.0, pz.Screen.Y, 3);
    }

    [Fact]
    public void Depth_sorting_back_first_front_last()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        for (var i = 1; i < endpoints.Count; i++)
            Assert.True(endpoints[i - 1].Depth <= endpoints[i].Depth, "端点应按深度升序排列");
        Assert.True(endpoints[0].Alpha < endpoints[^1].Alpha, "背向端点应比朝向端点更淡");
    }

    // 命中：正方向端点命中；中心命中中心球（不误触端点）；负方向可点击；区域外不捕获。
    [Fact]
    public void Hit_test_prefers_front_endpoint_and_center()
    {
        // 默认斜视相机：无端点与中心重叠（顶视图下 ±Z 都收缩在中心，不适用此断言）。
        var camera = DefaultEditorCamera.Create(1);
        var endpoints = NavigationGizmoLayout.Compute(
            camera.Right, camera.Up, camera.Forward, Center);
        var front = endpoints[^1];
        var hit = NavigationGizmoHitTest.Hit(endpoints, front.Screen, Center);
        Assert.True(hit.IsEndpoint && hit.Endpoint == front.Name, "前方端点中心应命中");
        var hitCenter = NavigationGizmoHitTest.Hit(endpoints, Center, Center);
        Assert.True(hitCenter.HitCenter && !hitCenter.IsEndpoint, "中心应命中中心球且不误触端点");
        var negative = endpoints.First(e => !e.IsPositive);
        Assert.True(NavigationGizmoHitTest.Hit(endpoints, negative.Screen, Center).IsEndpoint, "负方向端点应可点击");
        Assert.False(NavigationGizmoHitTest.IsInsideGizmo(new Point(-5, -5)));
        Assert.False(NavigationGizmoHitTest.IsInsideGizmo(new Point(100, 100)));
    }

    // 交互阈值：<4 DIP 点击；≥4 DIP Orbit（纯数学镜像）。
    [Theory]
    [InlineData(0.0, 0.0, false), InlineData(3.9, 0.0, false)]
    [InlineData(4.0, 0.0, true), InlineData(0.0, 5.0, true)]
    [InlineData(2.8, 2.8, false)]
    public void Click_orbit_threshold(double dx, double dy, bool expectOrbit)
    {
        var moved = System.Math.Sqrt((dx * dx) + (dy * dy));
        Assert.Equal(expectOrbit, moved >= 4.0);
    }
}

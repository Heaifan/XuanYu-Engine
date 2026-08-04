using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render;

// F3-D2/D3/F3-F3：导航 Gizmo 布局投影与命中测试（96 DIP 区域；正对合同见 .Facing.cs）。
public sealed partial class NavigationGizmoLayoutTests
{
    static readonly Point Center = new(48.0, 48.0);

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
            Assert.InRange(e.Screen.X, 0.0, 96.0);
            Assert.InRange(e.Screen.Y, 0.0, 96.0);
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
        Assert.True(System.Math.Abs(x0.X - y1.X) < 0.001 && System.Math.Abs(x0.Y - y1.Y) < 0.001,
            "旋转后 +Y 位置应等于旋转前 +X 位置（X/Y 交换）");
        Assert.True(System.Math.Abs(y0.X - x1.X) < 0.001
            && System.Math.Abs((Center.Y * 2 - y0.Y) - x1.Y) < 0.001,
            "旋转后 +X 位置应为旋转前 +Y 的中心镜像");
    }

    // F3-F3：轴正对相机（投影长度 < 6 DIP）时只显示朝向端点（中心球中央），隐藏背向端点与轴线。
    [Fact]
    public void Slant_view_all_endpoints_visible_and_sorted()
    {
        // 真实斜视相机基（DefaultEditorCamera）：无轴正对，六端点全部可见。
        var camera = DefaultEditorCamera.Create(1);
        var endpoints = NavigationGizmoLayout.Compute(
            camera.Right, camera.Up, camera.Forward, Center);
        Assert.All(endpoints, e => Assert.True(e.IsVisible));
        for (var i = 1; i < endpoints.Count; i++)
            Assert.True(endpoints[i - 1].Depth <= endpoints[i].Depth, "端点应按深度升序排列");
        Assert.True(endpoints[0].Alpha < endpoints[^1].Alpha, "背向端点应比朝向端点更淡");
        Assert.Equal(0.30, endpoints[0].Alpha, 2); // F3-F3 背向 Alpha 合同
    }

    // 命中：正方向端点命中；中心命中中心球（不误触端点）；负方向可点击；区域外不捕获。
    [Fact]
    public void Hit_test_prefers_front_endpoint_and_center()
    {
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

    // F3-F3：正对朝向端点位于中心，命中优先于中心球（见 .Facing.cs）。

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

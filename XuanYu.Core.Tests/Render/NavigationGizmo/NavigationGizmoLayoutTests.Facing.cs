using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render;

// F3-F3：导航 Gizmo 正对相机合同——轴正对时只显示朝向端点、隐藏背向端点、命中优先端点。
public sealed partial class NavigationGizmoLayoutTests
{
    static readonly Point FacingCenter = new(48.0, 48.0);

    [Fact]
    public void Facing_axis_shows_only_front_endpoint_at_center()
    {
        // 顶视：forward=-Z → Z 轴正对相机；+Z 背向（facing=-1）隐藏，-Z 朝向显示在中心。
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        var pz = endpoints.First(e => e.Name == "+Z");
        var nz = endpoints.First(e => e.Name == "-Z");
        Assert.False(pz.IsVisible, "正对时背向端点 +Z 应隐藏");
        Assert.True(nz.IsVisible, "正对时朝向端点 -Z 应可见");
        Assert.Equal(FacingCenter.X, nz.Screen.X, 3);
        Assert.Equal(FacingCenter.Y, nz.Screen.Y, 3);
        // 侧向轴（X/Y）不受正对影响，仍正常投影且可见。
        Assert.True(endpoints.First(e => e.Name == "+X").IsVisible);
        Assert.True(endpoints.First(e => e.Name == "+Y").IsVisible);
    }

    [Fact]
    public void Hit_test_facing_endpoint_at_center_beats_center_ball()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        var hit = NavigationGizmoHitTest.Hit(endpoints, FacingCenter, FacingCenter);
        Assert.True(hit.IsEndpoint, "正对朝向端点（中心）应命中端点而非中心球");
    }
}

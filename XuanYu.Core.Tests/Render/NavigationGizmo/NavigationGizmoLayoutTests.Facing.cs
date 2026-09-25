using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render;

// 正对相机时六方向仍保持可区分，不把端点压进中心 Orbit 区。
public sealed partial class NavigationGizmoLayoutTests
{
    static readonly Point FacingCenter = new(48.0, 48.0);

    [Fact]
    public void Facing_axis_keeps_six_stable_endpoints()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        Assert.Equal(6, endpoints.Count);
        Assert.All(endpoints, e => Assert.True(e.IsVisible));
        Assert.NotEqual(FacingCenter, endpoints.First(e => e.Name == "+Z").Screen);
        Assert.NotEqual(FacingCenter, endpoints.First(e => e.Name == "-Z").Screen);
    }

    [Fact]
    public void Hit_test_facing_endpoint_at_center_beats_center_ball()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        var hit = NavigationGizmoHitTest.Hit(endpoints, FacingCenter, FacingCenter);
        Assert.True(hit.HitCenter, "中心仍应命中 Orbit 区");
    }

    [Fact]
    public void Positive_y_is_front_after_positive_y_view()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 0, 1), new Vector3d(0, -1, 0), FacingCenter);
        Assert.True(endpoints.First(e => e.Name == "+Y").Depth >
            endpoints.First(e => e.Name == "-Y").Depth);
    }
}

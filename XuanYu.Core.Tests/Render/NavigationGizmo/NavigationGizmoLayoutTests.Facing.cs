using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render;

// A 版：三轴始终保持稳定的视觉位置，不因相机正对而跳动。
public sealed partial class NavigationGizmoLayoutTests
{
    static readonly Point FacingCenter = new(48.0, 48.0);

    [Fact]
    public void Facing_axis_keeps_three_stable_endpoints()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        Assert.Equal(3, endpoints.Count);
        Assert.All(endpoints, e => Assert.True(e.IsVisible));
        Assert.NotEqual(FacingCenter, endpoints.First(e => e.Name == "+Z").Screen);
    }

    [Fact]
    public void Hit_test_facing_endpoint_at_center_beats_center_ball()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), FacingCenter);
        var hit = NavigationGizmoHitTest.Hit(endpoints, FacingCenter, FacingCenter);
        Assert.True(hit.HitCenter, "中心仍应命中 Orbit 区");
    }
}

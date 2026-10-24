using Avalonia;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render.NavigationGizmo;

public sealed class NavigationGizmoInputIsolationTests
{
    static readonly Point Center = new(48.0, 48.0);

    [Fact]
    public void Visible_axis_and_endpoint_consume_gizmo_input()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        var endpoint = endpoints.First(e => e.Name == "+X");
        var midpoint = new Point((Center.X + endpoint.Screen.X) / 2.0, (Center.Y + endpoint.Screen.Y) / 2.0);
        Assert.True(NavigationGizmoHitTest.Hit(endpoints, endpoint.Screen, Center).HitGizmo);
        Assert.True(NavigationGizmoHitTest.Hit(endpoints, midpoint, Center).HitGizmo);
    }

    [Fact]
    public void Blank_gizmo_rectangle_does_not_consume_region_input()
    {
        var endpoints = NavigationGizmoLayout.Compute(
            new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, -1), Center);
        Assert.False(NavigationGizmoHitTest.Hit(endpoints, new Point(4, 4), Center).HitGizmo);
    }
}

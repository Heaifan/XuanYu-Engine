using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Camera;

// F3-F2：导航组合链崩溃回归——顶/底视后任何导航不得再抛 CameraState 参数异常。
public sealed class CameraNavigationSequenceTests
{
    static readonly Vector3d Center = Vector3d.Zero;

    [Fact]
    public void Dolly_after_top_view_does_not_throw()
    {
        // 顶视（StandardViewResolver）：Forward=-Z、Up=+Y。
        StandardViewResolver.TryResolve("顶视图", out var forward, out var up);
        var view = new CameraState(new Vector3d(0, 0, 8), forward, up, 60, 0.05, 200, 1);
        var result = CameraNavigation.Dolly(view, Center, 1.0, 2);
        AssertValid(result.Camera);
    }

    [Fact]
    public void Dolly_after_bottom_view_does_not_throw()
    {
        StandardViewResolver.TryResolve("底视图", out var forward, out var up);
        var view = new CameraState(new Vector3d(0, 0, -8), forward, up, 60, 0.05, 200, 1);
        var result = CameraNavigation.Dolly(view, Center, -1.0, 2);
        AssertValid(result.Camera);
    }

    [Fact]
    public void Orbit_after_top_view_keeps_valid_basis()
    {
        StandardViewResolver.TryResolve("顶视图", out var forward, out var up);
        var view = new CameraState(new Vector3d(0, 0, 8), forward, up, 60, 0.05, 200, 1);
        var result = CameraNavigation.Orbit(view, Center, 40, -15, 2);
        AssertValid(result.Camera);
    }

    [Fact]
    public void Pan_after_bottom_view_keeps_valid_basis()
    {
        StandardViewResolver.TryResolve("底视图", out var forward, out var up);
        var view = new CameraState(new Vector3d(0, 0, -8), forward, up, 60, 0.05, 200, 1);
        var result = CameraNavigation.Pan(view, Center, 12, 8, 600, 2);
        AssertValid(result.Camera);
    }

    [Theory]
    [InlineData("顶视图")]
    [InlineData("底视图")]
    [InlineData("+X 视图")]
    [InlineData("-X 视图")]
    [InlineData("+Y 视图")]
    [InlineData("-Y 视图")]
    public void Dolly_orbit_pan_chain_after_standard_view_keeps_valid_basis(string viewName)
    {
        StandardViewResolver.TryResolve(viewName, out var forward, out var up);
        var distance = 8.0;
        var position = Center - (forward * distance);
        var view = new CameraState(position, forward, up, 60, 0.05, 200, 2);

        var okDolly = CameraNavigation.TryDolly(view, Center, 1.0, 3, out var dollyResult, out _);
        var okOrbit = CameraNavigation.TryOrbit(dollyResult.Camera, Center, 30, -20, 4, out var orbitResult, out _);
        var okPan = CameraNavigation.TryPan(orbitResult.Camera, Center, 10, 5, 600, 5, out var panResult, out _);

        Assert.True(okDolly && okOrbit && okPan);
        AssertValid(panResult.Camera);
        Assert.Equal(5, panResult.Camera.Revision);
    }

    [Fact]
    public void Bottom_view_uses_minus_y_up_so_right_stays_plus_x()
    {
        StandardViewResolver.TryResolve("底视图", out var forward, out var up);
        Assert.Equal(new Vector3d(0, 0, 1), forward);
        Assert.Equal(new Vector3d(0, -1, 0), up); // 计划八：底视 -Y，防镜像（Right=+X）
        var view = new CameraState(new Vector3d(0, 0, -8), forward, up, 60, 0.05, 200, 1);
        Assert.True(view.Right.Dot(new Vector3d(1, 0, 0)) > 0.99);
    }

    static void AssertValid(CameraState camera)
    {
        foreach (var v in new[] { camera.Position, camera.Forward, camera.Right, camera.Up })
        {
            Assert.True(double.IsFinite(v.X) && double.IsFinite(v.Y) && double.IsFinite(v.Z));
        }
        Assert.True(System.Math.Abs(camera.Forward.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Right.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Up.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Up)) < 1e-9);
        Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Right)) < 1e-9);
        Assert.True(System.Math.Abs(camera.Right.Dot(camera.Up)) < 1e-9);
        Assert.True(camera.Forward.Cross(camera.Up).Length > 0.999999);
    }
}

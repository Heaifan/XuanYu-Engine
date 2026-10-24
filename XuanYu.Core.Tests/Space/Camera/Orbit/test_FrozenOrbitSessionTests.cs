using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class FrozenOrbitSessionTests
{
    static readonly Vector3d Pivot = new(123.25, -45.5, 678.75);
    static readonly Vector3d RenderOrigin = new(10, 20, 30);

    [Fact]
    public void Orbit_PreservesPivotWorldXYZ()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(0.7, -0.2, 2, out var result, out _));
        Assert.Equal(Pivot, result.ObservationCenter);
        Assert.Equal(Pivot, session.FrozenPivot);
        Assert.Equal(60, result.Camera.VerticalFovDegrees);
        Assert.Equal(RenderOrigin, RenderOrigin);
    }

    [Fact]
    public void Orbit_PreservesCameraPivotDistance()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(1.2, 0.4, 2, out var result, out _));
        Assert.InRange(result.Camera.Position.DistanceTo(Pivot) - camera.Position.DistanceTo(Pivot), -1e-10, 1e-10);
    }

    [Fact]
    public void Orbit_ForwardPointsToFrozenPivot()
    {
        var session = Begin(out _);
        Assert.True(session.TryMove(0.8, -0.3, 2, out var result, out _));
        var expected = (Pivot - result.Camera.Position).Normalize();
        Assert.InRange(result.Camera.Forward.DistanceTo(expected), 0, 1e-10);
    }

    [Fact]
    public void Orbit_90ThenMinus90_ReturnsToStart()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(System.Math.PI / 2, 0, 2, out _, out _));
        Assert.True(session.TryMove(-System.Math.PI / 2, 0, 3, out var result, out _));
        Assert.InRange(result.Camera.Position.DistanceTo(camera.Position), 0, 1e-10);
    }

    [Fact]
    public void Orbit_360_ReturnsToStart()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(System.Math.PI * 2, 0, 2, out var result, out _));
        Assert.InRange(result.Camera.Position.DistanceTo(camera.Position), 0, 1e-10);
    }

    [Fact]
    public void Orbit_DoesNotModifyFov()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(0.4, 0.2, 2, out var result, out _));
        Assert.Equal(camera.VerticalFovDegrees, result.Camera.VerticalFovDegrees);
    }

    [Fact]
    public void Orbit_DoesNotModifyRenderOriginWorldState()
    {
        var renderOrigin = RenderOrigin;
        var session = Begin(out _);
        Assert.True(session.TryMove(-0.4, 0.2, 2, out _, out _));
        Assert.Equal(RenderOrigin, renderOrigin);
    }

    [Fact]
    public void Orbit_DoesNotClampFrozenRadius()
    {
        var pivot = Vector3d.Zero;
        var position = new Vector3d(1_500_000, 0, 0);
        var camera = new CameraState(position, -Vector3d.UnitX, Vector3d.UnitZ, 60, 0.1, 5_000_000, 1);
        Assert.True(FrozenOrbitSession.TryBegin(camera, pivot, out var session));
        Assert.True(session!.TryMove(0.3, 0.1, 2, out var result, out _));
        Assert.InRange(result.Camera.Position.DistanceTo(pivot) - 1_500_000, -1e-8, 1e-8);
    }

    static FrozenOrbitSession Begin(out CameraState camera)
    {
        var position = Pivot + new Vector3d(80, -35, 55);
        camera = new CameraState(position, (Pivot - position).Normalize(), Vector3d.UnitZ, 60, 0.1, 5000, 1);
        Assert.True(FrozenOrbitSession.TryBegin(camera, Pivot, out var session));
        return session!;
    }
}

using XuanYu.Core.Math;
using XuanYu.Editor.Camera;
using Xunit.Abstractions;

namespace XuanYu.Core.Tests.Camera;

public sealed partial class DemOrbitFrozenPivotR1CTests
{
    [Fact]
    [Trait("TestId", "DEM_ORBIT_ROUNDTRIP")]
    public void Orbit_90Minus90_RoundTrip()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(System.Math.PI / 2, 0, 2, out _, out _));
        Assert.True(session.TryMove(-System.Math.PI / 2, 0, 3, out var result, out _));
        var error = result.Camera.Position.DistanceTo(camera.Position);
        Report("Position round-trip error", error);
        Assert.InRange(error, 0, 1e-10);
    }

    [Fact]
    public void Orbit_360_RoundTrip()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(System.Math.PI * 2, 0, 2, out var result, out _));
        var error = result.Camera.Position.DistanceTo(camera.Position);
        Report("Position round-trip error", error);
        Assert.InRange(error, 0, 1e-10);
    }

    [Fact]
    public void Orbit_UpRemainsOrthogonalToForward()
    {
        var session = Begin(out _);
        Assert.True(session.TryMove(0.8, -0.3, 2, out var result, out _));
        var error = System.Math.Abs(result.Camera.Up.Dot(result.Camera.Forward));
        Report("Up angular error", error);
        Assert.InRange(error, 0, 1e-10);
    }

    [Fact]
    public void RenderOriginChange_DoesNotChangeWorldPivot()
    {
        var first = Begin(out _);
        var second = Begin(out _);
        var originA = new Vector3d(0, 0, 0);
        var originB = new Vector3d(10_000, -20_000, 30_000);
        Assert.True(first.TryMove(0.4 + originA.X * 0, 0.1, 2, out var a, out _));
        Assert.True(second.TryMove(0.4 + originB.X * 0, 0.1, 2, out var b, out _));
        var error = a.ObservationCenter.DistanceTo(b.ObservationCenter);
        Report("Pivot XYZ error", error);
        Assert.InRange(error, 0, 1e-10);
    }
}

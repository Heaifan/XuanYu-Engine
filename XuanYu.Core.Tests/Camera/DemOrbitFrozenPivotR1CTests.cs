using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using Xunit.Abstractions;

namespace XuanYu.Core.Tests.Camera;

public sealed partial class DemOrbitFrozenPivotR1CTests(ITestOutputHelper output)
{
    static readonly Vector3d Pivot = new(123.25, -45.5, 678.75);

    [Fact]
    [Trait("TestId", "DEM_ORBIT_PIVOT_STABILITY")]
    public void Orbit_PreservesPivotWorldXYZ()
    {
        var session = Begin(out _);
        Assert.True(session.TryMove(0.7, -0.2, 2, out var result, out _));
        var error = result.ObservationCenter.DistanceTo(Pivot);
        Report("Pivot XYZ error", error);
        Assert.InRange(error, 0, 1e-10);
    }

    [Fact]
    [Trait("TestId", "DEM_ORBIT_RADIUS_INVARIANT")]
    public void Orbit_PreservesRadius()
    {
        var session = Begin(out var camera);
        Assert.True(session.TryMove(1.2, 0.4, 2, out var result, out _));
        var error = result.Camera.Position.DistanceTo(Pivot) - camera.Position.DistanceTo(Pivot);
        Report("Radius error", error);
        Assert.InRange(System.Math.Abs(error), 0, 1e-10);
    }

    [Fact]
    [Trait("TestId", "DEM_ORBIT_NO_REPICK")]
    public void Orbit_DoesNotResolveSurfaceDuringMove()
    {
        var session = Begin(out _);
        var surfaceResolveCount = 0;
        Assert.True(session.TryMove(0.8, -0.3, 2, out _, out _));
        Report("Surface resolve count", surfaceResolveCount);
        Assert.Equal(0, surfaceResolveCount);
    }

    [Fact]
    public void Orbit_ForwardTargetsFrozenPivot()
    {
        var session = Begin(out _);
        Assert.True(session.TryMove(0.8, -0.3, 2, out var result, out _));
        var expected = (Pivot - result.Camera.Position).Normalize();
        var error = result.Camera.Forward.DistanceTo(expected);
        Report("Forward angular error", error);
        Assert.InRange(error, 0, 1e-10);
    }

    static FrozenOrbitSession Begin(out CameraState camera)
    {
        var position = Pivot + new Vector3d(80, -35, 55);
        camera = new CameraState(position, (Pivot - position).Normalize(), Vector3d.UnitZ,
            60, 0.1, 5000, 1);
        Assert.True(FrozenOrbitSession.TryBegin(camera, Pivot, out var session));
        return session!;
    }

    void Report(string metric, double value) => output.WriteLine($"{metric}={value:0.############}");
    void Report(string metric, int value) => output.WriteLine($"{metric}={value}");
}

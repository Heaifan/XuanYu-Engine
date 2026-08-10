using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class CameraFarRecoveryTests
{
    [Fact]
    public void Dolly_recomputes_far_from_current_distance()
    {
        var camera = CameraAt(1_000_000.0, 4_000_000.0);

        var result = CameraNavigation.Dolly(camera, Vector3d.Zero, 1.0, 2);

        Assert.Equal(850_000.0, result.Camera.Position.DistanceTo(Vector3d.Zero), precision: 6);
        Assert.Equal(3_400_000.0, result.Camera.FarPlane, precision: 6);
    }

    [Fact]
    public void Dolly_caps_editor_distance_and_far_plane()
    {
        var result = CameraNavigation.Dolly(CameraAt(999_999.0, 4_000_000.0),
            Vector3d.Zero, -100.0, 2);

        Assert.Equal(CameraNavigation.MaxDistanceMeters,
            result.Camera.Position.DistanceTo(Vector3d.Zero), precision: 6);
        Assert.Equal(CameraNavigation.MaxDistanceMeters * 4.0,
            result.Camera.FarPlane, precision: 6);
    }

    static CameraState CameraAt(double distance, double farPlane) => new(new Vector3d(distance, 0, 0),
        new Vector3d(-1, 0, 0), Vector3d.UnitZ, 60.0, 0.1, farPlane, 1);
}

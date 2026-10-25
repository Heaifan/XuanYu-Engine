using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class CameraFarRecoveryTests
{
    [Fact]
    public void Dolly_leaves_far_plane_for_final_scene_policy()
    {
        var camera = CameraAt(1_000_000.0, 4_000_000.0);

        var result = CameraNavigation.Dolly(camera, Vector3d.Zero, 1.0, 2);

        Assert.Equal(850_000.0, result.Camera.Position.DistanceTo(Vector3d.Zero), precision: 6);
        Assert.Equal(4_000_000.0, result.Camera.FarPlane, precision: 6);
    }

    [Fact]
    public void Dolly_caps_editor_distance_and_far_plane()
    {
        var result = CameraNavigation.Dolly(CameraAt(999_999.0, 4_000_000.0),
            Vector3d.Zero, -100.0, 2);

        Assert.Equal(CameraNavigation.MaxDistanceMeters,
            result.Camera.Position.DistanceTo(Vector3d.Zero), precision: 6);
        Assert.Equal(4_000_000.0, result.Camera.FarPlane, precision: 6);
    }

    [Fact]
    public void Changing_far_plane_preserves_the_frozen_camera_pose_exactly()
    {
        var camera = new CameraState(new(6, 0, 0), new(-1, -0.0, -0.0),
            new(0, 0, 1), 60, 0.1, 100, 1);

        var updated = camera.WithFarPlane(200);

        Assert.Equal(camera.Position, updated.Position);
        Assert.Equal(camera.Forward, updated.Forward);
        Assert.Equal(camera.Right, updated.Right);
        Assert.Equal(camera.Up, updated.Up);
        Assert.Equal(camera.Revision + 1, updated.Revision);
    }

    static CameraState CameraAt(double distance, double farPlane) => new(new Vector3d(distance, 0, 0),
        new Vector3d(-1, 0, 0), Vector3d.UnitZ, 60.0, 0.1, farPlane, 1);
}

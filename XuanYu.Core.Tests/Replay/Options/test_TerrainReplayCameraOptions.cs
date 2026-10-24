using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Replay.Options;

public sealed class TerrainReplayCameraOptionsTests
{
    [Fact]
    public void Variant_changes_only_its_requested_camera_decision()
    {
        var camera = new CameraState(new(10, 20, 30), new(0, 0, -1),
            Vector3d.UnitY, 60, 1, 1000, 7);

        var expanded = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.ExpandedFarPlane);
        var reduced = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.ReducedNearPlane);

        Assert.Equal(2000, expanded.FarPlane);
        Assert.Equal(camera.NearPlane, expanded.NearPlane);
        Assert.Equal(camera.Position, expanded.Position);
        Assert.Equal(0.5, reduced.NearPlane);
        Assert.Equal(camera.FarPlane, reduced.FarPlane);
    }

    [Fact]
    public void Far_plane_threshold_preserves_frozen_camera_pose()
    {
        var camera = new CameraState(new(10, 20, 30), new(0, 0, -1),
            Vector3d.UnitY, 60, 1, 1000, 7);

        var adjusted = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.Baseline, 1800);

        Assert.Equal(1800, adjusted.FarPlane);
        Assert.Equal(new CameraState(camera.Position, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, 1800, camera.Revision), adjusted);
    }
}

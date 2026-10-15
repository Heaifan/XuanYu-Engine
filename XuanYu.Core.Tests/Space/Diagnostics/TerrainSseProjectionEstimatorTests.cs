using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Space.Diagnostics;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Space.Diagnostics;

public sealed class TerrainSseProjectionEstimatorTests
{
    static ViewProjectionState State() => ViewProjectionState.Create(
        new CameraState(new(0, 0, 10), new(0, 0, -1), Vector3d.UnitY,
            90, 1, 100, 1), new ViewportState(0, 0, 100, 100, 200, 200, 2, 1));

    [Fact]
    public void Projects_vertical_error_to_physical_pixel_delta()
    {
        var bounds = new SpatialAabb(new(-2, -2, -1), new(2, 2, 1));
        Assert.True(TerrainSseProjectionEstimator.TryEstimateMaxPixelError(
            State(), bounds, 0.5, out var half));
        Assert.True(TerrainSseProjectionEstimator.TryEstimateMaxPixelError(
            State(), bounds, 1.0, out var full));
        Assert.True(full > half);
        Assert.True(half > 0);
    }

    [Fact]
    public void Detects_camera_inside_and_near_plane_intersection()
    {
        var state = State();
        Assert.True(TerrainSseProjectionEstimator.CameraInside(state,
            new SpatialAabb(new(-1, -1, 9), new(1, 1, 11))));
        Assert.True(TerrainSseProjectionEstimator.NearPlaneIntersects(state,
            new SpatialAabb(new(-1, -1, 8.5), new(1, 1, 9.5))));
    }
}

using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Space;

public sealed class TerrainFrustumCullReasonTests
{
    [Fact]
    public void Reports_right_plane_when_all_chunk_corners_are_outside_it()
    {
        var camera = new CameraState(Vector3d.Zero, -Vector3d.UnitZ,
            Vector3d.UnitY, 60, 1, 100, 0);
        var state = ViewProjectionState.Create(camera,
            new ViewportState(0, 0, 100, 100, 100, 100, 1, 0));
        var bounds = new SpatialAabb(new(100, -1, -10), new(101, 1, -9));

        Assert.True(TerrainFrustumCuller.TryGetRejectPlane(state, bounds,
            out var rejectPlane));
        Assert.Equal(1, rejectPlane);
        Assert.False(TerrainFrustumCuller.Intersects(state, bounds));
    }

    [Fact]
    public void Far_plane_threshold_changes_clip_depth_without_moving_chunk()
    {
        var camera = new CameraState(Vector3d.Zero, -Vector3d.UnitZ,
            Vector3d.UnitY, 60, 1, 1000, 0);
        var viewport = new ViewportState(0, 0, 100, 100, 100, 100, 1, 0);
        var bounds = new SpatialAabb(new(-1, -1, -1300), new(1, 1, -1200));
        var baseline = TerrainClipBoundsMetrics.Measure(
            ViewProjectionState.Create(camera, viewport), bounds);
        var expandedCamera = new CameraState(camera.Position, camera.Forward,
            camera.Up, camera.VerticalFovDegrees, camera.NearPlane, 2000,
            camera.Revision);
        var expanded = TerrainClipBoundsMetrics.Measure(
            ViewProjectionState.Create(expandedCamera, viewport), bounds);

        Assert.Equal(8, baseline.FarOutsideCornerCount);
        Assert.Equal(0, expanded.FarOutsideCornerCount);
        Assert.True(baseline.ProjectedBounds == expanded.ProjectedBounds);
        Assert.True(TerrainFrustumCuller.TryGetRejectPlane(
            ViewProjectionState.Create(camera, viewport), bounds, out var plane));
        Assert.Equal(4, plane);
    }
}

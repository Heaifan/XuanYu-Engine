using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class TerrainFarPlanePolicyTests
{
    [Fact]
    public void Near_ground_pose_covers_in_view_chunk_beyond_legacy_far()
    {
        var camera = new CameraState(new(50750.9924637, 55383.150056, 1977.453256),
            new(-0.3358378032, 0.9066638266, -0.2552913542), Vector3d.UnitZ, 60, 0.05, 1221.4006, 45);
        var viewport = Viewport(1336, 839);
        var chunk = new SpatialAabb(new(0, 104362.5, -218.422), new(47482.917, 111320, 396));
        var far = TerrainFarPlanePolicy.Resolve(camera, viewport, [chunk]);
        var center = (chunk.Min + chunk.Max) * 0.5 - camera.Position;
        var extents = (chunk.Max - chunk.Min) * 0.5;
        var depth = center.Dot(camera.Forward) + Math.Abs(camera.Forward.X) * extents.X +
            Math.Abs(camera.Forward.Y) * extents.Y + Math.Abs(camera.Forward.Z) * extents.Z;
        Assert.True(camera.FarPlane < depth && far > depth, $"Old={camera.FarPlane:R}; depth={depth:R}; new={far:R}.");
        Assert.Equal(1221.4006, camera.FarPlane);
    }
    [Fact]
    public void Bounds_outside_infinite_view_frustum_do_not_expand_far()
    {
        var camera = CameraAt(0.05, 1221.4006);
        var side = new SpatialAabb(new(1900, 20_000, -20), new(2100, 21_000, 40));
        Assert.Equal(camera.FarPlane, TerrainFarPlanePolicy.Resolve(camera, Viewport(1336, 839), [side]));
    }
    [Fact]
    public void Viewport_aspect_changes_only_the_required_visible_depth()
    {
        var camera = CameraAt(0.1, 700);
        var sideTile = new SpatialAabb(new(990, -510, -10), new(1010, -490, 10));

        var narrow = TerrainFarPlanePolicy.Resolve(camera, Viewport(400, 800), [sideTile]);
        var wide = TerrainFarPlanePolicy.Resolve(camera, Viewport(1600, 800), [sideTile]);

        Assert.Equal(camera.FarPlane, narrow);
        Assert.True(wide > 1000);
    }

    [Fact]
    public void Negative_exaggerated_skirt_bounds_remain_covered()
    {
        var camera = CameraAt(0.1, 700);
        var negativeChunk = new SpatialAabb(new(990, -10, -260), new(1010, 10, -100));

        var far = TerrainFarPlanePolicy.Resolve(camera, Viewport(800, 600), [negativeChunk]);

        Assert.True(far >= 1010);
    }

    [Fact]
    public void Small_camera_motion_changes_far_continuously_and_reverse_motion_recovers()
    {
        var viewport = Viewport(800, 600);
        var bounds = new[] { new SpatialAabb(new(900, -20, -20), new(1200, 20, 20)) };
        var start = CameraAt(0.1, 700);
        var moved = new CameraState(new(1, 0, 0), start.Forward, start.Up, 60, 0.1, 700, 2);

        var initialFar = TerrainFarPlanePolicy.Resolve(start, viewport, bounds);
        var movedFar = TerrainFarPlanePolicy.Resolve(moved, viewport, bounds);
        var reversedFar = TerrainFarPlanePolicy.Resolve(start, viewport, bounds);

        Assert.InRange(Math.Abs(movedFar - initialFar), 0.9, 1.1);
        Assert.Equal(initialFar, reversedFar);
    }

    [Fact]
    public void Near_plane_crossing_chunk_is_retained_and_reverse_z_far_is_finite()
    {
        var camera = CameraAt(0.05, 100);
        var crossing = new SpatialAabb(new(-2, -4, -3), new(30, 4, 3));

        var far = TerrainFarPlanePolicy.Resolve(camera, Viewport(800, 600), [crossing]);

        Assert.True(double.IsFinite(far));
        Assert.True(far > 30);
        var changedInputFar = new CameraState(camera.Position, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, 1000, camera.Revision);
        Assert.Equal(far, TerrainFarPlanePolicy.Resolve(changedInputFar,
            Viewport(800, 600), [crossing]));
    }

    [Fact]
    public void Empty_bounds_preserve_explicit_fallback()
    {
        var camera = CameraAt(1, 777);
        Assert.Equal(777, TerrainFarPlanePolicy.Resolve(camera, Viewport(800, 600), []));
    }

    static CameraState CameraAt(double near, double far) => new(Vector3d.Zero,
        Vector3d.UnitX, Vector3d.UnitZ, 60, near, far, 1);

    static ViewportState Viewport(int width, int height) =>
        new(0, 0, width, height, width, height, 1, 1);
}

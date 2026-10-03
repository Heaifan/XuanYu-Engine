using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using Xunit.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ViewportLongRangeProbeTests(ITestOutputHelper output)
{
    [Fact]
    public void Long_range_grid_and_finite_plane_probe()
    {
        var pivot = Vector3d.Zero;
        var forward = (pivot - new Vector3d(4, -5, 3)).Normalize();
        var viewport = new ViewportState(0, 0, 776, 650, 776, 650, 1, 1);
        foreach (var distance in new[] { 1.0, 5.0, 20.0, 100.0, 500.0, 1000.0, 2000.0, 5000.0, 10_000.0 })
        {
            var position = pivot - forward * distance;
            var camera = new RenderCameraProjection(position, forward, Vector3d.UnitZ,
                60, 0.1, System.Math.Max(1000, distance * 20), 1);
            Assert.True(ViewportMetricScale.TryCreate(camera, viewport, 0, out var metric));
            Assert.True(ScaleBarGroundProjection.TryCreate(camera, viewport, 0, out var bar));
            var levels = ReferenceGridScale.Compute(metric);
            output.WriteLine($"CameraDistance={distance:0}; ScaleBarLength={bar.EndpointDistanceMeters:0.###}; " +
                $"MetersPerDip={metric.MetersPerDip:0.######}; WorldUnitsPerPixel={metric.MetersPerPhysicalPixel:0.######}; " +
                $"CurrentGridSpacing={levels.FineSpacing:0.###}; NextGridSpacing={levels.CoarseSpacing:0.###}; " +
                $"BlendFactor={levels.CoarseWeight:0.######}; CurrentAlpha={levels.FineWeight:0.######}; NextAlpha={levels.CoarseWeight:0.######}");
            var state = camera.ToViewProjection(viewport);
            Assert.True(ReferencePlaneFootprint.TryCreate(state, 0, out var footprint));
            var patch = ReferencePlanePatchPlacement.From(
                new MapRenderSnapshot("probe", 10_000, 10_000, XuanYu.Core.Map.MapSurfaceKind.Flat,
                    0, 0, 1, 1, 1), state, 0);
            Assert.True(patch.Contains(new Vector3d(footprint.MinX, footprint.MinY, 0)));
            Assert.True(patch.Contains(new Vector3d(footprint.MaxX, footprint.MaxY, 0)));
            output.WriteLine($"PlaneLogical=unbounded; PlaneMeshCenter=({patch.CenterX:0.###},{patch.CenterY:0.###}); " +
                $"PlaneMeshExtent=({patch.WidthMeters:0.###},{patch.DepthMeters:0.###}); " +
                $"PlaneWorldBounds=[{patch.MinX:0.###},{patch.MaxX:0.###}]x[{patch.MinY:0.###},{patch.MaxY:0.###}]; " +
                $"RenderOrigin={position}; FrustumResult=dynamic-patch-covers-camera");
        }
    }

    [Fact]
    public void Reference_plane_extent_is_independent_of_logical_map_size()
    {
        var map = new MapRenderSnapshot("probe", 10_000, 10_000,
            XuanYu.Core.Map.MapSurfaceKind.Flat, 0, 0, 1, 1, 1);
        var forward = (Vector3d.Zero - new Vector3d(4, -5, 3)).Normalize();
        var near = new RenderCameraProjection(-forward * 100, forward, Vector3d.UnitZ,
            60, 0.1, 10_000, 1);
        var far = new RenderCameraProjection(-forward * 5_000, forward, Vector3d.UnitZ,
            60, 0.1, 100_000, 1);
        var nearPatch = ReferencePlanePatchPlacement.From(map,
            near.ToViewProjection(new ViewportState(0, 0, 776, 650, 776, 650, 1, 1)), 0);
        var farState = far.ToViewProjection(new ViewportState(0, 0, 776, 650, 776, 650, 1, 1));
        var farPatch = ReferencePlanePatchPlacement.From(map, farState, 0);
        Assert.True(ReferencePlaneFootprint.TryCreate(farState, 0, out var farFootprint));
        Assert.True(farPatch.WidthMeters > nearPatch.WidthMeters);
        Assert.Equal(farFootprint.CenterX, farPatch.CenterX, 6);
        Assert.Equal(farFootprint.CenterY, farPatch.CenterY, 6);
    }
}

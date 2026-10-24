using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Map;

public sealed class ReferencePlaneHorizonCoverageTests
{
    [Fact] public void ReferencePlaneHorizonCoverageTest()
    {
        var state = State(3_000, 10_000);
        Assert.True(ReferencePlaneFootprint.TryCreate(state, 0, out var footprint));
        var top = WorldRayFactory.FromViewportPoint(state, 388, 0);
        var distance = ReferencePlaneHorizonCoverage.VisualGroundDistance(state, 0, 1);
        Assert.True(footprint.Contains(ReferencePlaneHorizonCoverage.Surrogate(top, 0, distance)));
    }

    [Fact] public void ReferencePlaneProjectedEdgeOutsideViewportTest()
    {
        var state = State(3_000, 10_000);
        var map = new MapRenderSnapshot("probe", 10_000, 10_000, XuanYu.Core.Map.MapSurfaceKind.Flat, 0, 0, 1, 1, 1);
        var patch = ReferencePlanePatchPlacement.From(map, state, 0);
        var minX = double.PositiveInfinity; var maxX = double.NegativeInfinity; var minY = double.PositiveInfinity; var maxY = double.NegativeInfinity;
        var projectedCount = 0;
        for (var i = 0; i < 4; i++) { if (!state.TryProjectWorldPoint(Corner(patch, i), out var projected)) continue; projectedCount++; minX = System.Math.Min(minX, projected.X); maxX = System.Math.Max(maxX, projected.X); minY = System.Math.Min(minY, projected.Y); maxY = System.Math.Max(maxY, projected.Y); }
        for (var y = 0; y <= state.Viewport.LogicalHeight; y += (int)state.Viewport.LogicalHeight / 2)
        for (var x = 0; x <= state.Viewport.LogicalWidth; x += (int)state.Viewport.LogicalWidth / 2)
        {
            var ray = WorldRayFactory.FromViewportPoint(state, x, y);
            var t = -ray.Origin.Z / ray.Direction.Z;
            if (t > 0 && t <= state.Camera.FarPlane) Assert.True(patch.Contains(ray.Origin + ray.Direction * t));
        }
        Assert.True(projectedCount > 0 && (minX < 0 || maxX > state.Viewport.LogicalWidth || minY < 0 || maxY > state.Viewport.LogicalHeight));
    }

    [Fact] public void ReferencePlaneNearParallelRayTest()
    {
        var state = State(100, 10_000, -0.001);
        var ray = WorldRayFactory.FromViewportPoint(state, 388, 0);
        var distance = ReferencePlaneHorizonCoverage.VisualGroundDistance(state, 0, 100);
        var point = ReferencePlaneHorizonCoverage.Surrogate(ray, 0, distance);
        Assert.True(double.IsFinite(distance) && double.IsFinite(point.X) && double.IsFinite(point.Y));
    }

    [Fact] public void ReferencePlaneFarPlaneBoundaryTest()
    {
        var before = Footprint(10_000); var at = Footprint(10_001); var after = Footprint(100_000);
        Assert.True(before.Width > 0 && at.Width > 0 && after.Width > 0);
        Assert.InRange(at.Width / before.Width, 0.5, 2.0);
        Assert.InRange(after.Width / at.Width, 0.5, 2.0);
    }

    [Fact] public void ReferencePlaneZoomContinuityTest()
    {
        ReferencePlaneFootprint? previous = null;
        foreach (var distance in new[] { 1.0, 5, 20, 100, 500, 1_000, 2_000, 5_000, 10_000 })
        {
            var current = Footprint(System.Math.Max(10_000, distance * 20));
            Assert.True(double.IsFinite(current.CenterX) && double.IsFinite(current.Width));
            if (previous is { } p) Assert.InRange(current.Width / p.Width, 0.01, 100.0);
            previous = current;
        }
    }

    [Fact] public void ReferencePlaneRenderOriginInvariantTest()
    {
        var state = State(12_000, 100_000);
        Assert.True(ReferencePlaneFootprint.TryCreate(state, 0, out var footprint));
        var shifted = new ReferencePlaneFootprint(footprint.MinX + state.RenderOrigin.X, footprint.MaxX + state.RenderOrigin.X, footprint.MinY + state.RenderOrigin.Y, footprint.MaxY + state.RenderOrigin.Y);
        Assert.Equal(footprint.Width, shifted.Width, 6); Assert.Equal(footprint.Depth, shifted.Depth, 6);
    }

    static ViewProjectionState State(double distance, double far, double z = -0.424264)
    {
        var position = new Vector3d(4, -5, 3) * (distance / System.Math.Sqrt(50));
        var forward = new Vector3d(-0.566138, 0.707107, z).Normalize();
        return new RenderCameraProjection(position, forward, Vector3d.UnitZ, 60, 0.1, far, 1).ToViewProjection(new ViewportState(0, 0, 776, 650, 776, 650, 1, 1));
    }

    static ReferencePlaneFootprint Footprint(double far) { Assert.True(ReferencePlaneFootprint.TryCreate(State(3_000, far), 0, out var value)); return value; }
    static Vector3d Corner(ReferencePlanePatchPlacement p, int i) => i switch { 0 => new(p.MinX, p.MinY, 0), 1 => new(p.MaxX, p.MinY, 0), 2 => new(p.MaxX, p.MaxY, 0), _ => new(p.MinX, p.MaxY, 0) };
}

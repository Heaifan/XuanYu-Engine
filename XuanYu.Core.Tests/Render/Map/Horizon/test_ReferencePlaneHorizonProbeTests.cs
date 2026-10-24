using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using Xunit.Abstractions;

namespace XuanYu.Core.Tests.Render.Map;

public sealed class ReferencePlaneHorizonProbeTests(ITestOutputHelper output)
{
    [Fact]
    public void Current_footprint_reports_horizon_sample_loss_and_projected_edge()
    {
        var viewport = new ViewportState(0, 0, 776, 650, 776, 650, 1, 1);
        var pivot = Vector3d.Zero;
        var forward = (pivot - new Vector3d(4_000, -5_000, 3_000)).Normalize();
        var camera = new RenderCameraProjection(new Vector3d(4_000, -5_000, 3_000), forward,
            Vector3d.UnitZ, 60, 0.1, 10_000, 1);
        var state = camera.ToViewProjection(viewport);
        var names = new[] { "BottomLeft", "BottomCenter", "BottomRight", "CenterLeft", "Center", "CenterRight", "TopLeft", "TopCenter", "TopRight" };
        var points = new[] { (0.0, 650.0), (388.0, 650.0), (776.0, 650.0), (0.0, 325.0), (388.0, 325.0), (776.0, 325.0), (0.0, 0.0), (388.0, 0.0), (776.0, 0.0) };
        var valid = 0;
        foreach (var (name, point) in names.Zip(points))
        {
            var ray = WorldRayFactory.FromViewportPoint(state, point.Item1, point.Item2);
            var parallel = System.Math.Abs(ray.Direction.Z) < 0.001;
            var distance = parallel ? double.NaN : -ray.Origin.Z / ray.Direction.Z;
            var hit = !parallel && distance > 0 && distance <= camera.FarPlane;
            if (hit) valid++;
            output.WriteLine($"{name};Screen=({point.Item1},{point.Item2});RayOrigin={ray.Origin};RayDirection={ray.Direction};RayDirectionZ={ray.Direction.Z:0.######};IntersectionValid={hit};IntersectionDistance={distance:0.###};RejectedByParallelEpsilon={parallel};RejectedByFarPlane={double.IsFinite(distance) && distance > camera.FarPlane};WorldIntersection={(hit ? ray.Origin + ray.Direction * distance : "N/A")}");
        }
        Assert.True(ReferencePlaneFootprint.TryCreate(state, 0, out var footprint));
        var patch = ReferencePlanePatchPlacement.From(new MapRenderSnapshot("probe", 10_000, 10_000,
            XuanYu.Core.Map.MapSurfaceKind.Flat, 0, 0, 1, 1, 1), state, 0);
        output.WriteLine($"FootprintValidCount={valid};Bounds=({footprint.MinX:0.###},{footprint.MaxX:0.###},{footprint.MinY:0.###},{footprint.MaxY:0.###});PatchWorldBounds=({patch.MinX:0.###},{patch.MaxX:0.###},{patch.MinY:0.###},{patch.MaxY:0.###})");
        for (var i = 0; i < 4; i++)
        {
            var corner = i switch { 0 => new Vector3d(patch.MinX, patch.MinY, 0), 1 => new Vector3d(patch.MaxX, patch.MinY, 0), 2 => new Vector3d(patch.MaxX, patch.MaxY, 0), _ => new Vector3d(patch.MinX, patch.MaxY, 0) };
            output.WriteLine($"PatchCorner{i}={(state.TryProjectWorldPoint(corner, out var screen) ? screen.ToString() : "N/A")}");
        }
        Assert.True(valid < 9);
    }
}

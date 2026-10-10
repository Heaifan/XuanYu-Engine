using System.Buffers.Binary;
using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XuanYu.World.Terrain;
namespace XuanYu.World.Tests.Camera;
public sealed class XYEPR2NavigationSafetyRidgeEscapeTests
{
    [Fact(DisplayName = "XYEPR2-NAV-SAFETY-INSIDE-003")]
    public async Task Zoom_out_does_not_enter_a_high_ridge_after_starting_inside()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteHgt(257);
        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var world = vm.TerrainWorld!;
            for (var step = 0; step < 100; step++) Assert.True(vm.DollyCamera(1));
            var center = vm.ObservationCenter;
            var surface = new TerrainWorldGroundSurface(world);
            var start = vm.RenderSnapshot.CameraState;
            var startSample = ToSample(world, start.Position);
            for (var y = -1; y <= 1; y++)
            for (var x = -1; x <= 1; x++)
                SetEdit(world, startSample.X + x, startSample.Y + y, 1000);
            for (var y = 0; y < world.Metadata.Height; y++)
            for (var x = 0; x < world.Metadata.Width; x++)
            {
                var radius = Math.Max(Math.Abs(x - startSample.X), Math.Abs(y - startSample.Y));
                if (radius is >= 2 and <= 64) SetEdit(world, x, y, 30000);
            }
            var startQuery = surface.QuerySurface(new(start.Position.X, start.Position.Y));
            Assert.True(startQuery.IsValid);
            Assert.True(start.Position.Z <= startQuery.SurfaceZ * vm.VerticalExaggeration + start.NearPlane,
                $"Fixture did not start inside: sample={startSample}; center={center}; " +
                $"camera={start.Position}; query={startQuery.SurfaceZ:R}.");
            var previousClearance = start.Position.Z - startQuery.SurfaceZ * vm.VerticalExaggeration - start.NearPlane;
            var probe = new MapPoint(start.Position.X + Math.Sign(start.Position.X - center.X) * 1000,
                start.Position.Y + Math.Sign(start.Position.Y - center.Y) * 1000);
            Assert.True(surface.QuerySurface(probe).SurfaceZ >= 30000,
                $"Fixture ridge probe was not high: sample={startSample}; probe={probe}.");
            var lastValidClearance = previousClearance;
            for (var step = 0; step < 80; step++)
            {
                var before = vm.RenderSnapshot.CameraState;
                Assert.True(vm.DollyCamera(-1), $"Zoom-Out rejected at step {step}.");
                var after = vm.RenderSnapshot.CameraState;
                var query = surface.QuerySurface(new(after.Position.X, after.Position.Y));
                if (query.IsValid)
                {
                    var clearance = after.Position.Z - query.SurfaceZ * vm.VerticalExaggeration - after.NearPlane;
                    Assert.True(clearance + 1e-6 >= previousClearance,
                        $"Zoom-Out entered a deeper terrain state at step {step}: " +
                        $"beforeClearance={previousClearance:R}; current={clearance:R}; surface={query.SurfaceZ:R}; " +
                        $"before={before.Position}; after={after.Position}.");
                    previousClearance = clearance;
                    lastValidClearance = clearance;
                }

                Assert.True(after.Position.DistanceTo(center) + 1e-9 >= before.Position.DistanceTo(center));
                Assert.Equal(center, vm.ObservationCenter);
                Assert.Equal(Math.Max(after.NearPlane * 10.0,
                    after.Position.DistanceTo(center) * 4.0), after.FarPlane);
            }
            Assert.True(lastValidClearance >= previousClearance - 1e-6,
                $"Zoom-Out recovery regressed after the ridge: {lastValidClearance:R}; sample={startSample}.");
        }
        finally { File.Delete(path); }
    }
    static TerrainSampleCoordinate ToSample(TerrainWorld world, Vector3d position)
    {
        var extent = world.Metadata.WorldExtent;
        const double metersPerDegree = 6378137.0 * Math.PI / 180.0;
        var longitudeMeters = metersPerDegree * Math.Cos(extent.South * Math.PI / 180.0);
        var x = position.X / (longitudeMeters * (extent.East - extent.West)) * (world.Metadata.Width - 1);
        var y = (1 - position.Y / (metersPerDegree * (extent.North - extent.South))) * (world.Metadata.Height - 1);
        return new(Math.Clamp((int)Math.Round(x), 0, world.Metadata.Width - 1),
            Math.Clamp((int)Math.Round(y), 0, world.Metadata.Height - 1));
    }
    static void SetEdit(TerrainWorld world, int x, int y, double height)
    {
        if (x >= 0 && x < world.Metadata.Width && y >= 0 && y < world.Metadata.Height)
            world.EditDelta.Set(new(x, y), height);
    }
    static string WriteHgt(int side)
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xyepr2-ridge-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        for (var index = 0; index < side * side; index++)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, 0);
            stream.Write(bytes);
        }
        return path;
    }
}

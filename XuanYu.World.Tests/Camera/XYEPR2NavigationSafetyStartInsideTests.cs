using System.Buffers.Binary;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Terrain;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.Camera;

public sealed class XYEPR2NavigationSafetyStartInsideTests(ITestOutputHelper output)
{
    [Fact(DisplayName = "XYEPR2-NAV-SAFETY-INSIDE-001")]
    public async Task Zoom_out_escapes_when_camera_starts_inside_surface()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var firstPath = WriteHgt();

        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(firstPath));
            var world = vm.TerrainWorld!;
            var lowSurface = new TerrainWorldGroundSurface(world);
            for (var step = 0; step < 100; step++)
                Assert.True(vm.DollyCamera(1));

            var before = vm.RenderSnapshot.CameraState;
            var insideQuery = lowSurface.QuerySurface(new(before.Position.X, before.Position.Y));
            Assert.True(insideQuery.IsValid, $"Fixed Dolly fixture did not reach valid XY: {before.Position}.");
            // Mutate the existing TerrainWorld edit layer so the camera remains inside the same bounds.
            for (var y = 0; y < 3; y++)
            for (var x = 0; x < 3; x++)
                world.EditDelta.Set(new TerrainSampleCoordinate(x, y), 30000);
            var surface = new TerrainWorldGroundSurface(world);
            var query = surface.QuerySurface(new(before.Position.X, before.Position.Y));
            var extent = world.Metadata.WorldExtent;
            const double metersPerDegreeLatitude = 6378137.0 * Math.PI / 180.0;
            var metersPerDegreeLongitude = metersPerDegreeLatitude * Math.Cos(extent.South * Math.PI / 180.0);
            var latitude = extent.South + before.Position.Y / metersPerDegreeLatitude;
            var longitude = extent.West + before.Position.X / metersPerDegreeLongitude;
            var normalizedX = (longitude - extent.West) / (extent.East - extent.West);
            var normalizedY = (extent.North - latitude) / (extent.North - extent.South);
            output.WriteLine($"before={before.Position}; center={vm.ObservationCenter}; extent={extent}; query={query.Status}; geo=({latitude:R},{longitude:R}); normalized=({normalizedX:R},{normalizedY:R})");
            Assert.Equal(WorldQueryStatus.Valid, query.Status);
            Assert.True(before.Position.Z <= query.SurfaceZ * vm.VerticalExaggeration + before.NearPlane,
                $"Fixture did not place camera inside terrain: cameraZ={before.Position.Z:R}; surfaceZ={query.SurfaceZ:R}.");

            var center = vm.ObservationCenter;
            var beforeDistance = before.Position.DistanceTo(center);
            Assert.True(vm.DollyCamera(-1));
            var after = vm.RenderSnapshot.CameraState;

            Assert.True(after.Position.DistanceTo(center) > beforeDistance,
                "Zoom-Out must move away from ObservationCenter even when the starting camera is below the surface.");
            Assert.True(after.Position.Z > before.Position.Z,
                $"Zoom-Out did not escape upward: before={before.Position}; after={after.Position}.");
            Assert.Equal(center, vm.ObservationCenter);
        }
        finally
        {
            File.Delete(firstPath);
        }
    }

    static string WriteHgt()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xyepr2-inside-first-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        foreach (var value in new short[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 })
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, value);
            stream.Write(bytes);
        }
        return path;
    }
}

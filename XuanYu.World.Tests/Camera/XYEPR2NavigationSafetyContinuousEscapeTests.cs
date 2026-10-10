using System.Buffers.Binary;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Terrain;

namespace XuanYu.World.Tests.Camera;

public sealed class XYEPR2NavigationSafetyContinuousEscapeTests
{
    [Fact(DisplayName = "XYEPR2-NAV-SAFETY-INSIDE-002")]
    public async Task Repeated_zoom_out_continues_until_inside_pose_has_clearance()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteHgt();
        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var world = vm.TerrainWorld!;
            for (var step = 0; step < 100; step++) Assert.True(vm.DollyCamera(1));

            // Raise the declared TerrainWorld surface at the deterministic close pose.
            for (var y = 0; y < 3; y++)
            for (var x = 0; x < 3; x++)
                world.EditDelta.Set(new TerrainSampleCoordinate(x, y), 1000);

            var center = vm.ObservationCenter;
            var surface = new TerrainWorldGroundSurface(world);
            var initial = vm.RenderSnapshot.CameraState;
            var initialQuery = surface.QuerySurface(new(initial.Position.X, initial.Position.Y));
            Assert.True(initialQuery.IsValid);
            Assert.True(initial.Position.Z <= initialQuery.SurfaceZ * vm.VerticalExaggeration + initial.NearPlane,
                $"Fixture did not start inside terrain: camera={initial.Position}; query={initialQuery.Status}/{initialQuery.SurfaceZ:R}.");

            var moved = 0;
            var validClearance = false;
            for (var step = 0; step < 80; step++)
            {
                var before = vm.RenderSnapshot.CameraState;
                var beforeDistance = before.Position.DistanceTo(center);
                Assert.True(vm.DollyCamera(-1), $"Zoom-Out rejected at step {step}.");
                var after = vm.RenderSnapshot.CameraState;
                var afterDistance = after.Position.DistanceTo(center);
                if (afterDistance > beforeDistance + 1e-9) moved++;
                Assert.True(afterDistance + 1e-9 >= beforeDistance,
                    $"Zoom-Out moved toward ObservationCenter at step {step}: before={before.Position}; after={after.Position}.");
                Assert.True(after.Position.Z + 1e-9 >= before.Position.Z,
                    $"Zoom-Out moved deeper below terrain at step {step}: before={before.Position}; after={after.Position}.");

                var query = surface.QuerySurface(new(after.Position.X, after.Position.Y));
                if (query.IsValid)
                    validClearance |= after.Position.Z - query.SurfaceZ * vm.VerticalExaggeration - after.NearPlane > 0;

                Assert.Equal(center, vm.ObservationCenter);
                var expectedFar = Math.Max(after.NearPlane * 10.0,
                    after.Position.DistanceTo(center) * 4.0);
                Assert.Equal(expectedFar, after.FarPlane);
            }

            Assert.True(moved >= 3, "Repeated Zoom-Out must continue moving after the initial recovery step.");
            Assert.True(validClearance, "Repeated Zoom-Out must eventually establish positive terrain clearance.");
        }
        finally { File.Delete(path); }
    }

    static string WriteHgt()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xyepr2-continuous-{Guid.NewGuid():N}.hgt");
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

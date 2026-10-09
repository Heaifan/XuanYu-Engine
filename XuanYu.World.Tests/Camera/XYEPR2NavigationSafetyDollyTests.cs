using System.Buffers.Binary;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class XYEPR2NavigationSafetyDollyTests
{
    [Fact]
    public async Task Editor_dolly_does_not_cross_the_dem_sample()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem([0, 0, 0, 0, 100, 0, 0, 0, 0]);
        try { Assert.True(await vm.ImportTerrainSourceAsync(path)); }
        finally { File.Delete(path); }

        var center = vm.ObservationCenter;
        var surface = new TerrainWorldGroundSurface(vm.TerrainWorld!);
        for (var step = 0; step < 100; step++) Assert.True(vm.DollyCamera(1));

        var camera = vm.RenderSnapshot.CameraState;
        var query = surface.QuerySurface(new(camera.Position.X, camera.Position.Y));
        Assert.True(query.IsValid);
        Assert.True(camera.Position.Z > query.SurfaceZ * vm.VerticalExaggeration,
            $"Editor Dolly crossed DEM sample at camera XY=({camera.Position.X:R},{camera.Position.Y:R}); " +
            $"surfaceZ={query.SurfaceZ:R}; camera={camera.Position}.");
        Assert.Equal(center, vm.ObservationCenter);
    }

    static string WriteDem(short[] samples)
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xye-pr2-near-terrain-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        foreach (var sample in samples)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, sample);
            stream.Write(bytes);
        }
        return path;
    }
}

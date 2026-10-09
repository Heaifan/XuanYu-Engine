using System.Buffers.Binary;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Camera;

public sealed class CAuditR1SurfaceAabbTests
{
    [Fact]
    public async Task Initial_frame_uses_aabb_z_midpoint_not_center_dem_sample()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem([0, 0, 0, 0, 100, 0, 0, 0, 0]);
        try { Assert.True(await vm.ImportTerrainSourceAsync(path)); }
        finally { File.Delete(path); }

        var center = vm.ObservationCenter;
        var surface = new TerrainWorldGroundSurface(vm.TerrainWorld!);
        var query = surface.QuerySurface(new(center.X, center.Y));

        Assert.True(query.IsValid);
        Assert.Equal(50, center.Z);
        Assert.Equal(100, query.SurfaceZ);
    }

    [Fact]
    public void Editor_dolly_preserves_observation_center()
    {
        var vm = new UiVm(null, () => true);
        var center = vm.ObservationCenter;
        Assert.True(vm.DollyCamera(1));
        Assert.Equal(center, vm.ObservationCenter);
    }

    static string WriteDem(short[] samples)
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-caudit-surface-{Guid.NewGuid():N}.hgt");
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

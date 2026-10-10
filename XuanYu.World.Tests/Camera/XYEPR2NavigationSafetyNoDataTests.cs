using System.Buffers.Binary;
using XuanYu.Editor.UI;
using XuanYu.World;
using XuanYu.World.Geo;
using XuanYu.World.Tests.Terrain;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.Camera;

/// <summary>
/// WP2 boundary evidence: a missing surface sample must not permanently block
/// a legal outward Dolly.  This is intentionally RED until the product failure
/// policy for NoData is implemented and approved.
/// </summary>
public sealed class XYEPR2NavigationSafetyNoDataTests(ITestOutputHelper output)
{
    [Fact(DisplayName = "XYEPR2-NAV-SAFETY-NODATA-001")]
    public async Task Surface_query_preserves_known_nodata_and_records_camera_policy_pending()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteHgtWithCenterNoData();
        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(path));

            var surface = new XuanYu.Editor.MapEditing.TerrainWorldGroundSurface(vm.TerrainWorld!);
            var extent = vm.TerrainWorld!.Metadata.WorldExtent;
            var mapping = new GeographicWorldMapping(new(extent.South, extent.West, 0));
            var center = mapping.ToWorld(new(
                (extent.South + extent.North) / 2.0,
                (extent.West + extent.East) / 2.0, 0));
            var noDataQuery = surface.QuerySurface(new(center.X, center.Y));
            var validQuery = surface.QuerySurface(new(0, 0));
            var camera = vm.RenderSnapshot.CameraState;
            var cameraQuery = surface.QuerySurface(new(camera.Position.X, camera.Position.Y));
            output.WriteLine($"camera={camera.Position}; center={center}; size=" +
                $"{vm.TerrainWorld.Metadata.Width}x{vm.TerrainWorld.Metadata.Height}; extent={extent}; " +
                $"centerStatus={noDataQuery.Status}; outerStatus={validQuery.Status}; " +
                $"cameraStatus={cameraQuery.Status}; CameraStrategy=UNKNOWN/PENDING");
            Assert.Equal(WorldQueryStatus.NoData, noDataQuery.Status);
            Assert.Equal(WorldQueryStatus.Valid, validQuery.Status);
        }
        finally
        {
            File.Delete(path);
        }
    }

    static string WriteHgtWithCenterNoData()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xyepr2-nodata-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        // Keep an effective outer ring for import/AutoFrame, while making the
        // center 3x3 cells NoData.  The test queries the exact geographic
        // center, which maps deterministically to sample (2, 2).
        for (var row = 0; row < 5; row++)
        for (var column = 0; column < 5; column++)
        {
            var sample = row is >= 1 and <= 3 && column is >= 1 and <= 3
                ? short.MinValue
                : (short)0;
            BinaryPrimitives.WriteInt16BigEndian(bytes, sample);
            stream.Write(bytes);
        }
        return path;
    }
}

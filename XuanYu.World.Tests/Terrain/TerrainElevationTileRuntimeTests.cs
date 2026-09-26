using System.Buffers.Binary;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using SourceTile = XuanYu.World.Terrain.Source.TerrainElevationTile;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainElevationTileRuntimeTests
{
    [Fact]
    public void Hgt_reader_decodes_big_endian_and_normalizes_south_to_north()
    {
        var path = WriteHgt("n23e121", [100, 200, 300, 400]);
        var tile = HgtTerrainElevationTileReader.Read(path);

        Assert.Equal(2, tile.Width);
        Assert.Equal(2, tile.Height);
        Assert.Equal(300, tile.GetElevation(23, 121).ElevationMeters);
        Assert.Equal(400, tile.GetElevation(23, 122).ElevationMeters);
        Assert.Equal(100, tile.GetElevation(24, 121).ElevationMeters);
        Assert.Equal(200, tile.GetElevation(24, 122).ElevationMeters);
    }

    [Fact]
    public void Runtime_snapshot_reduces_large_tile_without_changing_full_world_data()
    {
        var values = Enumerable.Range(0, 3601 * 3601).Select(_ => 123.0).ToArray();
        var tile = SourceTile.Create(3601, 3601, values,
            new TerrainGeoBounds(23, 121, 24, 122), new(1, 1));
        var world = TerrainWorld.FromElevationTile(tile);

        var snapshot = world.ToRenderSnapshot("n23e121", 1);

        Assert.Equal(3601, world.Metadata.Width);
        Assert.Equal(513, snapshot.Heightfield.Width);
        Assert.Equal(513, snapshot.Heightfield.Height);
        Assert.Equal(7, snapshot.CellSizeMeters);
        Assert.Equal(123, snapshot.Heightfield.ElevationAt(512, 512));
    }

    [Fact]
    public void Runtime_snapshot_keeps_real_meter_scale_and_geographic_orientation()
    {
        var tile = SourceTile.Create(3, 3,
            [10, 20, 30, 40, 50, 60, 70, 80, 90],
            new TerrainGeoBounds(23, 121, 24, 122), new(0.5, 0.5));

        var snapshot = TerrainWorld.FromElevationTile(tile).ToRenderSnapshot("tile", 1);

        Assert.Equal(70, snapshot.Heightfield.ElevationAt(0, 0));
        Assert.Equal(30, snapshot.Heightfield.ElevationAt(2, 2));
        Assert.Equal(0.5, snapshot.Metadata.ResolutionX);
        Assert.Equal(0.5, snapshot.Metadata.ResolutionY);
        Assert.Equal(1.0, snapshot.Heightfield.CellSizeMeters);
    }

    static string WriteHgt(string name, short[] values)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{name}-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        foreach (var value in values)
        {
            var bytes = new byte[2];
            BinaryPrimitives.WriteInt16BigEndian(bytes, value);
            stream.Write(bytes);
        }
        return path;
    }
}

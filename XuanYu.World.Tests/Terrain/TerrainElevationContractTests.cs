using System.Buffers.Binary;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainElevationContractTests
{
    [Fact]
    public void Stream_reader_exposes_the_frozen_nasadem_contract()
    {
        var bytes = new byte[8];
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan(0, 2), 10);
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan(2, 2), 20);
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan(4, 2), 30);
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan(6, 2), 40);

        var tile = ((IHgtReader)new HgtTerrainElevationTileReader()).Read(
            new MemoryStream(bytes), "n23e121");

        Assert.Equal("n23e121", tile.TileId);
        Assert.Equal(new TerrainGeoBounds(23, 121, 24, 122), tile.Bounds);
        Assert.Equal(TerrainElevationUnit.Meter, tile.ElevationUnit);
        Assert.Equal(TerrainVerticalDatum.Egm96Geoid, tile.VerticalDatum);
        Assert.Equal(TerrainSourceFormat.NasademHgt, tile.SourceFormat);
    }

    [Fact]
    public void Query_returns_invalid_for_hgt_nodata_samples()
    {
        var tile = TerrainElevationTile.Create(2, 2,
            [short.MinValue, 20, 30, 40], new(23, 121, 24, 122), new(1, 1));

        var result = tile.GetElevation(24, 121);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Tile_requires_a_stable_identity()
    {
        var raster = new TerrainSourceRaster(2, 2, [1, 2, 3, 4], [false, false, false, false]);

        Assert.Throws<ArgumentException>(() => new TerrainElevationTile(
            " ", new(23, 121, 24, 122), raster, new(1, 1),
            TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt));
    }
}

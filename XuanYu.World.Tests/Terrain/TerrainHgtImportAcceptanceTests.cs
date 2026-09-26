using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainHgtImportAcceptanceTests
{
    [Fact]
    public void Real_hgt_layout_becomes_tile_with_bounds_samples_and_nodata()
    {
        var progress = new TerrainImportAcceptanceFixture.ProgressCapture();
        var reader = (IHgtReader)new HgtTerrainElevationTileReader();
        using var stream = TerrainImportAcceptanceFixture.HgtStream(100, 200, short.MinValue, 400);

        var tile = reader.Read(stream, "n23e121", progress);

        Assert.Equal("n23e121", tile.TileId);
        Assert.Equal(2, tile.Width);
        Assert.Equal(new TerrainGeoBounds(23, 121, 24, 122), tile.Bounds);
        Assert.Equal(100, tile.ElevationAt(0, 0));
        Assert.True(tile.Raster.NoDataMask[2]);
        Assert.False(tile.GetElevation(23.5, 121).IsValid);
        Assert.All(progress.Values, item => Assert.InRange(item.Percentage, 0, 100));
    }

    [Fact]
    public void Reader_query_uses_known_geographic_coordinates()
    {
        var reader = (IHgtReader)new HgtTerrainElevationTileReader();
        using var stream = TerrainImportAcceptanceFixture.HgtStream(100, 200, 300, 400);
        var tile = reader.Read(stream, "n23e121");

        Assert.Equal(300, tile.GetElevation(23, 121).ElevationMeters);
        Assert.Equal(400, tile.GetElevation(23, 122).ElevationMeters);
        Assert.Equal(100, tile.GetElevation(24, 121).ElevationMeters);
        Assert.Equal(200, tile.GetElevation(24, 122).ElevationMeters);
        Assert.False(tile.GetElevation(22.9, 121).IsValid);
    }
}

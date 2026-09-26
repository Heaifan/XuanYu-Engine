using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainSourceImportTests
{
    [Fact]
    public void Reads_raster_values_and_nodata_without_zeroing_real_zero()
    {
        var path = WriteGrid("ncols 3\nnrows 2\nxllcorner 10\nyllcorner 20\ncellsize 5\nNODATA_value -9999\n1 0 -2\n-9999 3 4\n");
        var source = EsriAsciiGridTerrainReader.Read(path);

        Assert.Equal((3, 2), (source.Raster.Width, source.Raster.Height));
        Assert.Equal([1d, 0d, -2d, -9999d, 3d, 4d], source.Raster.ElevationMeters);
        Assert.Equal([false, false, false, true, false, false], source.Raster.NoDataMask);
    }

    [Fact]
    public void Reads_bounds_resolution_crs_and_metadata()
    {
        var path = WriteGrid("ncols 2\nnrows 2\nxllcorner 10\nyllcorner 20\ncellsize 5\nNODATA_value -9999\n1 2\n3 4\n");
        File.WriteAllText(Path.ChangeExtension(path, ".prj"), "EPSG:4326");
        var source = EsriAsciiGridTerrainReader.Read(path);

        Assert.Equal("EPSG:4326", source.SourceCrs);
        Assert.Equal(new TerrainGeographicBounds(10, 20, 20, 30), source.GeographicBounds);
        Assert.Equal(new TerrainResolution(5, 5), source.Resolution);
        Assert.Equal("ESRI ASCII Grid", source.Metadata["format"]);
    }

    [Fact]
    public void Rejects_truncated_or_invalid_grid()
    {
        var path = WriteGrid("ncols 2\nnrows 1\nxllcorner 0\nyllcorner 0\ncellsize 1\nNODATA_value -9999\n1\n");

        var error = Assert.Throws<TerrainSourceReadException>(() => EsriAsciiGridTerrainReader.Read(path));

        Assert.Contains("栅格", error.Message);
    }

    static string WriteGrid(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}.asc");
        File.WriteAllText(path, content);
        return path;
    }
}

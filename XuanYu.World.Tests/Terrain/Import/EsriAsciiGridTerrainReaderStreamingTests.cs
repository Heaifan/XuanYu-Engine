using XuanYu.World.Terrain.Import;

namespace XuanYu.World.Tests.Terrain.Import;

public sealed class EsriAsciiGridTerrainReaderStreamingTests
{
    [Fact]
    public void Rejects_sample_count_below_declared_grid_size_with_explicit_error()
    {
        var path = WriteGrid("ncols 2\nnrows 2\nxllcorner 0\nyllcorner 0\ncellsize 1\nNODATA_value -9999\n1 2\n3\n");

        var error = Assert.Throws<TerrainSourceReadException>(() => EsriAsciiGridTerrainReader.Read(path));

        Assert.Contains("不足", error.Message);
    }

    [Fact]
    public void Rejects_sample_count_above_declared_grid_size_with_explicit_error()
    {
        var path = WriteGrid("ncols 2\nnrows 1\nxllcorner 0\nyllcorner 0\ncellsize 1\nNODATA_value -9999\n1 2 3\n");

        var error = Assert.Throws<TerrainSourceReadException>(() => EsriAsciiGridTerrainReader.Read(path));

        Assert.Contains("超出", error.Message);
    }

    [Fact]
    public void Rejects_invalid_elevation_token_as_terrain_source_read_error()
    {
        var path = WriteGrid("ncols 1\nnrows 1\nxllcorner 0\nyllcorner 0\ncellsize 1\nNODATA_value -9999\nnot-a-number\n");

        var error = Assert.Throws<TerrainSourceReadException>(() => EsriAsciiGridTerrainReader.Read(path));

        Assert.Contains("高程样本非法", error.Message);
    }

    static string WriteGrid(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"terrain-streaming-{Guid.NewGuid():N}.asc");
        File.WriteAllText(path, content);
        return path;
    }
}

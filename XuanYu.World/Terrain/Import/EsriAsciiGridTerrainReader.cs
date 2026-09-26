using System.Globalization;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain.Import;

public sealed class EsriAsciiGridTerrainReader : ITerrainSourceReader
{
    static readonly string[] Required = ["ncols", "nrows", "cellsize", "nodata_value"];

    public static TerrainSourceData Read(string path) => new EsriAsciiGridTerrainReader().ReadFile(path);

    TerrainSourceData ITerrainSourceReader.Read(string path) => ReadFile(path);

    TerrainSourceData ReadFile(string path)
    {
        try { return ReadCore(path); }
        catch (TerrainSourceReadException) { throw; }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or FormatException or OverflowException)
        { throw new TerrainSourceReadException("DEM 文件读取失败。", error); }
    }

    static TerrainSourceData ReadCore(string path)
    {
        if (!File.Exists(path)) throw new TerrainSourceReadException("DEM 文件不存在。");
        using var reader = new StreamReader(path);
        var header = ParseHeader(ReadHeader(reader));
        var width = ReadInt(header, "ncols");
        var height = ReadInt(header, "nrows");
        var resolution = ReadDouble(header, "cellsize");
        var noData = ReadDouble(header, "nodata_value");
        if (width <= 0 || height <= 0 || resolution <= 0) throw new TerrainSourceReadException("DEM 栅格尺寸或分辨率非法。");
        int expected;
        try { expected = checked(width * height); }
        catch (OverflowException error) { throw new TerrainSourceReadException("DEM 栅格尺寸过大。", error); }
        var samples = ReadSamples(reader, expected, noData, out var mask);
        var west = Corner(header, "xllcorner", "xllcenter", resolution);
        var south = Corner(header, "yllcorner", "yllcenter", resolution);
        var bounds = new TerrainGeographicBounds(west, south, west + width * resolution, south + height * resolution);
        var metadata = header.ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
        metadata["format"] = "ESRI ASCII Grid";
        var crsPath = Path.ChangeExtension(path, ".prj");
        var crs = File.Exists(crsPath) ? File.ReadAllText(crsPath).Trim() : "Unknown";
        return new TerrainSourceData(new TerrainSourceRaster(width, height, samples, mask), crs,
            bounds, new TerrainResolution(resolution, resolution), metadata);
    }

    static string[] ReadHeader(StreamReader reader)
    {
        var lines = new string[6];
        for (var index = 0; index < lines.Length; index++)
            lines[index] = reader.ReadLine() ?? throw new TerrainSourceReadException("DEM 文件缺少栅格头或样本。");
        return lines;
    }

    static Dictionary<string, string> ParseHeader(IEnumerable<string> lines)
    {
        var header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in lines)
        {
            var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !header.TryAdd(parts[0], parts[1])) throw new TerrainSourceReadException("DEM 栅格头非法。");
        }
        if (Required.Any(key => !header.ContainsKey(key)) || (!header.ContainsKey("xllcorner") && !header.ContainsKey("xllcenter")) ||
            (!header.ContainsKey("yllcorner") && !header.ContainsKey("yllcenter")))
            throw new TerrainSourceReadException("DEM 栅格头缺少必要字段。");
        return header;
    }

    static double[] ReadSamples(StreamReader reader, int expected, double noData, out bool[] mask)
    {
        var samples = new double[expected];
        mask = new bool[expected];
        var count = 0;
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            foreach (var token in line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                if (count >= expected) throw new TerrainSourceReadException("DEM 栅格样本数量超出声明尺寸。");
                double value;
                try { value = double.Parse(token, CultureInfo.InvariantCulture); }
                catch (FormatException error) { throw new TerrainSourceReadException($"高程样本非法：{token}。", error); }
                catch (OverflowException error) { throw new TerrainSourceReadException($"高程样本非法：{token}。", error); }
                samples[count] = value;
                mask[count++] = value == noData;
            }
        }
        if (count < expected) throw new TerrainSourceReadException("DEM 栅格样本数量不足声明尺寸。");
        return samples;
    }

    static int ReadInt(IReadOnlyDictionary<string, string> header, string key) => int.Parse(header[key], CultureInfo.InvariantCulture);
    static double ReadDouble(IReadOnlyDictionary<string, string> header, string key) => double.Parse(header[key], CultureInfo.InvariantCulture);

    static double Corner(IReadOnlyDictionary<string, string> header, string cornerKey, string centerKey, double cellSize) =>
        ReadDouble(header, header.ContainsKey(cornerKey) ? cornerKey : centerKey) - (header.ContainsKey(cornerKey) ? 0 : cellSize / 2);
}

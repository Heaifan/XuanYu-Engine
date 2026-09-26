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
        var lines = File.ReadAllLines(path);
        if (lines.Length < 7) throw new TerrainSourceReadException("DEM 文件缺少栅格头或样本。");
        var header = ParseHeader(lines[..6]);
        var width = ReadInt(header, "ncols");
        var height = ReadInt(header, "nrows");
        var resolution = ReadDouble(header, "cellsize");
        var noData = ReadDouble(header, "nodata_value");
        if (width <= 0 || height <= 0 || resolution <= 0) throw new TerrainSourceReadException("DEM 栅格尺寸或分辨率非法。");
        var samples = ReadSamples(lines[6..], width * height, noData, out var mask);
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

    static double[] ReadSamples(IEnumerable<string> lines, int expected, double noData, out bool[] mask)
    {
        var values = string.Join(" ", lines).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != expected) throw new TerrainSourceReadException("DEM 栅格样本数量与尺寸不一致。");
        var samples = values.Select(value => double.Parse(value, CultureInfo.InvariantCulture)).ToArray();
        mask = samples.Select(value => value == noData).ToArray();
        return samples;
    }

    static int ReadInt(IReadOnlyDictionary<string, string> header, string key) => int.Parse(header[key], CultureInfo.InvariantCulture);
    static double ReadDouble(IReadOnlyDictionary<string, string> header, string key) => double.Parse(header[key], CultureInfo.InvariantCulture);

    static double Corner(IReadOnlyDictionary<string, string> header, string cornerKey, string centerKey, double cellSize) =>
        ReadDouble(header, header.ContainsKey(cornerKey) ? cornerKey : centerKey) - (header.ContainsKey(cornerKey) ? 0 : cellSize / 2);
}

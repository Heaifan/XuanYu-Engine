using System.Buffers.Binary;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO.Compression;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain.Import;

public sealed class HgtTerrainElevationTileReader : IHgtReader
{
    public static TerrainElevationTile ReadZip(string path,
        IProgress<TerrainImportProgress>? progress = null)
    {
        try
        {
            progress?.Report(new(TerrainImportStage.ReadingArchive, 0, 1, 8, "正在读取 ZIP"));
            using var archive = ZipFile.OpenRead(path);
            var entry = archive.Entries.SingleOrDefault(item =>
                item.Name.EndsWith(".hgt", StringComparison.OrdinalIgnoreCase));
            if (entry is null) throw new TerrainSourceReadException("HGT 缺失。");
            using var input = entry.Open();
            using var output = new MemoryStream();
            input.CopyTo(output);
            progress?.Report(new(TerrainImportStage.ReadingElevation, 0, output.Length, 15, "正在读取高程"));
            return ReadBytes(entry.Name, output.ToArray(), progress);
        }
        catch (InvalidDataException error) { throw new TerrainSourceReadException("ZIP 无效。", error); }
    }

    static readonly Regex Name = new("^([ns])(\\d{2})([ew])(\\d{3})$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static TerrainElevationTile Read(string path)
    {
        if (!File.Exists(path)) throw new TerrainSourceReadException("HGT 文件不存在。");
        return ReadBytes(path, File.ReadAllBytes(path), null);
    }

    public TerrainElevationTile Read(Stream stream, string tileId,
        IProgress<TerrainImportProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return ReadBytes(tileId, buffer.ToArray(), progress);
    }

    static TerrainElevationTile ReadBytes(string path, byte[] bytes,
        IProgress<TerrainImportProgress>? progress)
    {
        var side = checked((int)Math.Sqrt(bytes.Length / 2));
        if (side < 2 || side * side * 2 != bytes.Length)
            throw new TerrainSourceReadException("HGT 文件尺寸不是有效的方形 16 位栅格。");
        var (bounds, resolution) = ParseTileName(path, side);
        var values = new double[side * side];
        var mask = new bool[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            var value = BinaryPrimitives.ReadInt16BigEndian(bytes.AsSpan(index * 2, 2));
            values[index] = value;
            mask[index] = value == short.MinValue;
            if ((index & 0x1FFF) == 0)
                progress?.Report(new(TerrainImportStage.Decoding, index, values.Length,
                    40 + index * 30d / values.Length, "正在解码高程"));
        }
        return new TerrainElevationTile(Path.GetFileNameWithoutExtension(path), bounds,
            new TerrainSourceRaster(side, side, values, mask), resolution,
            TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt);
    }

    static (TerrainGeoBounds Bounds, TerrainResolution Resolution) ParseTileName(
        string path, int side)
    {
        var stem = Path.GetFileNameWithoutExtension(path).Split('-')[0];
        var match = Name.Match(stem);
        if (!match.Success) throw new TerrainSourceReadException("HGT 文件名必须是 n23e121 形式。");
        var latitude = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        var longitude = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
        var south = match.Groups[1].Value.Equals("s", StringComparison.OrdinalIgnoreCase)
            ? -latitude - 1 : latitude;
        var west = match.Groups[3].Value.Equals("w", StringComparison.OrdinalIgnoreCase)
            ? -longitude - 1 : longitude;
        var bounds = new TerrainGeoBounds(south, west, south + 1, west + 1);
        var latitudeMeters = 111_320.0 / (side - 1);
        var longitudeMeters = latitudeMeters * Math.Cos((south + 0.5) * Math.PI / 180.0);
        return (bounds, new TerrainResolution(longitudeMeters, latitudeMeters));
    }
}

namespace XuanYu.World.Terrain.Source;

public sealed class TerrainElevationTile : ITerrainElevationQuery
{
    public TerrainElevationTile(string tileId, TerrainGeoBounds bounds,
        TerrainSourceRaster raster, TerrainResolution resolution,
        TerrainElevationUnit unit, TerrainVerticalDatum datum,
        TerrainSourceFormat format)
    {
        if (string.IsNullOrWhiteSpace(tileId))
            throw new ArgumentException("TileId 不能为空。", nameof(tileId));
        ArgumentNullException.ThrowIfNull(raster);
        if (raster.ElevationMeters.Count != raster.Width * raster.Height)
            throw new ArgumentException("Tile 样本数量与尺寸不一致。", nameof(raster));
        TileId = tileId; Bounds = bounds; Raster = raster; Resolution = resolution;
        ElevationUnit = unit; VerticalDatum = datum; SourceFormat = format;
    }

    public TerrainElevationTile(TerrainSourceRaster raster, TerrainGeoBounds bounds)
        : this("tile", bounds, raster, new(1, 1), TerrainElevationUnit.Meter,
            TerrainVerticalDatum.Egm96Geoid, TerrainSourceFormat.NasademHgt) { }

    public TerrainElevationTile(TerrainSourceRaster raster, TerrainGeoBounds bounds,
        TerrainResolution resolution)
        : this("tile", bounds, raster, resolution, TerrainElevationUnit.Meter,
            TerrainVerticalDatum.Egm96Geoid, TerrainSourceFormat.NasademHgt) { }

    public static TerrainElevationTile Create(int width, int height, double[] values,
        TerrainGeoBounds bounds, TerrainResolution resolution) =>
        new("tile", bounds, new(width, height, values,
                values.Select(value => value == short.MinValue).ToArray()),
            resolution, TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt);

    public string TileId { get; }
    public TerrainGeoBounds Bounds { get; }
    public TerrainSourceRaster Raster { get; }
    public TerrainResolution Resolution { get; }
    public TerrainElevationUnit ElevationUnit { get; }
    public TerrainVerticalDatum VerticalDatum { get; }
    public TerrainSourceFormat SourceFormat { get; }
    public int Width => Raster.Width;
    public int Height => Raster.Height;
    public double ElevationAt(int row, int column) => Raster.ElevationMeters[row * Width + column];

    public TerrainElevationResult GetElevation(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear)
    {
        if (latitude < Bounds.South || latitude > Bounds.North ||
            longitude < Bounds.West || longitude > Bounds.East)
            return TerrainElevationResult.Invalid;
        var y = (Bounds.North - latitude) / (Bounds.North - Bounds.South) * (Height - 1);
        var x = (longitude - Bounds.West) / (Bounds.East - Bounds.West) * (Width - 1);
        return interpolation == TerrainElevationInterpolation.Nearest
            ? ReadSample((int)Math.Round(y), (int)Math.Round(x))
            : Bilinear(y, x);
    }

    TerrainElevationResult Bilinear(double y, double x)
    {
        var y0 = Math.Clamp((int)Math.Floor(y), 0, Height - 1);
        var x0 = Math.Clamp((int)Math.Floor(x), 0, Width - 1);
        var y1 = Math.Min(y0 + 1, Height - 1);
        var x1 = Math.Min(x0 + 1, Width - 1);
        var fy = y - y0; var fx = x - x0;
        var nw = ReadSample(y0, x0); var ne = ReadSample(y0, x1);
        var sw = ReadSample(y1, x0); var se = ReadSample(y1, x1);
        if (!nw.IsValid || !ne.IsValid || !sw.IsValid || !se.IsValid)
            return TerrainElevationResult.Invalid;
        return new(true, nw.ElevationMeters * (1 - fy) * (1 - fx) +
            ne.ElevationMeters * (1 - fy) * fx + sw.ElevationMeters * fy * (1 - fx) +
            se.ElevationMeters * fy * fx);
    }

    TerrainElevationResult ReadSample(int row, int column) =>
        Raster.NoDataMask[row * Width + column]
            ? TerrainElevationResult.Invalid
            : new(true, ElevationAt(row, column));
}

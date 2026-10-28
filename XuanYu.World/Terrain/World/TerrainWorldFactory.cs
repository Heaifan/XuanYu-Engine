using System.Globalization;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain;

public static class TerrainWorldFactory
{
    public static TerrainWorld FromElevationTile(Source.TerrainElevationTile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        var valid = tile.Raster.ElevationMeters.Where((_, i) => !tile.Raster.NoDataMask[i]).ToArray();
        if (valid.Length == 0) throw new ArgumentException("Terrain Source 不包含有效高程。", nameof(tile));
        var metadata = new TerrainMetadata(tile.Width, tile.Height, tile.Resolution,
            valid.Min(), valid.Max(), short.MinValue, tile.Raster.NoDataMask.Count(item => item),
            new(tile.Bounds.West, tile.Bounds.South, tile.Bounds.East, tile.Bounds.North));
        var layer = TerrainHeightLayer.CreateBase(tile.Width, tile.Height,
            tile.Raster.ElevationMeters.ToArray(), tile.Raster.NoDataMask.ToArray());
        return new TerrainWorld(layer, metadata, null, renderRowsSouthToNorth: true);
    }

    public static TerrainWorld FromSource(TerrainSourceData source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var raster = source.Raster;
        var valid = raster.ElevationMeters.Where((_, index) => !raster.NoDataMask[index]).ToArray();
        if (valid.Length == 0) throw new ArgumentException("Terrain Source 不包含有效高程。", nameof(source));
        var noData = source.Metadata.TryGetValue("nodata_value", out var raw)
            && double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? (double?)value : null;
        var metadata = new TerrainMetadata(raster.Width, raster.Height, source.Resolution,
            valid.Min(), valid.Max(), noData, raster.NoDataMask.Count(item => item), source.GeographicBounds);
        var baseLayer = TerrainHeightLayer.CreateBase(raster.Width, raster.Height,
            raster.ElevationMeters, raster.NoDataMask);
        return new TerrainWorld(baseLayer, metadata, null);
    }
}

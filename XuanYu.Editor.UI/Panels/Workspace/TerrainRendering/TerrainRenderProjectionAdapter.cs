using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public static class TerrainRenderProjectionAdapter
{
    public static TerrainHeightfield ToHeightfield(this TerrainWorld world,
        int maxSamples = 513)
    {
        if (maxSamples < 2) throw new ArgumentOutOfRangeException(nameof(maxSamples));
        var width = Math.Min(maxSamples, world.Metadata.Width);
        var height = Math.Min(maxSamples, world.Metadata.Height);
        var values = new double[width * height];
        var mask = new bool[values.Length];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var index = y * width + x;
            var sourceY = height == 1 ? 0 :
                (int)Math.Round(y * (world.Metadata.Height - 1d) / (height - 1));
            if (world.RenderRowsSouthToNorth) sourceY = world.Metadata.Height - 1 - sourceY;
            var sourceX = width == 1 ? 0 :
                (int)Math.Round(x * (world.Metadata.Width - 1d) / (width - 1));
            var sample = world.GetFinalHeight(new(sourceX, sourceY));
            mask[index] = sample.IsNoData;
            values[index] = sample.IsValid ? sample.Meters : 0.0;
        }
        var metadata = world.Metadata;
        var renderMetadata = new TerrainRenderMetadata(metadata.Width, metadata.Height,
            metadata.ResolutionX, metadata.ResolutionY, metadata.MinElevation,
            metadata.MaxElevation, metadata.NoData, metadata.NoDataCount,
            new(metadata.WorldExtent.West, metadata.WorldExtent.South,
                metadata.WorldExtent.East, metadata.WorldExtent.North));
        var cellSizeX = metadata.ResolutionX * (metadata.Width - 1d) / (width - 1d);
        var cellSizeY = metadata.ResolutionY * (metadata.Height - 1d) / (height - 1d);
        return new TerrainHeightfield(width, height, values, mask, renderMetadata,
            cellSizeX, cellSizeY);
    }

    public static TerrainRenderResource ToRenderSnapshot(this TerrainWorld world,
        string terrainId, int revision, int maxSamples = 513)
    {
        var heightfield = world.ToHeightfield(maxSamples);
        return new(terrainId, revision, heightfield, heightfield.CellSizeMeters);
    }

    public static TerrainRenderResource ToRenderSnapshot(this TerrainElevationTile tile,
        string terrainId, int revision, int maxSamples = 513) =>
        TerrainWorld.FromElevationTile(tile).ToRenderSnapshot(terrainId, revision, maxSamples);
}

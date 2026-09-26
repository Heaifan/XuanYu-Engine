using XuanYu.Render.Abstractions;

namespace XuanYu.World.Terrain;

public static class TerrainWorldProjection
{
    public static TerrainHeightfield ToHeightfield(this TerrainWorld world, int maxSamples = 513)
    {
        if (maxSamples < 2) throw new ArgumentOutOfRangeException(nameof(maxSamples));
        var width = Math.Min(maxSamples, world.Metadata.Width);
        var height = Math.Min(maxSamples, world.Metadata.Height);
        var stride = Math.Max(1, (int)Math.Floor(Math.Max(world.Metadata.Width - 1,
            world.Metadata.Height - 1) / (double)Math.Max(width - 1, height - 1)));
        var values = new double[width * height];
        var mask = new bool[values.Length];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var index = y * width + x;
                var sourceY = height == 1 ? 0 : (int)Math.Round(y * (world.Metadata.Height - 1d) / (height - 1));
                if (world.RenderRowsSouthToNorth) sourceY = world.Metadata.Height - 1 - sourceY;
                var sourceX = width == 1 ? 0 : (int)Math.Round(x * (world.Metadata.Width - 1d) / (width - 1));
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
        return new TerrainHeightfield(width, height, values, mask,
            renderMetadata, Math.Max(1.0, metadata.ResolutionX * stride));
    }

    public static TerrainRenderResource ToRenderSnapshot(this TerrainWorld world,
        string terrainId, int revision, int maxSamples = 513)
    {
        var heightfield = world.ToHeightfield(maxSamples);
        return new(terrainId, revision, heightfield, heightfield.CellSizeMeters);
    }
}

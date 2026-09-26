using XuanYu.Render.Abstractions;

namespace XuanYu.World.Terrain;

public static class TerrainWorldProjection
{
    public static TerrainHeightfield ToHeightfield(this TerrainWorld world)
    {
        var values = new double[world.Metadata.Width * world.Metadata.Height];
        var mask = new bool[values.Length];
        for (var y = 0; y < world.Metadata.Height; y++)
            for (var x = 0; x < world.Metadata.Width; x++)
            {
                var index = y * world.Metadata.Width + x;
                var sample = world.GetFinalHeight(new(x, y));
                mask[index] = sample.IsNoData;
                values[index] = sample.IsValid ? sample.Meters : 0.0;
            }
        var metadata = world.Metadata;
        var renderMetadata = new TerrainRenderMetadata(metadata.Width, metadata.Height,
            metadata.ResolutionX, metadata.ResolutionY, metadata.MinElevation,
            metadata.MaxElevation, metadata.NoData, metadata.NoDataCount,
            new(metadata.WorldExtent.West, metadata.WorldExtent.South,
                metadata.WorldExtent.East, metadata.WorldExtent.North));
        return new TerrainHeightfield(metadata.Width, metadata.Height, values, mask,
            renderMetadata, metadata.ResolutionX);
    }

    public static TerrainRenderResource ToRenderSnapshot(this TerrainWorld world,
        string terrainId, int revision) =>
        new(terrainId, revision, world.ToHeightfield(), world.Metadata.ResolutionX);
}

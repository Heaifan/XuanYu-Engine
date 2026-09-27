using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainChunkPartitionerTests
{
    [Fact]
    public void Size3601x3601Produces225Chunks()
    {
        var chunks = TerrainChunkPartitioner.Partition(Field(3601, 3601), "terrain", 1);
        Assert.Equal(225, chunks.Count);
    }

    [Fact]
    public void ChunkUses240CellsAnd241Samples()
    {
        var chunk = TerrainChunkPartitioner.Partition(Field(3601, 3601), "terrain", 1)[0];
        Assert.Equal(240, chunk.CellCountX);
        Assert.Equal(240, chunk.CellCountY);
        Assert.Equal(241, chunk.SampleCountX);
        Assert.Equal(241, chunk.SampleCountY);
    }

    [Fact]
    public void ChunkBoundsAndMinMaxAreCorrect()
    {
        var field = Field(3, 3, [1, 2, 9, 4, 5, 6, 7, 8, 3]);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        Assert.Equal(0, chunk.WorldBounds.MinX);
        Assert.Equal(2, chunk.WorldBounds.MaxX);
        Assert.Equal(1, chunk.MinElevation);
        Assert.Equal(9, chunk.MaxElevation);
    }

    [Fact]
    public void RemainderChunkSupported()
    {
        var chunks = TerrainChunkPartitioner.Partition(Field(500, 500), "terrain", 1);
        var last = chunks[^1];
        Assert.Equal(19, last.CellCountX);
        Assert.Equal(19, last.CellCountY);
        Assert.Equal(241, chunks[0].SampleCountX);
    }

    [Fact]
    public void OriginalHeightfieldUnchanged()
    {
        var values = Enumerable.Range(0, 9).Select(x => (double)x).ToArray();
        var field = Field(3, 3, values);
        _ = TerrainChunkPartitioner.Partition(field, "terrain", 1);
        Assert.Equal(values, field.ElevationMeters);
    }

    [Fact]
    public void ChunkCarriesStableIdentity()
    {
        var field = Field(3, 3);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 7)[0];
        Assert.True(chunk.Matches("terrain", 7));
        Assert.False(chunk.Matches("other", 7));
        Assert.False(chunk.Matches("terrain", 8));
        Assert.True(chunk.HasValidSampleRange(field));
    }

    static TerrainHeightfield Field(int width, int height, double[]? values = null) =>
        new(width, height, values ?? Enumerable.Repeat(1d, width * height).ToArray(),
            new bool[width * height], TerrainRenderMetadata.Empty, 1d);
}

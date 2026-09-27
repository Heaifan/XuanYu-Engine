using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainChunkMeshBuilderTests
{
    [Theory]
    [InlineData(TerrainLodLevel.Lod0, 1)]
    [InlineData(TerrainLodLevel.Lod1, 2)]
    [InlineData(TerrainLodLevel.Lod2, 4)]
    [InlineData(TerrainLodLevel.Lod3, 8)]
    [InlineData(TerrainLodLevel.Lod4, 16)]
    public void LodUsesExpectedStride(TerrainLodLevel lod, int stride)
    {
        var field = Field(25, 25);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, lod, 1d);
        Assert.Equal(stride, mesh.SampleStride);
    }

    [Theory]
    [InlineData(TerrainLodLevel.Lod0)]
    [InlineData(TerrainLodLevel.Lod1)]
    [InlineData(TerrainLodLevel.Lod2)]
    [InlineData(TerrainLodLevel.Lod3)]
    [InlineData(TerrainLodLevel.Lod4)]
    public void AllLodsPreserveOuterBoundary(TerrainLodLevel lod)
    {
        var field = Field(25, 25);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, lod, 2d);
        Assert.Contains(mesh.SurfaceVertices, vertex => vertex.X == 0 && vertex.Y == 0);
        Assert.Contains(mesh.SurfaceVertices, vertex => vertex.X == 24 && vertex.Y == 24);
    }

    [Fact]
    public void VerticalExaggerationIsPreserved()
    {
        var field = Field(2, 2, [1, 2, 3, 4]);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 3d);
        Assert.Equal(12, mesh.SurfaceVertices[^1].Z);
    }

    [Fact]
    public void SkirtIsPresentForBoundarySafety()
    {
        var field = Field(3, 3);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod1, 1d);
        Assert.True(mesh.Vertices.Count > mesh.SurfaceVertices.Count);
    }

    [Fact]
    public void ChunkMeshCoordinatesAreLocalToChunk()
    {
        var field = Field(481, 241);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[1];

        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 1d);

        Assert.Equal(0, mesh.SurfaceVertices[0].X);
        Assert.Equal(0, mesh.SurfaceVertices[0].Y);
        Assert.Equal(240, chunk.StartSampleX);
    }

    [Fact]
    public void SameTerrainRevisionAcceptsFreshHeightfieldInstance()
    {
        var cachedField = Field(481, 241);
        var currentField = Field(481, 241);
        var chunk = TerrainChunkPartitioner.Partition(cachedField, "terrain", 7)[1];

        var mesh = TerrainChunkMeshBuilder.Build(currentField, chunk, TerrainLodLevel.Lod0, 1d);

        Assert.NotEmpty(mesh.Vertices);
    }

    static TerrainHeightfield Field(int width, int height, double[]? values = null) =>
        new(width, height, values ?? Enumerable.Repeat(1d, width * height).ToArray(),
            new bool[width * height], TerrainRenderMetadata.Empty, 1d);
}

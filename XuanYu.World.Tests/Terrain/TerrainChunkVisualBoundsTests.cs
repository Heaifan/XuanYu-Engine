using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainChunkVisualBoundsTests
{
    [Fact]
    public void Exaggerated_bounds_contain_scaled_surface_and_skirt_vertices()
    {
        var field = new TerrainHeightfield(2, 2, [-20, 10, 30, 100],
            new bool[4], TerrainRenderMetadata.Empty, 2, 3);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1).Single();
        var exaggeration = 5d;
        var skirtDrop = Math.Max(1, Math.Max(field.CellSizeMeters,
            field.CellSizeYMeters) * exaggeration);
        var bounds = chunk.WorldBounds.WithVerticalExaggeration(
            new TerrainRenderTransform(exaggeration), skirtDrop);
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk,
            TerrainLodLevel.Lod0, exaggeration);

        Assert.True(bounds.MinZ <= mesh.Vertices.Min(vertex => vertex.Z));
        Assert.True(bounds.MaxZ >= mesh.Vertices.Max(vertex => vertex.Z));
    }
}

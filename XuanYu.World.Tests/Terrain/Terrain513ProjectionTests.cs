using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;

namespace XuanYu.World.Tests.Terrain;

public sealed class Terrain513ProjectionTests
{
    [Fact]
    public void Downsampled_mesh_keeps_both_source_extent_endpoints()
    {
        const int sourceWidth = 1201;
        const int sourceHeight = 801;
        var samples = Enumerable.Repeat(1d, sourceWidth * sourceHeight).ToArray();
        var world = new TerrainWorld(
            TerrainHeightLayer.CreateBase(sourceWidth, sourceHeight, samples,
                new bool[samples.Length]),
            new TerrainMetadata(sourceWidth, sourceHeight, new(2, 4), 1, 1,
                null, 0, new(0, 0, 1200, 800)), null);
        var resource = world.ToRenderSnapshot("downsampled", 1, 513);
        var chunk = TerrainChunkPartitioner.Partition(resource.Heightfield,
            resource.TerrainId, resource.Revision)[^1];
        var mesh = TerrainChunkMeshBuilder.Build(resource.Heightfield, chunk,
            TerrainLodLevel.Lod4, 1);
        var last = mesh.SurfaceVertices[^1];

        Assert.Equal(2400, last.X + chunk.StartSampleX * resource.CellSizeMeters);
        Assert.Equal(3200, last.Y + chunk.StartSampleY * resource.Heightfield.CellSizeYMeters);
    }
}

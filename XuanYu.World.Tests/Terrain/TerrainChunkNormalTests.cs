using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainChunkNormalTests
{
    [Fact]
    public void Flat_chunk_surface_normals_point_up()
    {
        Assert.Equal(Vector3d.UnitZ, Normal([7, 7, 7, 7, 7, 7, 7, 7, 7]));
    }

    [Fact]
    public void Chunk_surface_normals_follow_dem_slope()
    {
        Assert.Equal(new Vector3d(-1, 0, 1).Normalize(),
            Normal([0, 2, 4, 0, 2, 4, 0, 2, 4], 2), new Vector3dComparer(1e-10));
    }

    [Fact]
    public void Y_slope_chunk_surface_normals_follow_dem_slope()
    {
        Assert.Equal(new Vector3d(0, -1, 1).Normalize(),
            Normal([0, 0, 0, 2, 2, 2, 4, 4, 4], 2), new Vector3dComparer(1e-10));
    }

    static Vector3d Normal(IReadOnlyList<double> heights, double cellSize = 1)
    {
        var field = new TerrainHeightfield(3, 3, heights, new bool[9],
            TerrainRenderMetadata.Empty, cellSize);
        var chunk = TerrainChunkPartitioner.Partition(field, "terrain", 1)[0];
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 1);
        var vertex = mesh.SurfaceVertices[4];
        return new(vertex.Nx, vertex.Ny, vertex.Nz);
    }

    sealed class Vector3dComparer(double tolerance) : IEqualityComparer<Vector3d>
    {
        public bool Equals(Vector3d x, Vector3d y) =>
            Math.Abs(x.X - y.X) <= tolerance && Math.Abs(x.Y - y.Y) <= tolerance &&
            Math.Abs(x.Z - y.Z) <= tolerance;

        public int GetHashCode(Vector3d obj) => 0;
    }
}

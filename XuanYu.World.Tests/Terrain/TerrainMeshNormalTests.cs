using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainMeshNormalTests
{
    [Fact]
    public void Flat_grid_normals_point_up()
    {
        var mesh = Build([7, 7, 7, 7], 2, 2);

        Assert.All(mesh.Vertices, vertex => AssertVector(Normal(vertex), Vector3d.UnitZ));
    }

    [Fact]
    public void X_slope_uses_cell_size_and_tilts_against_rising_height()
    {
        var mesh = Build([0, 2, 4, 0, 2, 4, 0, 2, 4], 3, 3, 2);

        AssertVector(Normal(mesh.Vertices[4]), new Vector3d(-1, 0, 1).Normalize());
    }

    [Fact]
    public void Y_slope_tilts_against_rising_height()
    {
        var mesh = Build([0, 0, 0, 2, 2, 2, 4, 4, 4], 3, 3, 2);

        AssertVector(Normal(mesh.Vertices[4]), new Vector3d(0, -1, 1).Normalize());
    }

    [Fact]
    public void Normals_are_normalized_and_finite_at_boundaries_and_corners()
    {
        var mesh = Build([0, 1, 2, 3, 4, 5, 6, 7, 8], 3, 3);

        Assert.All(mesh.Vertices, vertex =>
        {
            var normal = Normal(vertex);
            Assert.True(double.IsFinite(normal.X));
            Assert.True(double.IsFinite(normal.Y));
            Assert.True(double.IsFinite(normal.Z));
            Assert.Equal(1, normal.Length, 10);
        });
        AssertVector(Normal(mesh.Vertices[0]), Normal(mesh.Vertices[2]));
        AssertVector(Normal(mesh.Vertices[0]), Normal(mesh.Vertices[6]));
    }

    [Fact]
    public void Visual_height_scale_changes_slope_response()
    {
        var mesh = Build([0, 2, 4, 0, 2, 4, 0, 2, 4], 3, 3, 2,
            new TerrainRenderTransform(2));

        AssertVector(Normal(mesh.Vertices[4]), new Vector3d(-2, 0, 1).Normalize());
    }

    [Fact]
    public void Minimum_legal_grid_builds_finite_normals()
    {
        var mesh = Build([1, 2, 3, 4], 2, 2);

        Assert.Equal(4, mesh.Vertices.Count);
        Assert.All(mesh.Vertices, vertex => Assert.True(double.IsFinite(Normal(vertex).Length)));
    }

    [Fact]
    public void NoData_neighbor_uses_a_safe_fallback_gradient()
    {
        var mesh = Build([0, 1, 2, 0, 3, 4, 5, 6, 7], 3, 3, mask: [false, false, false, true, false, false, false, false, false]);

        Assert.True(double.IsFinite(Normal(mesh.Vertices[4]).Length));
    }

    static TerrainMesh Build(IReadOnlyList<double> heights, int width, int height,
        double cellSize = 1, TerrainRenderTransform? transform = null, IReadOnlyList<bool>? mask = null)
    {
        var field = new TerrainHeightfield(width, height, heights, mask ?? new bool[heights.Count],
            TerrainRenderMetadata.Empty, cellSize);
        return TerrainMeshBuilder.Build(new("test", 1, field, cellSize),
            transform ?? TerrainRenderTransform.Default);
    }

    static void AssertVector(Vector3d actual, Vector3d expected) =>
        Assert.Equal(expected, actual, new Vector3dComparer(1e-10));

    static Vector3d Normal(TerrainMeshVertex vertex) => new(vertex.Nx, vertex.Ny, vertex.Nz);

    sealed class Vector3dComparer(double tolerance) : IEqualityComparer<Vector3d>
    {
        public bool Equals(Vector3d x, Vector3d y) =>
            Math.Abs(x.X - y.X) <= tolerance && Math.Abs(x.Y - y.Y) <= tolerance && Math.Abs(x.Z - y.Z) <= tolerance;

        public int GetHashCode(Vector3d obj) => 0;
    }
}

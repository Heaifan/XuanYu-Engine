using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainRenderContractTests
{
    [Fact]
    public void Heightfield_preserves_positive_zero_and_negative_logic()
    {
        var heightfield = new TerrainHeightfield(2, 2, [100, 0, -25, 50]);

        Assert.Equal(100, heightfield.ElevationAt(0, 0));
        Assert.Equal(0, heightfield.ElevationAt(0, 1));
        Assert.Equal(-25, heightfield.ElevationAt(1, 0));
    }

    [Fact]
    public void Presentation_transform_applies_default_exaggeration_only_to_visual_height()
    {
        var transform = TerrainRenderTransform.Default;

        Assert.Equal(100, transform.VisualHeight(100));
        Assert.Equal(-25, transform.VisualHeight(-25));
        Assert.Equal(100, transform.LogicalHeight(100));
    }

    [Fact]
    public void Mesh_builder_generates_indexed_grid_without_deleting_height_sign()
    {
        var resource = new TerrainRenderResource("taiwan-r0", 1,
            new TerrainHeightfield(2, 2, [100, 0, -25, 50]));

        var mesh = TerrainMeshBuilder.Build(resource, TerrainRenderTransform.Default);

        Assert.Equal(4, mesh.Vertices.Count);
        Assert.Equal(6, mesh.Indices.Count);
        Assert.Equal(100, mesh.Vertices[0].Z);
        Assert.Equal(0, mesh.Vertices[1].Z);
        Assert.Equal(-25, mesh.Vertices[2].Z);
        Assert.Equal(50, resource.Heightfield.ElevationAt(1, 1));
    }

    [Fact]
    public void Mesh_builder_keeps_world_origin_out_of_tile_local_vertices()
    {
        var resource = new TerrainRenderResource("origin", 1,
            new TerrainHeightfield(2, 2, [1, 2, 3, 4]), 1000,
            new(100000, 0, 0));

        var mesh = TerrainMeshBuilder.Build(resource, TerrainRenderTransform.Default);

        Assert.Equal(0, mesh.Vertices[0].X);
        Assert.Equal(0, mesh.Vertices[0].Y);
        Assert.Equal(1000, mesh.Vertices[1].X);
    }

    [Fact]
    public void Adjacent_tile_world_range_applies_origin_once_at_instance_boundary()
    {
        var resource = new TerrainRenderResource("adjacent", 1,
            new TerrainHeightfield(2, 2, [1, 2, 3, 4]), 100000,
            new(100000, 0, 0));

        var mesh = TerrainMeshBuilder.Build(resource, TerrainRenderTransform.Default);
        var localMin = mesh.Vertices.Min(vertex => vertex.X);
        var localMax = mesh.Vertices.Max(vertex => vertex.X);

        Assert.Equal(0, localMin);
        Assert.Equal(100000, localMax);
        Assert.Equal(100000, resource.WorldOrigin.X + localMin);
        Assert.Equal(200000, resource.WorldOrigin.X + localMax);
    }

    [Fact]
    public void Projection_and_draw_plan_expose_terrain_as_a_separate_pass()
    {
        var terrain = new TerrainRenderResource("r0", 1, new TerrainHeightfield(2, 2, [1, 2, 3, 4]));
        var projection = new RenderProjection(default, [], false, default, Terrain: terrain);

        Assert.True(projection.HasTerrain);
        Assert.Contains(RenderDrawPlan.GetFrameDrawPlan(projection), x => x.Kind == RenderDrawKind.Terrain);
    }
}

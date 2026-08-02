using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR4D3F1BaseVertexTests
{
    readonly GlbImportService _import = new();

    [Fact]
    public void Multi_primitive_glb_normalizes_indices_and_base_vertex()
    {
        var model = Import(WorldCR4D1GlbFactory.MultiPrimitive());

        Assert.Equal(6, model.Vertices.Count);
        Assert.Equal([0u, 1u, 2u, 3u, 4u, 5u], model.Indices);
        Assert.All(model.Primitives, p => Assert.Equal(0, p.BaseVertex));
        Assert.All(model.Indices, i => Assert.True(i < model.Vertices.Count));
    }

    [Fact]
    public void Three_primitives_all_normalized()
    {
        var model = Import(WorldCR4D1GlbFactory.ThreePrimitives());

        Assert.Equal(9, model.Vertices.Count);
        Assert.Equal(3, model.Primitives.Count);
        Assert.Equal(Enumerable.Range(0, 9).Select(x => (uint)x), model.Indices);
        Assert.All(model.Primitives, p => Assert.Equal(0, p.BaseVertex));
    }

    [Fact]
    public void Unindexed_primitive_gets_sequential_global_indices()
    {
        var model = Import(WorldCR4D1GlbFactory.Triangle(indices: false));

        Assert.Equal(3, model.Vertices.Count);
        Assert.Equal([0u, 1u, 2u], model.Indices);
        Assert.Single(model.Primitives);
        Assert.Equal(0, model.Primitives[0].BaseVertex);
    }

    [Fact]
    public void Single_primitive_behavior_is_unchanged()
    {
        var model = Import(WorldCR4D1GlbFactory.Triangle());

        Assert.Equal(3, model.Vertices.Count);
        Assert.Equal([0u, 1u, 2u], model.Indices);
        Assert.Single(model.Primitives);
        Assert.Equal((0, 3, 0),
            (model.Primitives[0].FirstIndex, model.Primitives[0].IndexCount, model.Primitives[0].BaseVertex));
    }

    static StaticModelData Import(byte[] glb) =>
        new GlbImportService().ImportBytes(glb, "t.glb").Model!;
}

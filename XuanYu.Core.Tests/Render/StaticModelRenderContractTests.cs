using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.Assets;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class StaticModelRenderContractTests
{
    [Fact]
    public void D1_static_model_maps_to_render_resource_without_third_party_types()
    {
        var model = Model();
        var key = new RenderStaticModelKey("d2:test");

        var resource = StaticModelRenderAdapter.ToRenderResource(model, key, 7);

        Assert.Equal(key, resource.Key);
        Assert.Equal(7, resource.Revision);
        Assert.Equal(model.Vertices.Count, resource.Vertices.Count);
        Assert.Equal(model.Indices, resource.Indices);
        Assert.Equal(model.LocalBounds, resource.LocalBounds);
    }

    [Fact]
    public void Primitive_ranges_and_base_colors_are_preserved()
    {
        var resource = StaticModelRenderAdapter.ToRenderResource(
            Model(), new RenderStaticModelKey("d2:multi"), 1);

        Assert.Equal(2, resource.Primitives.Count);
        Assert.Equal((0, 3, 0), Range(resource.Primitives[0]));
        Assert.Equal((3, 3, 0), Range(resource.Primitives[1]));
        Assert.Equal(new RenderStaticModelColor(0.9, 0.2, 0.1, 1), resource.Primitives[0].BaseColor);
        Assert.Equal(new RenderStaticModelColor(0.1, 0.6, 0.9, 1), resource.Primitives[1].BaseColor);
    }

    [Fact]
    public void Static_model_frame_plan_uses_model_fill_not_legacy_placeholder()
    {
        var key = new RenderStaticModelKey("d2:model");
        var entity = new RenderEntityProjection(EntityId.FromInt(3), Vector3d.Zero,
            Vector3d.Zero, new(1, 1, 1), true, RenderEntityType.StaticModel, key);

        var plan = RenderDrawPlan.GetFrameDrawPlan(new RenderProjection(default, [entity], false, Vector3d.Zero));

        Assert.Single(plan);
        Assert.Equal(RenderDrawKind.EntityFill, plan[0].Kind);
        Assert.Equal(RenderEntityType.StaticModel, plan[0].EntityType);
        Assert.Equal(0, plan[0].VertexCount);
    }

    static (int, int, int) Range(RenderStaticModelPrimitive p) =>
        (p.FirstIndex, p.IndexCount, p.BaseVertex);

    static StaticModelData Model() => new(
        [
            Vertex(0, 0, 0), Vertex(1, 0, 0), Vertex(0, 1, 0),
            Vertex(0, 0, 1), Vertex(1, 0, 1), Vertex(0, 1, 1)
        ],
        [0, 1, 2, 3, 4, 5],
        [
            new(0, 3, 0, new StaticModelColor(0.9, 0.2, 0.1, 1)),
            new(3, 3, 0, new StaticModelColor(0.1, 0.6, 0.9, 1))
        ],
        new SpatialAabb(Vector3d.Zero, new(1, 1, 1)),
        new StaticModelImportMetadata("d2", "WORLD-C-R4-D1", 1),
        []);

    static StaticModelVertex Vertex(double x, double y, double z) =>
        new(new Vector3d(x, y, z), Vector3d.UnitZ, StaticModelUv.Zero);
}

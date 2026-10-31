using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.Assets;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

static class D2StaticModelDemo
{
    static readonly RenderStaticModelKey Key = new("WORLD-C-R4-D2:demo-static-model");
    static readonly RenderStaticModelResource Resource =
        StaticModelRenderAdapter.ToRenderResource(Model(), Key, 1);

    public static RenderProjectionResult Apply(RenderProjectionResult source)
    {
        if (!source.Success || Environment.GetEnvironmentVariable("XUANYU_D2_STATIC_MODEL_DEMO") != "1")
            return source;
        var p = source.Projection;
        if (p.Entities.Count == 0) return source;
        var entities = p.Entities
            .Select(e => e with { EntityType = RenderEntityType.StaticModel, StaticModelKey = Key })
            .ToArray();
        return RenderProjectionResult.Ok(p with { Entities = entities, StaticModels = [Resource] });
    }

    static StaticModelData Model() => new(
        [
            Vertex(-0.8, -0.4, 0), Vertex(0.8, -0.4, 0), Vertex(-0.25, 0.8, 0.35),
            Vertex(0.8, -0.4, 0), Vertex(0.8, 0.55, 0.05), Vertex(-0.25, 0.8, 0.35)
        ],
        [0, 1, 2, 3, 4, 5],
        [
            new(0, 3, 0, new StaticModelColor(0.86, 0.36, 0.22, 1)),
            new(3, 3, 0, new StaticModelColor(0.24, 0.58, 0.82, 1))
        ],
        new SpatialAabb(new(-0.8, -0.4, 0), new(0.8, 0.8, 0.35)),
        new StaticModelImportMetadata("WORLD-C-R4-D2 demo", "WORLD-C-R4-D1", 1),
        []);

    static StaticModelVertex Vertex(double x, double y, double z) =>
        new(new Vector3d(x, y, z), new Vector3d(0, -0.35, 0.94).Normalize(), StaticModelUv.Zero);
}

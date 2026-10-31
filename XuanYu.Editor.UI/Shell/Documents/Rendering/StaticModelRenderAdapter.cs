using XuanYu.Editor.Assets;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public static class StaticModelRenderAdapter
{
    public static RenderStaticModelResource ToRenderResource(
        StaticModelData model,
        RenderStaticModelKey key,
        int revision)
    {
        var vertices = model.Vertices.Select(v => new RenderStaticModelVertex(
            v.Position, v.Normal, v.Uv0.U, v.Uv0.V)).ToArray();
        var primitives = model.Primitives.Select(p => new RenderStaticModelPrimitive(
            p.FirstIndex, p.IndexCount, p.BaseVertex, new RenderStaticModelColor(
                p.BaseColorFactor.R, p.BaseColorFactor.G,
                p.BaseColorFactor.B, p.BaseColorFactor.A))).ToArray();
        return new RenderStaticModelResource(
            key, revision, vertices, model.Indices.ToArray(), primitives, model.LocalBounds);
    }
}

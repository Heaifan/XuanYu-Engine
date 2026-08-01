using XuanYu.Core.Spatial;

namespace XuanYu.Render.Abstractions;

public sealed record RenderStaticModelResource(
    RenderStaticModelKey Key,
    int Revision,
    IReadOnlyList<RenderStaticModelVertex> Vertices,
    IReadOnlyList<uint> Indices,
    IReadOnlyList<RenderStaticModelPrimitive> Primitives,
    SpatialAabb LocalBounds);

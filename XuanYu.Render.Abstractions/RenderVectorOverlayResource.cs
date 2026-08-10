using XuanYu.Core.Spatial;

namespace XuanYu.Render.Abstractions;

public sealed record RenderVectorOverlayResource(
    RenderVectorOverlayKey Key,
    int Revision,
    IReadOnlyList<RenderVectorOverlayVertex> Vertices,
    IReadOnlyList<uint> Indices,
    IReadOnlyList<RenderVectorOverlayPrimitive> Primitives,
    SpatialAabb WorldBounds);

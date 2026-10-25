using XuanYu.Core.Spatial;

namespace XuanYu.Render.Abstractions;

public sealed record RenderVectorOverlayResource(
    RenderVectorOverlayKey Key,
    int Revision,
    IReadOnlyList<RenderVectorOverlayVertex> Vertices,
    IReadOnlyList<uint> Indices,
    IReadOnlyList<RenderVectorOverlayPrimitive> Primitives,
    SpatialAabb WorldBounds,
    IReadOnlyList<RenderVectorOverlayLabel>? Labels = null,
    IReadOnlyList<RenderLabelBitmap>? LabelBitmaps = null)
{
    public IReadOnlyList<RenderVectorOverlayLabel> LabelInstances => Labels ?? [];
    public IReadOnlyList<RenderLabelBitmap> LabelBitmapResources => LabelBitmaps ?? [];
}

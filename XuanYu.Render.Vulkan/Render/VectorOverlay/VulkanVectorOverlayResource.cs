using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

sealed class VulkanVectorOverlayResource : IDisposable
{
    public VulkanVectorOverlayResource(RenderVectorOverlayKey key, int revision,
        VulkanStaticModelBuffer vertices, VulkanStaticModelBuffer indices,
        IReadOnlyList<RenderVectorOverlayPrimitive> primitives,
        IReadOnlyList<RenderVectorOverlayLabel> labels,
        IReadOnlyList<RenderLabelBitmap> labelBitmaps)
    {
        Key = key; Revision = revision; VertexBuffer = vertices; IndexBuffer = indices;
        Primitives = primitives;
        Labels = labels; LabelBitmaps = labelBitmaps;
    }

    public RenderVectorOverlayKey Key { get; }
    public int Revision { get; private set; }
    public VulkanStaticModelBuffer VertexBuffer { get; }
    public VulkanStaticModelBuffer IndexBuffer { get; }
    public IReadOnlyList<RenderVectorOverlayPrimitive> Primitives { get; private set; }
    public IReadOnlyList<RenderVectorOverlayLabel> Labels { get; private set; }
    public IReadOnlyList<RenderLabelBitmap> LabelBitmaps { get; private set; }

    public void Update(int revision, IReadOnlyList<RenderVectorOverlayPrimitive> primitives,
        IReadOnlyList<RenderVectorOverlayLabel> labels, IReadOnlyList<RenderLabelBitmap> labelBitmaps)
    { Revision = revision; Primitives = primitives; Labels = labels; LabelBitmaps = labelBitmaps; }

    public void Dispose() { VertexBuffer.Dispose(); IndexBuffer.Dispose(); }
}

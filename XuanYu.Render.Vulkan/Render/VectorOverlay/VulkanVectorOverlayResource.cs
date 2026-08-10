using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

sealed class VulkanVectorOverlayResource : IDisposable
{
    public VulkanVectorOverlayResource(RenderVectorOverlayKey key, int revision,
        VulkanStaticModelBuffer vertices, VulkanStaticModelBuffer indices,
        IReadOnlyList<RenderVectorOverlayPrimitive> primitives)
    {
        Key = key; Revision = revision; VertexBuffer = vertices; IndexBuffer = indices;
        Primitives = primitives;
    }

    public RenderVectorOverlayKey Key { get; }
    public int Revision { get; private set; }
    public VulkanStaticModelBuffer VertexBuffer { get; }
    public VulkanStaticModelBuffer IndexBuffer { get; }
    public IReadOnlyList<RenderVectorOverlayPrimitive> Primitives { get; private set; }

    public void Update(int revision, IReadOnlyList<RenderVectorOverlayPrimitive> primitives)
    { Revision = revision; Primitives = primitives; }

    public void Dispose() { VertexBuffer.Dispose(); IndexBuffer.Dispose(); }
}

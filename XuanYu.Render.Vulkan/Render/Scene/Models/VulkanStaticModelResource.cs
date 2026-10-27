using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

sealed class VulkanStaticModelResource : IDisposable
{
    public VulkanStaticModelResource(
        RenderStaticModelKey key,
        int revision,
        VulkanStaticModelBuffer vertices,
        VulkanStaticModelBuffer indices,
        IReadOnlyList<RenderStaticModelPrimitive> primitives)
    {
        Key = key; Revision = revision; VertexBuffer = vertices;
        IndexBuffer = indices; Primitives = primitives;
    }

    public RenderStaticModelKey Key { get; }
    public int Revision { get; }
    public VulkanStaticModelBuffer VertexBuffer { get; }
    public VulkanStaticModelBuffer IndexBuffer { get; }
    public IReadOnlyList<RenderStaticModelPrimitive> Primitives { get; }
    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}

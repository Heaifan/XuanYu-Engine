using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

sealed class VulkanVectorOverlayCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    readonly Dictionary<RenderVectorOverlayKey, VulkanVectorOverlayResource> _items = [];

    public VulkanVectorOverlayCache(Vk vk, VulkanDeviceOwner device, Action<string>? log) =>
        (_vk, _device, _log) = (vk, device, log);

    public VulkanVectorOverlayResource? Get(RenderVectorOverlayResource model)
    {
        if (_items.TryGetValue(model.Key, out var old) && old.Revision == model.Revision) return old;
        if (!VulkanVectorOverlayValidator.Validate(model, out var error))
        { _log?.Invoke($"Vector Overlay 资源校验失败：{error}"); return old; }
        var vertices = model.Vertices.Select(VulkanVectorOverlayVertex.From).ToArray();
        var indices = model.Indices.ToArray();
        var vertexBytes = vertices.Length * (int)VulkanVectorOverlayVertex.Stride;
        var indexBytes = indices.Length * sizeof(uint);
        if (old is not null && VulkanVectorOverlayBufferReusePolicy.CanReuse(old.VertexBuffer.CapacityBytes, vertexBytes)
            && VulkanVectorOverlayBufferReusePolicy.CanReuse(old.IndexBuffer.CapacityBytes, indexBytes)
            && old.VertexBuffer.TryUpdate(vertices) && old.IndexBuffer.TryUpdate(indices))
        { old.Update(model.Revision, model.Primitives.ToArray()); return old; }
        var vb = VulkanStaticModelBuffer.Create(_vk, _device, vertices, BufferUsageFlags.VertexBufferBit, out error);
        if (vb is null) { _log?.Invoke($"Vector Overlay 顶点缓冲创建失败：{error}"); return old; }
        var ib = VulkanStaticModelBuffer.Create(_vk, _device, indices, BufferUsageFlags.IndexBufferBit, out error);
        if (ib is null) { vb.Dispose(); _log?.Invoke($"Vector Overlay 索引缓冲创建失败：{error}"); return old; }
        var next = new VulkanVectorOverlayResource(model.Key, model.Revision, vb, ib, model.Primitives.ToArray());
        _items[model.Key] = next; old?.Dispose(); return next;
    }

    public void RetainOnly(IEnumerable<RenderVectorOverlayKey> keys)
    {
        var keep = keys.ToHashSet();
        foreach (var key in _items.Keys.Where(k => !keep.Contains(k)).ToArray())
        { _items[key].Dispose(); _items.Remove(key); }
    }

    public void Dispose() { foreach (var item in _items.Values) item.Dispose(); _items.Clear(); }
}

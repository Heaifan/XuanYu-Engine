using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

sealed class VulkanStaticModelCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    readonly Dictionary<RenderStaticModelKey, VulkanStaticModelResource> _items = [];
    readonly VulkanStaticModelFailureTracker _failures = new();
    public VulkanStaticModelCache(Vk vk, VulkanDeviceOwner device, Action<string>? log) =>
        (_vk, _device, _log) = (vk, device, log);

    public VulkanStaticModelResource? Get(RenderStaticModelResource model)
    {
        if (_items.TryGetValue(model.Key, out var old) && old.Revision == model.Revision) return old;
        if (_failures.ShouldSkip(model.Key, model.Revision)) return old;
        if (!TryCreate(model, out var next, out var reason))
        {
            _failures.Record(model.Key, model.Revision);
            _log?.Invoke(VulkanStaticModelLog.Failed(model.Key, "Create", reason));
            return old;
        }
        _items[model.Key] = next!;
        old?.Dispose();
        _log?.Invoke(VulkanStaticModelLog.Created(model));
        return next;
    }

    public void RetainOnly(IEnumerable<RenderStaticModelKey> keys)
    {
        var keep = keys.Where(k => k.IsValid).ToHashSet();
        foreach (var key in _items.Keys.Where(k => !keep.Contains(k)).ToArray())
        {
            _items[key].Dispose();
            _items.Remove(key);
            _log?.Invoke(VulkanStaticModelLog.Disposed(key));
        }

        _failures.ClearNotIn(keys);
    }

    bool TryCreate(RenderStaticModelResource m, out VulkanStaticModelResource? resource, out string error)
    {
        resource = null;
        if (!VulkanStaticModelValidator.Validate(m, out error)) return false;
        var vertices = m.Vertices.Select(VulkanStaticModelVertex.From).ToArray();
        var vb = VulkanStaticModelBuffer.Create(_vk, _device, vertices,
            BufferUsageFlags.VertexBufferBit, out error);
        if (vb is null) return false;
        var ib = VulkanStaticModelBuffer.Create(_vk, _device, m.Indices.ToArray(),
            BufferUsageFlags.IndexBufferBit, out error);
        if (ib is null) { vb.Dispose(); return false; }
        resource = new VulkanStaticModelResource(m.Key, m.Revision, vb, ib,
            m.Primitives.ToArray(), checked((uint)m.Indices.Count));
        return true;
    }

    public void Dispose()
    {
        foreach (var item in _items.Values) item.Dispose();
        _items.Clear();
    }
}

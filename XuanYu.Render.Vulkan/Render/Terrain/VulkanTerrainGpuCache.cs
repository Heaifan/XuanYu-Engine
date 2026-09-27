using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed class VulkanTerrainGpuCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    readonly Dictionary<string, VulkanTerrainGpuResource> _resources = new(StringComparer.Ordinal);
    readonly Dictionary<string, (int Revision, double Exaggeration)> _signatures = new(StringComparer.Ordinal);
    public VulkanTerrainGpuCache(Vk vk, VulkanDeviceOwner device, Action<string>? log) =>
        (_vk, _device, _log) = (vk, device, log);

    public VulkanTerrainGpuResource? GetOrCreate(TerrainRenderResource resource, TerrainRenderTransform transform)
    {
        if (_resources.TryGetValue(resource.TerrainId, out var current) &&
            _signatures[resource.TerrainId] == (resource.Revision, transform.VerticalExaggeration))
            return current;
        var next = VulkanTerrainGpuResource.Create(_vk, _device, resource, transform, out var error);
        if (next is null) { _log?.Invoke($"Terrain GPU 资源创建失败：{error}"); return current; }
        if (_resources.Remove(resource.TerrainId, out var replaced)) replaced.Dispose();
        _resources[resource.TerrainId] = next;
        _signatures[resource.TerrainId] = (resource.Revision, transform.VerticalExaggeration);
        _log?.Invoke($"Terrain GPU 资源创建完成：{resource.TerrainId}；索引={next.IndexCount}；夸张={transform.VerticalExaggeration:0.###}");
        return next;
    }

    public void RetainOnly(IEnumerable<string> terrainIds)
    {
        var retained = terrainIds.ToHashSet(StringComparer.Ordinal);
        foreach (var id in _resources.Keys.Where(id => !retained.Contains(id)).ToArray())
        {
            _resources[id].Dispose(); _resources.Remove(id); _signatures.Remove(id);
        }
    }

    public void Dispose()
    {
        foreach (var resource in _resources.Values) resource.Dispose();
        _resources.Clear(); _signatures.Clear();
    }
}

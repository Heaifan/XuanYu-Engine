using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed class VulkanTerrainGpuCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    VulkanTerrainGpuResource? _current;
    string _id = ""; int _revision; double _exaggeration;
    public VulkanTerrainGpuCache(Vk vk, VulkanDeviceOwner device, Action<string>? log) =>
        (_vk, _device, _log) = (vk, device, log);

    public VulkanTerrainGpuResource? GetOrCreate(TerrainRenderResource resource, TerrainRenderTransform transform)
    {
        if (_current is not null && _id == resource.TerrainId && _revision == resource.Revision && _exaggeration == transform.VerticalExaggeration)
            return _current;
        var next = VulkanTerrainGpuResource.Create(_vk, _device, resource, transform, out var error);
        if (next is null) { _log?.Invoke($"Terrain GPU 资源创建失败：{error}"); return _current; }
        _current?.Dispose(); _current = next; _id = resource.TerrainId;
        _revision = resource.Revision; _exaggeration = transform.VerticalExaggeration;
        _log?.Invoke($"Terrain GPU 资源创建完成：{resource.TerrainId}；索引={next.IndexCount}；夸张={transform.VerticalExaggeration:0.###}");
        return next;
    }

    public void Dispose() { _current?.Dispose(); _current = null; }
}

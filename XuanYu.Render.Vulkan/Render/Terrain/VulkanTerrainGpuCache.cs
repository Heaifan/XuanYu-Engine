using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed class VulkanTerrainGpuCache : IDisposable
{
    public const ulong DefaultBudgetBytes = 256UL * 1024 * 1024;
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    readonly Dictionary<TerrainChunkGpuKey, VulkanTerrainGpuResource> _resources = [];
    readonly LinkedList<TerrainChunkGpuKey> _lru = [];
    readonly Dictionary<string, IReadOnlyList<TerrainChunkDescriptor>> _chunks = [];
    int _meshBuilds; int _vertexUploads; int _indexUploads; int _bufferCreates;
    public VulkanTerrainGpuCache(Vk vk, VulkanDeviceOwner device, Action<string>? log) =>
        (_vk, _device, _log) = (vk, device, log);

    public IReadOnlyList<TerrainChunkDescriptor> GetChunks(TerrainRenderResource resource)
    {
        var key = $"{resource.TerrainId}:{resource.Revision}";
        return _chunks.TryGetValue(key, out var chunks) ? chunks :
            _chunks[key] = TerrainChunkPartitioner.Partition(
                resource.Heightfield, resource.TerrainId, resource.Revision);
    }

    public VulkanTerrainGpuResource? GetOrCreate(TerrainRenderResource resource,
        TerrainChunkDescriptor chunk, TerrainLodLevel lod, TerrainRenderTransform transform)
    {
        var key = new TerrainChunkGpuKey(resource.TerrainId, resource.Revision,
            transform.VerticalExaggeration, chunk.ChunkX, chunk.ChunkY, lod);
        if (_resources.TryGetValue(key, out var current)) { Touch(key); return current; }
        var next = VulkanTerrainGpuResource.Create(_vk, _device, resource, chunk, lod, transform, out var error);
        if (next is null) { _log?.Invoke($"Terrain Chunk GPU 资源创建失败：{error}"); return null; }
        _resources[key] = next; _lru.AddLast(key);
        _meshBuilds++; _vertexUploads++; _indexUploads++; _bufferCreates += 2;
        Trim(key);
        return next;
    }

    internal static bool BelongsTo(TerrainChunkGpuKey key, TerrainRenderResource resource) =>
        key.TerrainId == resource.TerrainId && key.Revision == resource.Revision;

    void Touch(TerrainChunkGpuKey key)
    {
        var node = _lru.Find(key);
        if (node is null || node == _lru.Last) return;
        _lru.Remove(node); _lru.AddLast(node);
    }

    void Trim(TerrainChunkGpuKey current)
    {
        while (ResidentGpuBytes > DefaultBudgetBytes && _resources.Count > 1)
        {
            var key = _lru.First!.Value;
            if (key == current) { Touch(key); continue; }
            _lru.RemoveFirst(); _resources[key].Dispose(); _resources.Remove(key);
        }
    }

    public ulong ResidentGpuBytes => (ulong)_resources.Values.Sum(x => (long)x.GpuBytes);
    public (int Mesh, int Vertex, int Index, int Buffers) CreationCounts =>
        (_meshBuilds, _vertexUploads, _indexUploads, _bufferCreates);

    public void RetainOnly(IEnumerable<TerrainRenderResource> resources)
    {
        var retained = resources.ToDictionary(x => x.TerrainId, x => x.Revision, StringComparer.Ordinal);
        foreach (var key in _resources.Keys.Where(x =>
            !retained.TryGetValue(x.TerrainId, out var revision) || revision != x.Revision).ToArray())
        { _resources[key].Dispose(); _resources.Remove(key); _lru.Remove(key); }
        foreach (var key in _chunks.Keys.Where(key =>
            !retained.TryGetValue(key.Split(':')[0], out var revision) ||
            !key.EndsWith($":{revision}", StringComparison.Ordinal)).ToArray())
            _chunks.Remove(key);
    }

    public void Dispose()
    {
        foreach (var resource in _resources.Values) resource.Dispose();
        _resources.Clear(); _lru.Clear(); _chunks.Clear();
    }
}

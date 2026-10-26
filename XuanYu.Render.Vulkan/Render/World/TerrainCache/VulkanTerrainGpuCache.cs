using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed partial class VulkanTerrainGpuCache : IDisposable
{
    public const ulong DefaultBudgetBytes = 256UL * 1024 * 1024;
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly Action<string>? _log;
    readonly Action<IDisposable, Action?>? _retire;
    readonly Dictionary<TerrainChunkGpuKey, VulkanTerrainGpuResource> _resources = [];
    readonly LinkedList<TerrainChunkGpuKey> _lru = [];
    readonly Dictionary<string, IReadOnlyList<TerrainChunkDescriptor>> _chunks = [];
    int _meshBuilds; int _vertexUploads; int _indexUploads; int _bufferCreates;
    public double MeshBuildMilliseconds { get; private set; }
    public double UploadMilliseconds { get; private set; }
    public VulkanTerrainGpuCache(Vk vk, VulkanDeviceOwner device, Action<string>? log,
        ulong budgetBytes = DefaultBudgetBytes, Action<IDisposable, Action?>? retire = null)
    {
        if (budgetBytes == 0) throw new ArgumentOutOfRangeException(nameof(budgetBytes));
        (_vk, _device, _log, _budgetBytes, _retire) = (vk, device, log, budgetBytes, retire);
    }

    public IReadOnlyList<TerrainChunkDescriptor> GetChunks(TerrainRenderResource resource)
    {
        var cells = XuanYu.Core.Diagnostics.TerrainReplayOptions.Enabled
            ? XuanYu.Core.Diagnostics.TerrainReplayOptions.ChunkCellsOverride
            : TerrainChunkPartitioner.CellsPerChunk;
        var key = $"{resource.TerrainId}:{resource.Revision}";
        return _chunks.TryGetValue(key, out var chunks) ? chunks :
            _chunks[key] = TerrainChunkPartitioner.Partition(
                resource.Heightfield, resource.TerrainId, resource.Revision, cells);
    }

    public VulkanTerrainGpuResource? GetOrCreate(TerrainRenderResource resource,
        TerrainChunkDescriptor chunk, TerrainLodLevel lod, TerrainRenderTransform transform)
    {
        var key = new TerrainChunkGpuKey(resource.TerrainId, resource.Revision,
            transform.VerticalExaggeration, chunk.ChunkX, chunk.ChunkY, lod);
        if (_resources.TryGetValue(key, out var current))
        { RecordResource(current); Touch(key); return current; }
        var next = VulkanTerrainGpuResource.Create(_vk, _device, resource, chunk, lod, transform,
            out var error, out var meshMs, out var uploadMs);
        MeshBuildMilliseconds += meshMs; UploadMilliseconds += uploadMs;
        if (next is null)
        {
            if (error != "NO_TRIANGLES")
                _log?.Invoke($"Terrain Chunk GPU 资源创建失败：{error}");
            return null;
        }
        _resources[key] = next; _lru.AddLast(key);
        _meshBuilds++; _vertexUploads++; _indexUploads++; _bufferCreates += 2;
        RecordResource(next);
        Trim(key);
        return next;
    }

    void Trim(TerrainChunkGpuKey current)
    {
        while (CachedGpuBytes > _budgetBytes && _resources.Count > 1)
        {
            var key = _lru.First!.Value;
            if (key == current) { Touch(key); continue; }
            _lru.RemoveFirst();
            var resource = _resources[key];
            _resources.Remove(key);
            Retire(resource);
        }
    }

    internal static bool BelongsTo(TerrainChunkGpuKey key, TerrainRenderResource resource) =>
        key.TerrainId == resource.TerrainId && key.Revision == resource.Revision;

    void Touch(TerrainChunkGpuKey key)
    {
        var node = _lru.Find(key);
        if (node is null || node == _lru.Last) return;
        _lru.Remove(node); _lru.AddLast(node);
    }

    public (int Mesh, int Vertex, int Index, int Buffers) CreationCounts =>
        (_meshBuilds, _vertexUploads, _indexUploads, _bufferCreates);
    internal string RetirementDiagnostics =>
        $"entries={_resources.Count};recordedReferences={_recordedResources.Count};" +
        $"recordingReferences={_recordingResources?.Count ?? 0};cacheRetired={_retired.Count};" +
        $"disposed={_disposedResourceCount};residentBytes={ResidentGpuBytes}";

    // Keep the established cache entry point in this source file; lifecycle
    // bookkeeping remains implemented by the retirement partial.
    public void RetainOnly(IEnumerable<TerrainRenderResource> resources) => RetainOnlyCore(resources);
}

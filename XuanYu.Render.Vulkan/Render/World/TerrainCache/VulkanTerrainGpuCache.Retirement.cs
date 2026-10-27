using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed partial class VulkanTerrainGpuCache
{
    readonly ulong _budgetBytes;
    readonly HashSet<VulkanTerrainGpuResource> _recordedResources = [];
    readonly HashSet<VulkanTerrainGpuResource> _retired = [];
    HashSet<VulkanTerrainGpuResource>? _recordingResources;
    int _disposedResourceCount;

    public int RetiredGpuResourceCount => _retired.Count;
    public int DisposedGpuResourceCount => _disposedResourceCount;
    public ulong ResidentGpuBytes => CachedGpuBytes + (ulong)_retired.Sum(x => (long)x.GpuBytes);
    ulong CachedGpuBytes => (ulong)_resources.Values.Sum(x => (long)x.GpuBytes);

    internal void BeginCommandBufferRecording()
    {
        if (_recordingResources is not null)
            throw new InvalidOperationException("Terrain command buffer recording is already active.");
        _recordingResources = [];
    }

    internal void CompleteCommandBufferRecording()
    {
        if (_recordingResources is null)
            throw new InvalidOperationException("Terrain command buffer recording was not started.");
        _recordedResources.Clear();
        _recordedResources.UnionWith(_recordingResources);
        _recordingResources = null;
        DisposeUnrecordedRetired();
    }

    internal void CancelCommandBufferRecording()
    {
        _recordingResources = null;
        DisposeUnrecordedRetired();
    }

    void RecordResource(VulkanTerrainGpuResource resource) => _recordingResources?.Add(resource);

    void Retire(VulkanTerrainGpuResource resource)
    {
        _retired.Add(resource);
        DisposeUnrecordedRetired();
    }

    void DisposeUnrecordedRetired()
    {
        foreach (var resource in _retired.Where(x => !_recordedResources.Contains(x) &&
                     (_recordingResources is null || !_recordingResources.Contains(x))).ToArray())
        {
            if (_retire is null)
            {
                resource.Dispose();
                _disposedResourceCount++;
            }
            else _retire(resource, () => _disposedResourceCount++);
            _retired.Remove(resource);
        }
    }

    void RetainOnlyCore(IEnumerable<TerrainRenderResource> resources)
    {
        var retained = resources.ToDictionary(x => x.TerrainId, x => x.Revision, StringComparer.Ordinal);
        foreach (var key in _resources.Keys.Where(x =>
                     !retained.TryGetValue(x.TerrainId, out var revision) || revision != x.Revision).ToArray())
        {
            var resource = _resources[key];
            _resources.Remove(key); _lru.Remove(key);
            Retire(resource);
        }
        foreach (var key in _chunks.Keys.Where(key =>
                     !retained.TryGetValue(key.TerrainId, out var revision) || revision != key.Revision).ToArray())
            _chunks.Remove(key);
    }

    public void Dispose()
    {
        foreach (var resource in _resources.Values.Concat(_retired).Distinct())
        { resource.Dispose(); _disposedResourceCount++; }
        _resources.Clear(); _retired.Clear(); _recordedResources.Clear();
        _recordingResources = null; _lru.Clear(); _chunks.Clear();
    }
}

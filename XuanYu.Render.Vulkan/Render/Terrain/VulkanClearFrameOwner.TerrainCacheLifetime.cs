using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    ulong _terrainGpuCacheBudgetBytes = 256UL * 1024 * 1024;

    internal int RetiredTerrainGpuResourceCount => _terrainCache?.RetiredGpuResourceCount ?? 0;
    internal int DisposedTerrainGpuResourceCount => _terrainCache?.DisposedGpuResourceCount ?? 0;
    internal string TerrainRetirementDiagnostics =>
        $"cache[{_terrainCache?.RetirementDiagnostics ?? "not-created"}];" +
        _resourceRetirement.Describe();

    internal void SetTerrainGpuCacheBudgetForProbe(ulong budgetBytes)
    {
        if (_terrainCache is not null)
            throw new InvalidOperationException("Terrain cache budget must be set before its creation.");
        if (budgetBytes == 0) throw new ArgumentOutOfRangeException(nameof(budgetBytes));
        _terrainGpuCacheBudgetBytes = budgetBytes;
    }

    bool BeginTerrainCommandBufferRecording()
    {
        var hasTerrainDraw = _renderProjection.HasTerrain && _terrainPipeline.Handle != 0;
        if (_terrainCache is null && !hasTerrainDraw) return false;
        _terrainCache ??= new Terrain.VulkanTerrainGpuCache(_vk, _deviceOwner, _log,
            _terrainGpuCacheBudgetBytes, RetireTerrainGpuResource);
        _terrainCache.BeginCommandBufferRecording();
        if (!_renderProjection.HasTerrain) _terrainCache.RetainOnly([]);
        return true;
    }

    void CompleteTerrainCommandBufferRecording() => _terrainCache!.CompleteCommandBufferRecording();
    void CancelTerrainCommandBufferRecording() => _terrainCache!.CancelCommandBufferRecording();
}

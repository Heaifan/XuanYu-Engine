using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.Terrain.Offscreen;

public sealed unsafe partial class VulkanOffscreenTerrainRenderer
{
    public TerrainRenderStats TerrainStats => _frame?.TerrainStats ?? default;
    public double TerrainMeshBuildMilliseconds => _frame?.TerrainMeshBuildMilliseconds ?? 0;
    public int RetiredTerrainGpuResourceCount => _frame?.RetiredTerrainGpuResourceCount ?? 0;
    public int DisposedTerrainGpuResourceCount => _frame?.DisposedTerrainGpuResourceCount ?? 0;
    public string TerrainRetirementDiagnostics =>
        _frame?.TerrainRetirementDiagnostics ?? "frame-owner=unavailable";
}

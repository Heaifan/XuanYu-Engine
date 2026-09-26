using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _terrainPipeline;
    PipelineLayout _terrainPipelineLayout;
    Terrain.VulkanTerrainGpuCache? _terrainCache;

    public void SetTerrainPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _terrainPipeline = pipeline; _terrainPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Terrain Pipeline 注入后 CommandBuffer 重录失败");
    }

    internal void DisposeTerrain() { _terrainCache?.Dispose(); _terrainCache = null; }

    void DrawTerrain(CommandBuffer cb, float* scene)
    {
        if (_terrainPipeline.Handle == 0 || _terrainPipelineLayout.Handle == 0 || !_renderProjection.HasTerrain) return;
        var resource = _terrainCache ??= new Terrain.VulkanTerrainGpuCache(_vk, _deviceOwner, _log);
        var gpu = resource.GetOrCreate(_renderProjection.Terrain!, TerrainRenderTransform.Default);
        if (gpu is null) return;
        var vb = gpu.Vertices.Buffer; var ib = gpu.Indices.Buffer; ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
        FillScenePushConstants(scene, _renderProjection, default, default, new(1, 1, 1), 0, gizmoModeOverride: -16);
        PushSceneConstants(cb, scene); _vk.CmdDrawIndexed(cb, gpu.IndexCount, 1, 0, 0, 0);
        BindProceduralVertexBuffer(cb);
    }
}

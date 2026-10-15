using Silk.NET.Vulkan;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _terrainPipeline;
    PipelineLayout _terrainPipelineLayout;
    Terrain.VulkanTerrainGpuCache? _terrainCache;
    readonly TerrainLodStateCache _terrainLodStates = new();
    public TerrainRenderStats TerrainStats { get; private set; }
    public double TerrainMeshBuildMilliseconds => _terrainCache?.MeshBuildMilliseconds ?? 0;
    public double TerrainUploadMilliseconds => _terrainCache?.UploadMilliseconds ?? 0;

    public void SetTerrainPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _terrainPipeline = pipeline; _terrainPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Terrain Pipeline 注入后 CommandBuffer 重录失败");
    }

    internal void DisposeTerrain()
    { _terrainCache?.Dispose(); _terrainCache = null; _terrainLodStates.Clear(); }

    internal void RetainTerrainLodStates(IEnumerable<TerrainRenderResource> resources) =>
        _terrainLodStates.RetainOnly(resources.Select(x => (x.TerrainId, x.Revision)));

    void DrawTerrain(CommandBuffer cb, float* scene, int terrainIndex)
    {
        if (_terrainPipeline.Handle == 0 || _terrainPipelineLayout.Handle == 0 || !_renderProjection.HasTerrain) return;
        var cache = _terrainCache ??= new Terrain.VulkanTerrainGpuCache(_vk, _deviceOwner, _log);
        var resource = _renderProjection.TerrainResources[terrainIndex];
        var state = CurrentViewProjectionState();
        TerrainProbeLogger.Center(state, resource);
        var draws = new List<TerrainChunkDraw>(); var culled = 0;
        var probeChunks = 0;
        RetainTerrainLodStates(_renderProjection.TerrainResources);
        foreach (var chunk in cache.GetChunks(resource))
        {
            var transform = _renderProjection.EffectiveTerrainTransform;
            var skirtDrop = Math.Max(1, Math.Max(resource.Heightfield.CellSizeMeters,
                resource.Heightfield.CellSizeYMeters) * transform.VerticalExaggeration);
            var b = chunk.WorldBounds.WithVerticalExaggeration(transform, skirtDrop);
            var origin = resource.WorldOrigin;
            var bounds = new SpatialAabb(new(origin.X + b.MinX, origin.Y + b.MinY, b.MinZ),
                new(origin.X + b.MaxX, origin.Y + b.MaxY, b.MaxZ));
            var reject = TerrainFrustumCuller.TryGetRejectPlane(state, bounds, out var rejectPlane);
            if (TerrainReplayOptions.ShouldCull(!reject,
                TerrainReplayOptions.Enabled ? TerrainReplayOptions.Variant : TerrainReplayVariant.Baseline))
            {
                ProbeTerrainChunk(probeChunks++, state, chunk, bounds, origin, -1, false,
                    resource.Revision, rejectPlane, 0, "FRUSTUM_PLANE");
                culled++; continue;
            }
            var lodKey = new TerrainLodStateKey(resource.TerrainId, resource.Revision,
                chunk.ChunkX, chunk.ChunkY);
            var lodSelection = SelectReplayTerrainLod(lodKey, state, bounds,
                chunk, out var previousLod, out var lod);
            var gpu = cache.GetOrCreate(resource, chunk, lod, _renderProjection.EffectiveTerrainTransform);
            if (gpu is null)
            {
                ProbeTerrainChunk(probeChunks++, state, chunk, bounds, origin, (int)lod,
                    true, resource.Revision, -1, 0, "GPU_RESOURCE_UNAVAILABLE",
                    lodSelection, previousLod);
                continue;
            }
            var vb = gpu.Vertices.Buffer; var ib = gpu.Indices.Buffer; ulong offset = 0;
            _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset); _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
            FillScenePushConstants(scene, _renderProjection, origin, default, new(1, 1, 1), 0, -16);
            PushSceneConstants(cb, scene); _vk.CmdDrawIndexed(cb, gpu.IndexCount, 1, 0, 0, 0);
            var diagnosticVariant = TerrainReplayOptions.Enabled
                ? TerrainReplayOptions.Variant : TerrainReplayVariant.Baseline;
            var drawResult = reject && diagnosticVariant == TerrainReplayVariant.DisableChunkCulling
                ? "AB_CULLING_DISABLED" : "DRAW_RECORDED";
            ProbeTerrainChunk(probeChunks++, state, chunk, bounds, origin, (int)lod,
                true, resource.Revision, reject ? rejectPlane : -1,
                (int)gpu.IndexCount, drawResult, lodSelection, previousLod);
            draws.Add(new(chunk.ChunkX, chunk.ChunkY, (int)lod, (int)gpu.IndexCount / 3, (int)gpu.IndexCount));
        }
        var counts = cache.CreationCounts;
        var frameStats = TerrainRenderStats.From(draws, culled) with
        { TerrainResidentGpuBytes = (long)cache.ResidentGpuBytes, MeshBuildCount = counts.Mesh,
            VertexUploadCount = counts.Vertex, IndexUploadCount = counts.Index, BufferCreateCount = counts.Buffers };
        TerrainStats = Merge(TerrainStats, frameStats);
        BindProceduralVertexBuffer(cb);
    }

    static TerrainRenderStats Merge(TerrainRenderStats a, TerrainRenderStats b) => new(
        a.VisibleChunks + b.VisibleChunks, a.CulledChunks + b.CulledChunks,
        a.LOD0Chunks + b.LOD0Chunks, a.LOD1Chunks + b.LOD1Chunks,
        a.LOD2Chunks + b.LOD2Chunks, a.LOD3Chunks + b.LOD3Chunks,
        a.LOD4Chunks + b.LOD4Chunks, a.TerrainDrawCalls + b.TerrainDrawCalls,
        a.TerrainTriangles + b.TerrainTriangles, a.TerrainIndices + b.TerrainIndices,
        b.TerrainResidentGpuBytes, b.MeshBuildCount, b.VertexUploadCount,
        b.IndexUploadCount, b.BufferCreateCount);
}

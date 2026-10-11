using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    TerrainLodLevel SelectReplayTerrainLod(TerrainLodStateKey key,
        ViewProjectionState state, SpatialAabb bounds, TerrainChunkDescriptor chunk)
    {
        var selected = _terrainLodStates.Select(key, state, bounds,
            chunk.CellCountX, chunk.CellCountY).Lod;
        if (!TerrainReplayOptions.Enabled) return (TerrainLodLevel)selected;
        return (TerrainLodLevel)TerrainReplayOptions.SelectLod(selected,
            TerrainReplayOptions.Variant);
    }

    static void ProbeTerrainChunk(int index, ViewProjectionState state,
        TerrainChunkDescriptor chunk, SpatialAabb bounds,
        XuanYu.Core.Math.Vector3d origin, int lod,
        bool visible, int revision, int rejectedPlane, int indexCount, string result)
    {
        if (!TerrainReplayOptions.Enabled && index >= 2) return;
        TerrainProbeLogger.Chunk(state, chunk, bounds, origin, lod, visible,
            result == "DRAW_RECORDED" || result == "AB_CULLING_DISABLED",
            revision, rejectedPlane, indexCount, result);
    }
}

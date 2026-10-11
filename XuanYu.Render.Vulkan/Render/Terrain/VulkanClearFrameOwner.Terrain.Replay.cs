using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    TerrainLodSelection SelectReplayTerrainLod(TerrainLodStateKey key,
        ViewProjectionState state, SpatialAabb bounds, TerrainChunkDescriptor chunk,
        out int previousLod, out TerrainLodLevel drawLod)
    {
        previousLod = TerrainReplayOptions.Enabled
            ? _terrainLodStates.PreviousLod(key) : -1;
        var selection = _terrainLodStates.Select(key, state, bounds,
            chunk.CellCountX, chunk.CellCountY);
        drawLod = TerrainReplayOptions.Enabled
            ? (TerrainLodLevel)TerrainReplayOptions.SelectLod(selection.Lod,
                TerrainReplayOptions.Variant)
            : (TerrainLodLevel)selection.Lod;
        return selection;
    }

    static void ProbeTerrainChunk(int index, ViewProjectionState state,
        TerrainChunkDescriptor chunk, SpatialAabb bounds,
        XuanYu.Core.Math.Vector3d origin, int lod,
        bool visible, int revision, int rejectedPlane, int indexCount, string result,
        TerrainLodSelection? selection = null, int previousLod = -1)
    {
        if (!TerrainReplayOptions.Enabled && index >= 2) return;
        TerrainProbeLogger.Chunk(state, chunk, bounds, origin, lod, visible,
            result == "DRAW_RECORDED" || result == "AB_CULLING_DISABLED",
            revision, rejectedPlane, indexCount, result, selection, previousLod);
    }
}

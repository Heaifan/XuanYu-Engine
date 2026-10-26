using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;
using XuanYu.Core.Space.Diagnostics;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;
using System.Globalization;

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
        var variant = TerrainReplayOptions.Variant;
        var budget = TerrainReplayOptions.SseBudgetPixels(variant);
        if (TerrainReplayOptions.Enabled && budget.HasValue)
        {
            var measurements = new TerrainSseLodMeasurement[TerrainLodSelector.MaxLod + 1];
            for (var lod = TerrainLodSelector.MinLod; lod <= TerrainLodSelector.MaxLod; lod++)
            {
                double? estimatedPixels = null;
                if (TerrainSseReplayErrorTable.TryGet(key.TerrainId, key.Revision,
                        key.ChunkX, key.ChunkY, lod, out var errorMeters) &&
                    TerrainSseProjectionEstimator.TryEstimateMaxPixelError(state, bounds,
                        errorMeters, out var pixels)) estimatedPixels = pixels;
                measurements[lod] = new(lod, estimatedPixels);
            }
            var candidate = TerrainSseCandidateSelector.Select(measurements, budget.Value,
                previousLod < 0 ? null : previousLod, TerrainLodSelector.HysteresisRatio,
                TerrainSseProjectionEstimator.CameraInside(state, bounds),
                TerrainSseProjectionEstimator.NearPlaneIntersects(state, bounds));
            _terrainLodStates.Remember(key, candidate.ChosenLod);
            selection = selection with { Lod = candidate.ChosenLod };
            ViewportProbe.Log("terrain", $"[TERRAIN-SSE] TerrainId={key.TerrainId};Revision={key.Revision};" +
                $"Chunk={key.ChunkX},{key.ChunkY};BudgetPx={budget.Value.ToString(CultureInfo.InvariantCulture)};" +
                $"ChosenLod={candidate.ChosenLod};EstimatedMaxPixels={candidate.EstimatedMaxPixels?.ToString("R", CultureInfo.InvariantCulture) ?? "UNKNOWN"};" +
                $"Reason={candidate.Reason};Classification={candidate.Classification};" +
                $"CameraRevision={state.Camera.Revision};ErrorTable={TerrainSseReplayErrorTable.LoadStatus}");
        }
        var chosen = TerrainReplayOptions.Enabled
            ? TerrainReplayOptions.SelectLod(selection.Lod, variant) : selection.Lod;
        drawLod = (TerrainLodLevel)chosen;
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

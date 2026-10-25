using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Render.Abstractions;

public static class TerrainProbeLogger
{
    public static void Center(ViewProjectionState state, TerrainRenderResource resource)
    {
        if (!ViewportProbe.Enabled) return;
        var width = (resource.Heightfield.Width - 1) * resource.CellSizeMeters;
        var depth = (resource.Heightfield.Height - 1) * resource.CellSizeYMeters;
        var center = resource.WorldOrigin + new Vector3d(width / 2, depth / 2, 0);
        var projected = state.TryProjectWorldPoint(center, out var screen);
        var fingerprint = TerrainReplayOptions.Enabled
            ? TerrainSnapshotFingerprint.Compute(resource)
            : "NOT_CAPTURED";
        var inside = projected && screen.X >= 0 && screen.Y >= 0 &&
            screen.X <= state.Viewport.LogicalWidth &&
            screen.Y <= state.Viewport.LogicalHeight;
        ViewportProbe.Log("terrain", $"[TERRAIN-SNAPSHOT] TerrainId={resource.TerrainId};" +
            $"Revision={resource.Revision};Size={resource.Heightfield.Width}x" +
            $"{resource.Heightfield.Height};CellSizeXY={resource.CellSizeMeters}," +
            $"{resource.CellSizeYMeters};WorldOrigin={resource.WorldOrigin};" +
            $"Extent={resource.Metadata.WorldExtent};HeightfieldSha256={fingerprint};" +
            $"BoundsCenter={center};Screen=({screen.X:0.###},{screen.Y:0.###});" +
            $"InsideViewport={inside};Camera={state.Camera.Position};" +
            $"Forward={state.Camera.Forward};RenderOrigin={state.RenderOrigin}");
        if (TerrainReplayOptions.Enabled) LogView(state, "[TERRAIN-VIEW]");
    }

    public static void Chunk(ViewProjectionState state, TerrainChunkDescriptor chunk,
        SpatialAabb worldBounds, Vector3d origin, int lod, bool visible, bool submitted, int revision,
        int rejectedPlane = -1, int indexCount = 0, string result = "DRAW_RECORDED",
        TerrainLodSelection? selection = null, int previousLod = -1)
    {
        if (!ViewportProbe.Enabled) return;
        var clip = TerrainClipBoundsMetrics.Measure(state, worldBounds);
        var renderMin = worldBounds.Min - origin; var renderMax = worldBounds.Max - origin;
        var draw = submitted ? "True" : "False";
        var cull = visible ? "VISIBLE" : "CULLED";
        ViewportProbe.Log("terrain", $"[TERRAIN-CHUNK] TerrainId={chunk.TerrainId};" +
            $"Revision={revision};Chunk={chunk.ChunkX},{chunk.ChunkY};" +
            $"WorldBounds={worldBounds.Min}->{worldBounds.Max};" +
            $"RenderBounds={renderMin}->{renderMax};" +
            $"Cells={chunk.CellCountX}x{chunk.CellCountY};LOD={lod};" +
            $"CullResult={cull};RejectPlane={PlaneName(rejectedPlane)};" +
            $"Result={result};DrawRecorded={draw};IndexCount={indexCount};" +
            $"PreviousLod={previousLod};AutoLod={selection?.Lod ?? -1};" +
            $"ProjectedPixels={selection?.ProjectedPixels ?? 0:0.########};" +
            $"BaseCellPixels={selection?.BaseCellPixels ?? 0:0.########};" +
            $"ClipFrontCorners={clip.FrontFacingCornerCount};" +
            $"ClipFarOutsideCorners={clip.FarOutsideCornerCount};" +
            $"ClipNearOutsideCorners={clip.NearOutsideCornerCount};" +
            $"ClipDepthRatio=({clip.MinDepthRatio:0.######},{clip.MaxDepthRatio:0.######});" +
            $"ProjectedBounds=({clip.ProjectedBounds.MinX:0.###},{clip.ProjectedBounds.MinY:0.###})-" +
            $"({clip.ProjectedBounds.MaxX:0.###},{clip.ProjectedBounds.MaxY:0.###});" +
            $"RenderOrigin={state.RenderOrigin};CameraRevision={state.Camera.Revision}");
        if (TerrainReplayOptions.Enabled)
            LogView(state, $"[TERRAIN-CHUNK-VIEW] Chunk={chunk.ChunkX},{chunk.ChunkY}");
    }

    static void LogView(ViewProjectionState state, string prefix) =>
        ViewportProbe.Log("terrain", $"{prefix};Camera={state.Camera.Position};" +
            $"Forward={state.Camera.Forward};Fov={state.Camera.VerticalFovDegrees};" +
            $"NearPlane={state.Camera.NearPlane};FarPlane={state.Camera.FarPlane};" +
            $"CameraRevision={state.Camera.Revision};RenderOrigin={state.RenderOrigin}");

    static string PlaneName(int plane) => plane switch
    {
        0 => "LEFT",
        1 => "RIGHT",
        2 => "BOTTOM",
        3 => "TOP",
        4 => "FAR_REVERSE_Z",
        5 => "NEAR_REVERSE_Z",
        _ => "NONE"
    };
}

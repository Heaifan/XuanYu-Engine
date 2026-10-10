using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

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
        ViewportProbe.Log("terrain", $"[TERRAIN-CENTER] TerrainId={resource.TerrainId};TerrainBoundsCenterWorld={center};Screen=({screen.X:0.###},{screen.Y:0.###});InsideViewport={projected && screen.X >= 0 && screen.Y >= 0 && screen.X <= state.Viewport.LogicalWidth && screen.Y <= state.Viewport.LogicalHeight};CameraPosition={state.Camera.Position};Forward={state.Camera.Forward};ObservationCenter=UNKNOWN;RenderOrigin={state.RenderOrigin};Bounds={resource.Metadata.WorldExtent}");
    }

    public static void Chunk(ViewProjectionState state, TerrainChunkDescriptor chunk,
        Vector3d origin, int lod, bool visible, bool submitted, int revision)
    {
        if (!ViewportProbe.Enabled) return;
        var min = new Vector3d(chunk.WorldBounds.MinX, chunk.WorldBounds.MinY, chunk.WorldBounds.MinZ) + origin;
        var max = new Vector3d(chunk.WorldBounds.MaxX, chunk.WorldBounds.MaxY, chunk.WorldBounds.MaxZ) + origin;
        var a = state.TryProjectWorldPoint(min, out var amin);
        var b = state.TryProjectWorldPoint(max, out var amax);
        ViewportProbe.Log("terrain", $"[TERRAIN-CHUNK] ChunkId={chunk.ChunkX},{chunk.ChunkY};WorldBounds={min}->{max};RenderBounds={chunk.WorldBounds};LOD={lod};FrustumVisible={visible};RenderOriginRevision={state.Camera.Revision};ModelTransformRevision={revision};MeshRevision={revision};ProjectedBounds=({(a ? amin.X : double.NaN):0.###},{(a ? amin.Y : double.NaN):0.###})-({(b ? amax.X : double.NaN):0.###},{(b ? amax.Y : double.NaN):0.###});DrawSubmitted={submitted}");
    }
}

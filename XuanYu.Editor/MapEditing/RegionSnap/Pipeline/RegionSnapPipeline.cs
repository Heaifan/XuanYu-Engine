using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionSnapPipeline
{
    public static RegionEdgeSnapResult Resolve(
        MapRegionId sourceRegionId, MapPoint rawWorldPoint, ScreenPoint pointer,
        MapDefinition map, ViewProjectionState projection, RegionSnapState state,
        Func<RegionSpatialBounds, ImmutableArray<MapRegionId>> localQuery,
        Func<MapRegionId, MapRegion?> regionLookup, RegionEdgeSnapSettings settings)
    {
        settings.Validate();
        if (!RegionSnapQuery.TryBounds(pointer.X, pointer.Y, settings.ReleaseRadiusPx, map, projection, out var bounds))
        { state.Clear(); return RegionEdgeSnapResult.Raw(rawWorldPoint); }
        ImmutableArray<MapRegionId> ids;
        try { ids = localQuery(bounds); }
        catch (InvalidOperationException) { state.Clear(); return RegionEdgeSnapResult.Raw(rawWorldPoint); }
        var candidates = RegionSnapQuery.BuildCandidates(ids, regionLookup, map, projection);
        if (state.Kind == RegionSnapKind.Vertex && TryKeepVertex(state, pointer, candidates, settings.ReleaseRadiusPx, out var heldVertex))
            return heldVertex;
        if (state.Kind == RegionSnapKind.Edge)
        {
            var priority = RegionEdgeSnapResolver.Resolve(sourceRegionId, rawWorldPoint, pointer, candidates, Enter(settings));
            if (priority.Kind == RegionSnapKind.Vertex) return Acquire(state, priority);
            if (RegionEdgeSnapLockResolver.TryResolve(state.TargetRegionId!.Value, state.TargetSegmentIndex,
                    pointer, candidates, settings.ReleaseRadiusPx, out var heldEdge)) return Acquire(state, heldEdge);
            state.Clear();
            return AcquireIfSnapped(state, priority, rawWorldPoint);
        }
        state.Clear();
        var result = RegionEdgeSnapResolver.Resolve(sourceRegionId, rawWorldPoint, pointer, candidates, Enter(settings));
        return AcquireIfSnapped(state, result, rawWorldPoint);
    }

    static bool TryKeepVertex(RegionSnapState state, ScreenPoint pointer,
        ImmutableArray<RegionEdgeSnapRegion> candidates, double radius, out RegionEdgeSnapResult result)
    {
        result = default;
        var region = candidates.FirstOrDefault(item => item.RegionId == state.TargetRegionId);
        if (region.RegionId != state.TargetRegionId || state.TargetVertexIndex < 0 ||
            state.TargetVertexIndex >= region.Vertices.Length) return false;
        var vertex = region.Vertices[state.TargetVertexIndex];
        if (DistanceSquared(pointer, vertex.ScreenPoint) > radius * radius) return false;
        result = new(vertex.WorldPoint, RegionSnapKind.Vertex, region.RegionId, state.TargetVertexIndex,
            DistanceSquared(pointer, vertex.ScreenPoint));
        return true;
    }

    static RegionEdgeSnapResult Acquire(RegionSnapState state, RegionEdgeSnapResult result)
    { state.Acquire(result); return result; }

    static RegionEdgeSnapResult AcquireIfSnapped(RegionSnapState state, RegionEdgeSnapResult result, MapPoint raw)
    { if (result.IsSnapped) state.Acquire(result); return result.IsSnapped ? result : RegionEdgeSnapResult.Raw(raw); }

    static RegionEdgeSnapSettings Enter(RegionEdgeSnapSettings settings) => settings with { ReleaseRadiusPx = settings.EnterRadiusPx };

    static double DistanceSquared(ScreenPoint a, ScreenPoint b) =>
        Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
}

using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class GeometrySnapPipeline
{
    public static GeometrySnapResult Resolve(GeometryFeatureKey source, MapPoint raw, ScreenPoint pointer,
        MapDefinition map, ViewProjectionState projection, GeometrySnapState state,
        Func<RegionSpatialBounds, ImmutableArray<GeometryFeatureKey>> localQuery,
        RegionEdgeSnapSettings settings)
    {
        settings.Validate();
        if (!GeometrySnapQuery.TryBounds(pointer.X, pointer.Y, settings.ReleaseRadiusPx, map, projection, out var bounds))
        { state.Clear(); return GeometrySnapResult.Raw(raw); }
        ImmutableArray<GeometryFeatureKey> keys;
        try { keys = localQuery(bounds); } catch (InvalidOperationException) { state.Clear(); return GeometrySnapResult.Raw(raw); }
        var candidates = GeometrySnapQuery.BuildCandidates(keys, map, projection);
        if (state.IsSnapped && TryKeep(source, pointer, candidates, state, settings.ReleaseRadiusPx, out var held)) return held;
        state.Clear();
        var result = GeometrySnapArbitration.Resolve(source, raw, pointer, candidates, settings.EnterRadiusPx);
        if (result.IsSnapped) state.Acquire(result);
        return result;
    }

    static bool TryKeep(GeometryFeatureKey source, ScreenPoint pointer,
        ImmutableArray<GeometryFeatureAdapter> features, GeometrySnapState state,
        double radius, out GeometrySnapResult result)
    {
        result = default;
        if (state.Target is not { } target || !GeometrySnapPolicy.CanTarget(source, target)) return false;
        var feature = features.FirstOrDefault(item => item.Key == target);
        if (feature.Key != target) return false;
        if (state.Kind == GeometrySnapKind.Vertex && state.TargetIndex < feature.Points.Length)
        {
            var vertex = GeometrySnapQuery.Vertices(feature)[state.TargetIndex];
            if (Distance(pointer, vertex.ScreenPoint) <= radius * radius)
            { result = new(vertex.WorldPoint, GeometrySnapKind.Vertex, target, state.TargetIndex, Distance(pointer, vertex.ScreenPoint)); return true; }
        }
        if (state.Kind == GeometrySnapKind.Segment && state.TargetIndex < feature.SegmentCount)
        {
            var segment = GeometrySnapQuery.Segments(feature)[state.TargetIndex];
            if (RegionEdgeSnapGeometry.TryClosestPoint(pointer, segment.StartScreen, segment.EndScreen, out var closest, out var parameter))
            {
                var distance = Distance(pointer, closest);
                if (distance <= radius * radius && (segment.Closed || parameter > double.Epsilon && parameter < 1 - double.Epsilon))
                { result = new(ClosestWorld(segment, pointer), GeometrySnapKind.Segment, target, state.TargetIndex, distance); return true; }
            }
        }
        return false;
    }

    static MapPoint ClosestWorld(GeometrySegmentCandidate segment, ScreenPoint pointer)
    {
        RegionEdgeSnapGeometry.TryClosestPoint(pointer, segment.StartScreen, segment.EndScreen, out _, out var t);
        return new(segment.Start.X + (segment.End.X - segment.Start.X) * t, segment.Start.Y + (segment.End.Y - segment.Start.Y) * t);
    }
    static double Distance(ScreenPoint a, ScreenPoint b) => Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
}

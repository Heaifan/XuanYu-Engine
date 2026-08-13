using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

static class GeometrySnapArbitration
{
    public static GeometrySnapResult Resolve(GeometryFeatureKey source, MapPoint raw,
        ScreenPoint pointer, ImmutableArray<GeometryFeatureAdapter> features, double radius)
    {
        var vertex = BestVertex(source, pointer, features, radius);
        if (vertex is { } hit) return new(hit.WorldPoint, GeometrySnapKind.Vertex, hit.Feature, hit.Index, Distance(pointer, hit.ScreenPoint));
        var segment = BestSegment(source, pointer, features, radius);
        return segment is { } edge ? new(ClosestWorld(edge, pointer), GeometrySnapKind.Segment,
            edge.Feature, edge.Index, Distance(pointer, Closest(edge, pointer))) : GeometrySnapResult.Raw(raw);
    }

    static GeometryVertexCandidate? BestVertex(GeometryFeatureKey source, ScreenPoint pointer,
        ImmutableArray<GeometryFeatureAdapter> features, double radius)
    {
        GeometryVertexCandidate? best = null; GeometryFeatureKey? bestFeature = null; var bestIndex = -1;
        var bestDistance = double.PositiveInfinity; var limit = radius * radius;
        foreach (var feature in features)
            if (GeometrySnapPolicy.CanTarget(source, feature.Key))
                foreach (var vertex in GeometrySnapQuery.Vertices(feature))
                {
                    var distance = Distance(pointer, vertex.ScreenPoint);
                    if (distance <= limit && Better(bestFeature, bestIndex, bestDistance, distance, vertex.Feature, vertex.Index))
                    { best = vertex; bestFeature = vertex.Feature; bestIndex = vertex.Index; bestDistance = distance; }
                }
        return best;
    }

    static GeometrySegmentCandidate? BestSegment(GeometryFeatureKey source, ScreenPoint pointer,
        ImmutableArray<GeometryFeatureAdapter> features, double radius)
    {
        GeometrySegmentCandidate? best = null; GeometryFeatureKey? bestFeature = null; var bestIndex = -1;
        var bestDistance = double.PositiveInfinity; var limit = radius * radius;
        foreach (var feature in features)
            if (GeometrySnapPolicy.CanTarget(source, feature.Key))
                foreach (var segment in GeometrySnapQuery.Segments(feature))
                {
                    if (!RegionEdgeSnapGeometry.TryClosestPoint(pointer, segment.StartScreen, segment.EndScreen,
                            out var closest, out var parameter) || (!segment.Closed &&
                            (parameter <= double.Epsilon || parameter >= 1 - double.Epsilon))) continue;
                    var distance = Distance(pointer, closest);
                    if (distance <= limit && Better(bestFeature, bestIndex, bestDistance, distance, segment.Feature, segment.Index))
                    { best = segment; bestFeature = segment.Feature; bestIndex = segment.Index; bestDistance = distance; }
                }
        return best;
    }

    static bool Better(GeometryFeatureKey? bestFeature, int bestIndex, double bestDistance,
        double distance, GeometryFeatureKey feature, int index) =>
        bestFeature is null || distance < bestDistance ||
        distance == bestDistance && Compare(bestFeature.Value, feature, bestIndex, index);

    static bool Compare(GeometryFeatureKey current, GeometryFeatureKey next, int currentIndex, int nextIndex) =>
        string.CompareOrdinal(next.ToString(), current.ToString()) < 0 || current == next && nextIndex < currentIndex;
    static ScreenPoint Closest(GeometrySegmentCandidate segment, ScreenPoint point) =>
        RegionEdgeSnapGeometry.TryClosestPoint(point, segment.StartScreen, segment.EndScreen, out var closest, out _) ? closest : segment.StartScreen;
    static MapPoint ClosestWorld(GeometrySegmentCandidate segment, ScreenPoint pointer)
    {
        RegionEdgeSnapGeometry.TryClosestPoint(pointer, segment.StartScreen, segment.EndScreen, out _, out var t);
        return new(segment.Start.X + (segment.End.X - segment.Start.X) * t, segment.Start.Y + (segment.End.Y - segment.Start.Y) * t);
    }
    static double Distance(ScreenPoint a, ScreenPoint b) => DistanceSquared(a, b);
    static double DistanceSquared(ScreenPoint a, ScreenPoint b) => Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
}

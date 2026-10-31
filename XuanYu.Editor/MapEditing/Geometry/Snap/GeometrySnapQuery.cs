using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class GeometrySnapQuery
{
    public static bool TryBounds(double x, double y, double radius, MapDefinition map,
        ViewProjectionState projection, out RegionSpatialBounds bounds) =>
        RegionSnapQuery.TryBounds(x, y, radius, map, projection, out bounds);

    public static ImmutableArray<GeometryFeatureAdapter> BuildCandidates(
        ImmutableArray<GeometryFeatureKey> keys, MapDefinition map,
        ViewProjectionState projection)
    {
        var result = ImmutableArray.CreateBuilder<GeometryFeatureAdapter>();
        foreach (var key in keys)
            if (GeometryFeatureAdapters.TryGet(map, key, out var feature) &&
                feature.Capabilities.HasFlag(GeometryCapabilities.SnapTarget) &&
                BuildProjected(feature, map.Surface.BaseHeightMeters, projection, out var projected))
                result.Add(projected);
        return result.ToImmutable();
    }

    public static ImmutableArray<GeometryVertexCandidate> Vertices(GeometryFeatureAdapter feature) =>
        feature.Points.Select((point, index) => new GeometryVertexCandidate(
            feature.Key, index, point, feature.Projected[index])).ToImmutableArray();

    public static ImmutableArray<GeometrySegmentCandidate> Segments(GeometryFeatureAdapter feature)
    {
        var count = feature.SegmentCount; var result = ImmutableArray.CreateBuilder<GeometrySegmentCandidate>(count);
        for (var index = 0; index < count; index++)
        {
            var next = feature.IsClosed ? (index + 1) % feature.Points.Length : index + 1;
            result.Add(new(feature.Key, index, feature.Points[index], feature.Points[next],
                feature.Projected[index], feature.Projected[next], feature.IsClosed));
        }
        return result.ToImmutable();
    }

    static bool BuildProjected(GeometryFeatureAdapter feature, double height,
        ViewProjectionState projection, out GeometryFeatureAdapter projected)
    {
        var screens = ImmutableArray.CreateBuilder<ScreenPoint>(feature.Points.Length);
        foreach (var point in feature.Points)
            if (!projection.TryProjectWorldPoint(new(point.X, point.Y, height), out var screen))
            { projected = default; return false; }
            else screens.Add(screen);
        projected = feature with { Projected = screens.ToImmutable() };
        return true;
    }
}
